# Rozpoznanie technologii — testy przygotowawcze do spotkania z promotorem

Ten folder zawiera **dwa niezależne, minimalne projekty testowe**, zgodnie z ustaleniem:
najpierw sprawdzić osobno każdą kluczową technologię, zanim zacznie się budować całą aplikację.

Oba projekty celowo **nie zależą od siebie nawzajem** i nie wymagają żadnych zewnętrznych
plików (audio generowane jest programowo) — można je uruchomić od razu po otwarciu w
Visual Studio.

---

## Wymagania

- Visual Studio 2026 (lub 2022 z zainstalowanym SDK .NET 10)
- .NET 10 SDK
- Słuchawki stereo (obowiązkowo do testu audio!)

Otwórz `RozpoznanieTechnologii.sln` w Visual Studio — oba projekty pojawią się w Solution
Explorer. Każdy można uruchomić osobno (prawy klik na projekt → **Set as Startup Project**,
potem F5).

---

## Projekt 1: `01_AudioTest` — OpenAL Soft (audio 3D)

**Czego dotyczy:** biblioteki `OpenTK.Audio.OpenAL`, przez którą aplikacja docelowa będzie
renderować dźwięk przestrzenny.

**Jak działa:** program konsolowy generuje dwa czyste tony (440 Hz i 880 Hz) programowo —
nie trzeba przygotowywać żadnych plików WAV. Przeprowadza po kolei 3 testy, z komunikatami
w konsoli mówiącymi, czego nasłuchiwać.

**Co sprawdza:**
1. Czy OpenAL Soft w ogóle inicjalizuje się poprawnie na danym komputerze,
2. Czy słychać różnicę lewo/prawo (panning stereo),
3. **Test kluczowy:** czy przy obrocie „głowy” słuchacza dźwięk realistycznie „przepływa”
   dookoła — to jest fundament całej pracy,
4. Czy głośność maleje wraz z odległością źródła.

Uruchom, załóż słuchawki, wykonaj po kolei testy (ENTER przechodzi dalej).

---

## Projekt 2: `02_UiTest` — Avalonia UI + MVVM

**Czego dotyczy:** frameworka interfejsu użytkownika (Avalonia) i wzorca MVVM
(`CommunityToolkit.Mvvm`), na których będzie oparty cały interfejs aplikacji.

**Co sprawdza:** czy okno się uruchamia, czy przycisk i bindowanie danych (kliknięcie →
zmiana tekstu i licznika) działają na .NET 10.

---

## Co dalej (po pozytywnym wyniku testów)

Jeśli oba testy wypadną pozytywnie, kolejnym krokiem (zgodnie z ustaleniami z promotorem)
jest połączenie obu technologii w jednym projekcie i zbudowanie zrębu aplikacji — mamy już
przygotowany taki szkielet (`SpatialAudioNavigator`), zaktualizowany do .NET 10, gotowy do
rozbudowy na kolejnym etapie.
