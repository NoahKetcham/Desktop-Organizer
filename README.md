# Desktop-Organizer

A **Fences-like desktop organizer** that creates translucent, customizable panels directly on the Windows desktop background, providing an intuitive way to organize and group desktop icons and files.

## 🏗️ Architecture

This project uses a **dual-project architecture** for optimal desktop integration:

### **Boxes.Host** (C++ Win32)
- **Purpose**: Native desktop host and tray application
- **Technology**: C++ Win32 API with DirectComposition support
- **Responsibilities**:
  - Creates transparent windows attached to desktop WorkerW
  - Manages system tray icon and context menu
  - Handles low-level desktop integration
  - Launches the settings UI when requested

### **Boxes** (C# WinUI 3)
- **Purpose**: Settings UI and configuration interface
- **Technology**: .NET 8 with WinUI 3
- **Responsibilities**:
  - User-friendly settings and preferences UI
  - Template and rule management
  - Organization method configuration
  - Cross-process communication with host

## 🛠️ Technology Stack

| Component | Technology | Purpose |
|-----------|------------|---------|
| **Desktop Host** | C++ Win32 API | Native desktop integration, tray icon |
| **Settings UI** | C# + WinUI 3 | Modern, responsive user interface |
| **Communication** | Named Pipes | Inter-process communication between host and UI |
| **Desktop Integration** | DirectComposition | Hardware-accelerated visual effects (planned) |
| **Build System** | MSBuild + Visual Studio | Native Windows development |

## 📁 Project Structure

```
Desktop-Organizer/
├── Boxes/                          # Main solution directory
│   ├── Boxes/                      # WinUI 3 C# Settings Application
│   │   ├── App.xaml/cs            # Application entry point
│   │   ├── MainWindow.xaml/cs     # Main settings window
│   │   ├── Pages/                 # Settings pages
│   │   │   ├── OverviewPage.xaml/cs
│   │   │   ├── TemplatesPage.xaml/cs
│   │   │   ├── OrganizationMethodsPage.xaml/cs
│   │   │   └── DesktopFilesPage.xaml/cs
│   │   ├── Models/                # Data models
│   │   ├── Services/              # Business logic services
│   │   ├── ViewModels/            # MVVM view models
│   │   └── Controls/              # Custom UI controls
│   │
│   ├── Boxes.Host/                # C++ Win32 Desktop Host
│   │   ├── main.cpp               # Application entry point
│   │   ├── TrayIcon.h/cpp         # System tray management
│   │   ├── DesktopHost.h/cpp      # WorkerW window integration
│   │   └── Boxes.Host.vcxproj     # C++ project file
│   │
│   └── Boxes (Package)/           # MSIX Packaging Project
│       ├── Package.appxmanifest   # App manifest
│       └── Images/                # Package assets
│
└── Docs/                          # Documentation
    ├── IdeaBoard.md               # Initial concept and brainstorming
    └── prd_*.md                   # Product requirement documents
```

## 🚀 Current Status

### ✅ **Implemented**
- **C++ Win32 Host**: Tray icon, desktop window integration, basic WorkerW attachment
- **WinUI 3 Settings UI**: Complete settings interface with navigation
- **Project Structure**: Organized dual-project architecture
- **Build System**: Working compilation and project references

### 🚧 **In Development**
- **Visual Effects**: DirectComposition-based translucent panels (commented out due to SDK compatibility)
- **Icon Organization**: Desktop file scanning and grouping logic
- **Inter-process Communication**: Named pipes for host↔UI communication

### 🔮 **Future Enhancements**
- **Hardware-accelerated Effects**: Gaussian blur, tint layers, noise textures for "liquid glass" appearance
- **Template System**: Pre-defined organization patterns and layouts
- **Rule Engine**: Intelligent file categorization and placement
- **Animation System**: Smooth transitions and visual feedback
- **Plugin Architecture**: Extensible organization methods

## 🎯 Key Features

### **Desktop Integration**
- Native Win32 application running at desktop level
- Transparent windows that blend with wallpaper
- Always-accessible tray icon interface
- Seamless integration with Windows shell

### **User Experience**
- Modern, responsive settings interface
- Intuitive drag-and-drop organization
- Real-time preview of changes
- Customizable visual themes and effects

### **Performance**
- Minimal resource usage when idle
- Hardware-accelerated rendering (planned)
- Efficient background processing
- Optimized for battery life on laptops

## 🛠️ Development Setup

### **Prerequisites**
- Windows 10/11 (10.0.19041.0 or later)
- Visual Studio 2022 with C++ and .NET workloads
- Windows SDK 10.0.19041.0 or compatible version

### **Building**
1. Open `Boxes/Boxes.slnx` in Visual Studio
2. Set `Boxes.Host` as startup project
3. Build solution (F6) or run (F5)

### **Architecture Benefits**
- **Native Performance**: C++ host for optimal desktop integration
- **Modern UX**: WinUI 3 for polished settings interface
- **Maintainability**: Clear separation of concerns
- **Extensibility**: Easy to add new organization features

## 📝 Notes

- DirectComposition visual effects are currently disabled due to Windows SDK 10.0.26100.0 compatibility issues
- All visual effects code is preserved and can be re-enabled when SDK compatibility is resolved
- Project follows modern Windows development best practices with native C++ and managed C# components