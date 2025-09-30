## Product Requirements Document — Desktop Organizer (Sprint 1)

### Document owner
- **owner**: Noah K
- **last updated**: 2025-09-26

## 1. Overview
A WinUI 3 desktop application that lets users organize their Windows desktop via customizable containers for files and folders, with icon customization and a scalable architecture for future features.

## 2. Objectives (Sprint 1)
- **Scaffold a clean, scalable UI shell** with navigation and placeholder pages.
- **Establish MVVM, DI, and app state patterns** for maintainability.
- **Stub persistence and services** (in-memory) that can be swapped for SQLite.
- **Prototype drag & drop** for creating items from files/folders.
- **Implement basic theming and settings** persisted locally.

## 3. Scope
- **In scope**
  - App shell: `NavigationView` + `Frame` navigation
  - Pages: Dashboard, Containers, Editor, Icons, Settings
  - MVVM setup with `CommunityToolkit.Mvvm`
  - DI/hosting via `Microsoft.Extensions.Hosting`
  - In-memory repositories for Containers/Items/Icons
  - Settings persistence (JSON in `%LocalAppData%`)
  - Drag & drop (internal and shell drop into Editor)
  - Basic icon import (copy to app cache; record entry)
- **Out of scope**
  - SQLite implementation and migrations
  - Advanced layout (snap-to-grid, alignment)
  - Batch icon management and SVG rasterization
  - Shell overlays/context menus; global hotkeys

## 4. Personas & primary use cases
- **Power organizer**: Creates themed containers, groups work items, switches quickly.
- **Minimalist**: Hides clutter into containers, uses keyboard and search.
- **Designer**: Custom icons for a branded look.

## 5. User stories (Sprint 1)
- As a user, I can navigate between Dashboard, Containers, Editor, Icons, and Settings.
- As a user, I can create a container and see it listed and previewed.
- As a user, I can drag a file/folder onto a container canvas to create an item tile.
- As a user, I can import an image to use as an icon for a container or item.
- As a user, I can switch theme (Light/Dark/System) and it persists.

## 6. Functional requirements
- **Navigation**
  - `NavigationView` hosts routes: Dashboard, Containers, Editor, Icons, Settings.
  - `INavigationService` abstracts navigation for viewmodels.
- **Containers**
  - Create, list, delete containers (rename optional).
  - `ContainersPage` lists containers; `ContainerPreviewControl` shows preview.
- **Editor**
  - Displays selected container as grid/canvas placeholder.
  - Accepts shell drop (files/folders) → creates `Item` entries and tiles.
- **Icons**
  - Import PNG/ICO/JPG; copy to `%LocalAppData%\DesktopOrganizer\icons\`.
  - Assign `IconAsset` to container or item; simple picker UI.
- **Settings**
  - Persist theme; grid size; snap toggles (snap can be stubbed).
- **State & messaging**
  - App-wide `AppState` holds selected container and settings.
  - Use Toolkit `WeakReferenceMessenger` for events (e.g., IconAdded).

## 7. Non-functional requirements
- Smooth navigation and 60fps with 100+ placeholder items.
- Basic accessibility: focus visuals, names, keyboard nav.
- Code quality: analyzers no errors; unit tests for core services/viewmodels.

## 8. Architecture overview
- Solution layout
  - `DesktopOrganizer.App` (WinUI 3 UI)
  - `DesktopOrganizer.Core` (models, interfaces)
  - `DesktopOrganizer.Infrastructure` (infrastructure implementations)
  - `DesktopOrganizer.Tests` (unit tests)
- Key interfaces in `Core`
  - `IContainerService`, `IItemService`, `IIconService`, `ISettingsService`, `IFileSystemService`, `INavigationService`, `IThemeService`
- Implementations in `Infrastructure` with in-memory repos for Sprint 1.

## 9. UI/UX and navigation
- **Shell**
  - Top `CommandBar`: New Container, Import Icon, Search (stub).
  - `NavigationView` left; `Frame` content area.
- **Pages**
  - `DashboardPage`: previews of recent/pinned containers.
  - `ContainersPage`: list with New button and selection to open Editor.
  - `EditorPage`: canvas/grid placeholder showing items; accepts drops.
  - `IconsPage`: gallery of imported icons; assign action (stub).
  - `SettingsPage`: theme picker; grid/snap toggles.
- **Controls**
  - `ContainerPreviewControl`, `ItemTileControl`.

## 10. Data model (initial)
- `Container`: Id, Name, Type (Grid|Canvas), Position, Size, LayoutOptions, CreatedAt, UpdatedAt
- `Item`: Id, ContainerId, Path, DisplayName, IconId, Position, Size, Tags
- `IconAsset`: Id, Name, FilePath, Variants, Source, Hash
- `Settings`: Theme, GridSize, SnapToGrid, ShowLabels

## 11. Persistence and storage
- Sprint 1: in-memory repositories; JSON for settings.
- Icons stored at `%LocalAppData%\DesktopOrganizer\icons\` with DB/JSON references.
- Plan for Sprint 2: SQLite with simple migrations.

## 12. Accessibility and localization
- Names for interactive elements; keyboard navigation; high-contrast support.
- English only in Sprint 1; resource dictionaries for future i18n.

## 13. Risks and mitigations
- WinUI project setup friction → use Visual Studio templates; document prerequisites.
- File system permissions for icon cache → use `ApplicationData.Current.LocalFolder`.
- Drag/drop edge cases → guard against large payloads; debounce.

## 14. Success metrics
- App launches and navigates reliably.
- Create 10 containers and 100 items without perf regressions.
- Theme persists across restart; icons import/assign works for common formats.

## 15. Milestones and timeline (Sprint 1 ~1–2 weeks)
- Day 1–2: Solution scaffolding, DI, MVVM, navigation shell.
- Day 3–4: Pages + controls placeholders; AppState; settings persistence.
- Day 5: Drag/drop prototype; icon import stub.
- Day 6: Tests, analyzers, a11y pass, packaging.

## 16. Detailed task breakdown (Sprint 1)
- **Solution setup**
  - [ ] Create solution and projects: `App/Core/Infrastructure/Tests`
  - [ ] Add NuGet: `CommunityToolkit.Mvvm`, `Microsoft.Extensions.Hosting`, `Microsoft.Extensions.Logging`, `CommunityToolkit.WinUI.UI`
  - [ ] Configure DI container in `App.xaml.cs`; register services/VMs
- **App shell & navigation**
  - [ ] Add `ShellPage` with `NavigationView` + `Frame`
  - [ ] Implement `INavigationService` and hook to VMs
  - [ ] Add menu items for Dashboard, Containers, Editor, Icons, Settings
- **Pages & controls**
  - [ ] `DashboardPage` + VM (previews stub)
  - [ ] `ContainersPage` + VM (list and New)
  - [ ] `EditorPage` + VM (canvas/grid placeholder)
  - [ ] `IconsPage` + VM (gallery stub)
  - [ ] `SettingsPage` + VM (theme, grid size, snap)
  - [ ] `ContainerPreviewControl`, `ItemTileControl`
- **Models & services (Core/Infrastructure)**
  - [ ] Define models: `Container`, `Item`, `IconAsset`, `Settings`
  - [ ] Define interfaces: `IContainerService`, `IItemService`, `IIconService`, `ISettingsService`, `IFileSystemService`, `IThemeService`
  - [ ] Implement in-memory repos/services
- **Settings & theming**
  - [ ] Implement `ISettingsService` with JSON in local folder
  - [ ] Theme switching wired to `Application.RequestedTheme`
- **Drag & drop**
  - [ ] Enable shell drop on `EditorPage`; map to `Item` creation
  - [ ] Internal drag visuals for `ItemTileControl` (basic)
- **Icons**
  - [ ] File picker to import icon images; copy to `icons/` cache
  - [ ] Assign icon to container/item (picker stub)
- **Testing & quality**
  - [ ] Unit tests: `SettingsService`, `NavigationService`, one ViewModel
  - [ ] Add analyzers or `.editorconfig`; zero warnings
  - [ ] Basic UI smoke test for navigation
- **Packaging**
  - [ ] MSIX packaging and app identity; ensure app runs after install

## 17. Acceptance criteria (Sprint 1)
- App launches packaged; navigation across all pages works.
- Create a container; visible in `ContainersPage` and preview on `DashboardPage`.
- Drop a file/folder onto `EditorPage` → visible `ItemTile`.
- Theme selection persists across restarts.
- Unit tests pass; no analyzer errors.

## 18. Dependencies & environment
- Windows 11, .NET 8, Windows App SDK 1.5+.
- Visual Studio Build Tools 2022 with Windows SDK and MSBuild.
- Optional: edit code in Cursor; build/package via MSBuild CLI.

## 19. Future backlog (post Sprint 1)
- SQLite repositories + migrations; search and tagging.
- Advanced layout: snap-to-grid, alignment, grouping, templates.
- Batch icon tooling; SVG rasterization; color theming.
- Export/import configuration; cloud sync.
- Shell integration, context menus, global hotkeys.
- Telemetry opt-in; diagnostics pane.

## 20. Open questions & assumptions
- Assumes local-only data in Sprint 1; no sync.
- Target audience starts with single-user personal workflow.
- Confirm preferred icon formats and sizes for import defaults.