using OpenTK.Audio.OpenAL;

namespace AudioTest;

class Program
{
    private const int SampleRate = 44100;

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Test rozpoznawczy: OpenAL Soft (audio 3D) na .NET 10 ===");
        Console.WriteLine();
        Console.WriteLine("UWAGA: załóż słuchawki stereo przed kontynuowaniem.");
        Console.WriteLine("Naciśnij ENTER, aby rozpocząć...");
        Console.ReadLine();

        // ---------- Inicjalizacja OpenAL ----------
        ALDevice device = ALC.OpenDevice(null);
        if (device == ALDevice.Null)
        {
            Console.WriteLine("BŁĄD: nie udało się otworzyć urządzenia audio. Test negatywny - sprawdź sterowniki dźwięku.");
            return;
        }

        ALContext context = ALC.CreateContext(device, (int[]?)null);
        ALC.MakeContextCurrent(context);

        var error = AL.GetError();
        if (error != ALError.NoError)
        {
            Console.WriteLine($"BŁĄD OpenAL przy inicjalizacji: {error}");
            return;
        }

        AL.DistanceModel(ALDistanceModel.InverseDistanceClamped);

        Console.WriteLine("OK: OpenAL zainicjalizowany poprawnie.");
        Console.WriteLine($"    Urządzenie: {ALC.GetString(device, AlcGetString.DeviceSpecifier)}");
        Console.WriteLine();

        // ---------- Przygotowanie dwóch tonów (bez plików WAV) ----------
        byte[] toneA = GenerateSineWave(frequencyHz: 440, durationSeconds: 3.0f);  // ton "A"
        byte[] toneB = GenerateSineWave(frequencyHz: 880, durationSeconds: 3.0f);  // ton "B", oktawę wyżej

        int bufferA = AL.GenBuffer();
        int bufferB = AL.GenBuffer();
        AL.BufferData(bufferA, ALFormat.Mono16, toneA, SampleRate);
        AL.BufferData(bufferB, ALFormat.Mono16, toneB, SampleRate);

        int sourceA = AL.GenSource();
        int sourceB = AL.GenSource();

        AL.Source(sourceA, ALSourcei.Buffer, bufferA);
        AL.Source(sourceB, ALSourcei.Buffer, bufferB);
        AL.Source(sourceA, ALSourceb.Looping, true);
        AL.Source(sourceB, ALSourceb.Looping, true);
        AL.Source(sourceA, ALSourcef.ReferenceDistance, 3.0f);
        AL.Source(sourceB, ALSourcef.ReferenceDistance, 3.0f);
        AL.Source(sourceA, ALSourcef.MaxDistance, 60.0f);
        AL.Source(sourceB, ALSourcef.MaxDistance, 60.0f);

        // Słuchacz na pozycji (0,0,0), patrzy domyślnie w kierunku +Z.
        SetListener(position: (0, 0, 0), facingRadians: 0);

        // ---------- TEST 1: lewo / prawo ----------
        Console.WriteLine("=== TEST 1: panning lewo / prawo ===");
        AL.Source(sourceA, ALSource3f.Position, -5f, 0f, 0f); // ton A: lewo
        AL.Source(sourceB, ALSource3f.Position, 5f, 0f, 0f);  // ton B: prawo
        AL.SourcePlay(sourceA);
        AL.SourcePlay(sourceB);
        Console.WriteLine("Powinieneś słyszeć: ton 440 Hz po LEWEJ stronie, ton 880 Hz po PRAWEJ stronie.");
        Console.WriteLine("Naciśnij ENTER, aby przejść do testu 2...");
        Console.ReadLine();

        // ---------- TEST 2: obrót słuchacza o 360° (test kluczowy) ----------
        Console.WriteLine();
        Console.WriteLine("=== TEST 2: obrót słuchacza o 360° ===");
        Console.WriteLine("Oba dźwięki zostają nieruchomo z przodu/z tyłu - obracamy słuchacza.");
        AL.Source(sourceA, ALSource3f.Position, 0f, 0f, 5f);  // ton A: przed słuchaczem
        AL.Source(sourceB, ALSource3f.Position, 0f, 0f, -5f); // ton B: za słuchaczem
        Console.WriteLine("Start obrotu za 2 sekundy...");
        Thread.Sleep(2000);

        const int steps = 200;
        const float totalSeconds = 8f;
        for (int i = 0; i <= steps; i++)
        {
            float angle = (float)(i / (double)steps * 2 * Math.PI);
            SetListener(position: (0, 0, 0), facingRadians: angle);
            Thread.Sleep((int)(totalSeconds * 1000 / steps));
        }

        Console.WriteLine("Obrót zakończony. Dźwięki powinny były 'przepłynąć' dookoła głowy.");
        Console.WriteLine("Naciśnij ENTER, aby przejść do testu 3...");
        Console.ReadLine();

        // ---------- TEST 3: tłumienie z odległością ----------
        Console.WriteLine();
        Console.WriteLine("=== TEST 3: głośność a odległość ===");
        AL.SourceStop(sourceB);
        SetListener(position: (0, 0, 0), facingRadians: 0);
        Console.WriteLine("Ton A oddala się od słuchacza przez 6 sekund (z 2 m do 50 m).");

        const int distSteps = 120;
        for (int i = 0; i <= distSteps; i++)
        {
            float z = 2f + (50f - 2f) * (i / (float)distSteps);
            AL.Source(sourceA, ALSource3f.Position, 0f, 0f, z);
            Thread.Sleep(50);
        }

        Console.WriteLine("Ton A powinien był wyraźnie ucichnąć wraz z odległością.");
        Console.WriteLine("Naciśnij ENTER, aby zakończyć test...");
        Console.ReadLine();

        // ---------- Sprzątanie ----------
        AL.SourceStop(sourceA);
        AL.SourceStop(sourceB);
        AL.DeleteSource(sourceA);
        AL.DeleteSource(sourceB);
        AL.DeleteBuffer(bufferA);
        AL.DeleteBuffer(bufferB);
        ALC.MakeContextCurrent(ALContext.Null);
        ALC.DestroyContext(context);
        ALC.CloseDevice(device);

        Console.WriteLine();
        Console.WriteLine("=== Test zakończony. ===");
    }

    private static void SetListener((float x, float y, float z) position, float facingRadians)
    {
        AL.Listener(ALListener3f.Position, position.x, position.y, position.z);
        AL.Listener(ALListener3f.Velocity, 0f, 0f, 0f);

        float[] orientation =
        {
            MathF.Sin(facingRadians), 0f, MathF.Cos(facingRadians),
            0f, 1f, 0f
        };
        AL.Listener(ALListenerfv.Orientation, ref orientation[0]);
    }

    private static byte[] GenerateSineWave(double frequencyHz, float durationSeconds)
    {
        int sampleCount = (int)(SampleRate * durationSeconds);
        var samples = new short[sampleCount];
        int fadeSamples = SampleRate / 50; // fade in/out, inaczej zapętlenie trzaska

        for (int i = 0; i < sampleCount; i++)
        {
            double t = i / (double)SampleRate;
            double value = Math.Sin(2 * Math.PI * frequencyHz * t);

            double fade = 1.0;
            if (i < fadeSamples) fade = i / (double)fadeSamples;
            else if (i > sampleCount - fadeSamples) fade = (sampleCount - i) / (double)fadeSamples;

            samples[i] = (short)(value * fade * short.MaxValue * 0.6);
        }

        var bytes = new byte[sampleCount * 2];
        Buffer.BlockCopy(samples, 0, bytes, 0, bytes.Length);
        return bytes;
    }
}
