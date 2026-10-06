# Kontekst projektu — Aplikacja do tworzenia dźwiękowych scen 3D

> Ten plik podsumowuje ustalenia i decyzje wypracowane w rozmowie z Claude (claude.ai)
> dot. pracy inżynierskiej. Umieść go jako `CLAUDE.md` w katalogu głównym repozytorium
> (Claude Code czyta go automatycznie) lub skopiuj do `.github/copilot-instructions.md`
> (GitHub Copilot Chat w VS2026 — wymaga włączenia opcji w Tools > Options > GitHub >
> Copilot > Copilot Chat).

## Temat pracy

„Projekt i implementacja aplikacji do tworzenia dźwiękowych scen 3D dla potrzeb
wspomagania nauki orientacji przestrzennej osób niewidomych”. Autor: Sebastian Świnarski.
Promotor jest osobą niewidomą — dostępność (accessibility) to priorytet nadrzędny nad
resztą funkcjonalności, nie dodatek.

Aplikacja symuluje poruszanie się po środowisku (plac miejski, ulica, wnętrze budynku)
z rozmieszczonymi źródłami dźwięku 3D. Użytkownik uczy się lokalizować dźwięki i budować
mentalną mapę otoczenia wyłącznie na podstawie słuchu.

## Stos technologiczny (i DLACZEGO — ważne, nie zmieniać bez powodu)

| Warstwa | Technologia | Uzasadnienie |
|---|---|---|
| Platforma | **.NET 10 (LTS)**, C# | .NET 8 traci wsparcie 10.11.2026 — zbyt wcześnie względem harmonogramu pracy. .NET 10 wspierane do 11.2028. |
| Audio 3D | **OpenAL Soft** (`OpenTK.Audio.OpenAL`) | HRTF, tłumienie z odległością, natywnie kompatybilne z nowoczesnym .NET. |
| UI | **Avalonia UI** (MVVM, `CommunityToolkit.Mvvm`) | Wieloplatformowość, rosnące wsparcie accessibility, furtka na przyszły port poza Windows. |
| Format map | **JSON** (`System.Text.Json`) | Czytelny, łatwy do rozszerzenia. |
| IDE | **Visual Studio 2026** | Aktualna wersja (następca VS2022). |

### Odrzucona technologia: Slab3D

Pierwotnie planowany framework. **Zrezygnowano z niego świadomie i z konkretnego,
zweryfikowanego powodu** — nie mów, że jest „porzucony od 2018”, bo to nieprawda:
- Ostatnie wydanie: **6.8.4, kwiecień 2023**, opiekun: U.S. Army Futures Command (nadal
  formalnie utrzymywany).
- Realny problem: wymaga .NET Framework 4/4.5, Visual Studio 2017, MonoGame 3.7/XNA,
  DirectX SDK z 2010 — niekompatybilne z .NET 10, tylko Windows.
- API (SRAPI) w C++ z managed wrapperem (slabsharp) — nieidiomatyczne dla C#.
- Profil zastosowań: lotniczo-wojskowy (sonifikacja przeszkód dla pilotów, DIS radio) —
  odległy od tematu pracy.

## Kluczowe decyzje koncepcyjne (accessibility-first)

1. **Tworzenie scen dźwiękowych — NIE klasyczny edytor GUI.** Żadnego canvasu z myszą.
   Użytkownik (także niewidomy, samodzielnie!) porusza się po scenie w tym samym trybie
   nawigacji co przy odtwarzaniu, i w bieżącej pozycji umieszcza źródło dźwięku wybrane
   z biblioteki. To świadomy wybór różniący projekt od typowych „edytorów map”.

2. **Informacja zwrotna o pozycji/orientacji — NIE ciągła narracja.** Stały strumień
   komunikatów zagłuszałby scenę dźwiękową, którą użytkownik ma słuchać — to sprzeczne
   z celem aplikacji. Zamiast tego:
   - informacja **na żądanie** (osobny klawisz, np. „I”) — ogłasza kierunek świata
     („północ”, nie stopnie) i pozycję względem najbliższego punktu orientacyjnego,
     nie surowe współrzędne XYZ;
   - komunikaty **zdarzeniowe**, tylko przy istotnych zmianach: wejście w zasięg źródła
     dźwięku, kolizja, potwierdzenie zapisu/dodania źródła w trybie tworzenia sceny.

3. Sterowanie wyłącznie klawiaturą (WSAD ruch, Q/E obrót), zero zależności od myszy.

## Stan projektu (na dziś)

- **Prototyp `SpatialAudioNavigator`** (Core + Desktop) — zaimplementowany: silnik audio
  3D (OpenAL), nawigacja gracza, loader map JSON, podstawowy UI Avalonia. Zaktualizowany
  do `net10.0`. **Nieprzetestowany jeszcze w realnym Visual Studio** (pisany bez dostępu
  do środowiska z NuGet) — pierwsze uruchomienie samo w sobie jest testem.
- **Dokument założeń projektowych** (`zalozenia_projektu.docx`) wysłany do promotora:
  cel, zakres, grupa docelowa, analiza rynku (Seeing Assistant Move, DotWalker, NavCog —
  żadna nie robi dokładnie tego samo), funkcjonalności, wymagania funkcjonalne WF-01..12
  i niefunkcjonalne WN-01..10 (WN-01/WN-02 = dostępność, priorytet krytyczny).
- **Testy rozpoznawcze technologii** (`RozpoznanieTechnologii/01_AudioTest`,
  `02_UiTest`) — minimalne, niezależne projekty konsolowy (OpenAL, dźwięk generowany
  programowo, bez plików WAV) i Avalonia (UI+MVVM), z arkuszem wyników do wypełnienia.

## Ustalenia z promotorem

- Spotkania regularne, **co 2 tygodnie, online**, stały termin.
- Kolejność prac: rozpoznanie technologii → kilka izolowanych przykładów → sprawdzenie
  integracji technologii w praktyce → implementacja zrębu aplikacji.

## Otwarte ryzyka / do zweryfikowania w praktyce

- Avalonia UI nie ma dojrzałego odpowiednika ARIA live-region — obsługę czytnika ekranu
  (NVDA/JAWS) trzeba będzie zbudować przez UI Automation i realnie przetestować.
- Realizm HRTF zależy od sprzętu (słuchawek) użytkownika.
- Dostępność wolnych od praw autorskich nagrań dźwiękowych odpowiedniej jakości.

## Jak pracować nad tym kodem

- Priorytet: dostępność > funkcjonalność > estetyka. Każda decyzja UI/UX powinna dać się
  uzasadnić z perspektywy osoby niewidomej korzystającej z aplikacji wyłącznie klawiaturą.
- Nie proponuj rozwiązań wymagających myszy jako jedynej opcji.
- Nie cofaj decyzji o .NET 10 / OpenAL Soft / Avalonia bez wyraźnego powodu — zostały
  zweryfikowane i uzasadnione (patrz wyżej).
