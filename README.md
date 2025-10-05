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

### **Boxes.Avalonia** (C# Avalonia UI)
- **Purpose**: Desktop boxes and settings interface
- **Technology**: .NET 8 with Avalonia UI 11.3
- **Responsibilities**:
  - Beautiful glass-like desktop containers
  - Settings and preferences UI
  - Drag-and-drop window functionality
  - Cross-process communication with host

## 🛠️ Technology Stack

| Component | Technology | Purpose |
|-----------|------------|---------|
| **Desktop Host** | C++ Win32 API | Native desktop integration, tray icon |
| **Desktop Boxes** | C# + Avalonia UI 11.3 | Beautiful glass-like desktop containers |
| **Communication** | Named Pipes | Inter-process communication between host and UI |
| **Visual Effects** | Avalonia Rendering | Hardware-accelerated glass effects |
| **Build System** | MSBuild + Visual Studio | Cross-platform development |

## 📁 Project Structure

```
Desktop-Organizer/
├── Boxes/                          # Main solution directory
│   ├── Boxes.Avalonia/            # Avalonia UI Desktop Boxes
│   │   ├── App.xaml/cs            # Application entry point
│   │   ├── Program.cs             # Main application startup
│   │   ├── Views/
│   │   │   └── DesktopBox.axaml/cs # Desktop container window
│   │   ├── Models/                # Data models (Box, FileItem, etc.)
│   │   ├── Services/              # Business logic (GlassEffect, etc.)
│   │   └── Boxes.Avalonia.csproj  # Avalonia project file
│   │
│   ├── Boxes.Host/                # C++ Win32 Desktop Host
│   │   ├── main.cpp               # Application entry point
│   │   ├── TrayIcon.h/cpp         # System tray management
│   │   ├── DesktopHost.h/cpp      # WorkerW window integration
│   │   └── Boxes.Host.vcxproj     # C++ project file
│   │
│   ├── Boxes/                     # Legacy WinUI 3 (preserved)
│   │   └── ...                    # Original WinUI implementation
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
- **C++ Win32 Host**: Tray icon, desktop window integration, process launching
- **Avalonia Desktop Boxes**: Beautiful glass-like desktop containers with drag functionality
- **Cross-Process Communication**: Host launches Avalonia desktop boxes successfully
- **Visual Effects**: Semi-transparent backgrounds with blur effects using Avalonia
- **Project Architecture**: Clean separation between native host and managed UI

### 🚧 **In Development**
- **Icon Organization**: Desktop file scanning and grouping logic
- **Template System**: Pre-defined organization patterns and layouts
- **Rule Engine**: Intelligent file categorization and placement

### 🔮 **Future Enhancements**
- **Advanced Visual Effects**: Enhanced glass effects, animations, and transitions
- **Plugin Architecture**: Extensible organization methods
- **Multi-Monitor Support**: Desktop boxes across multiple displays
- **Cloud Sync**: Settings synchronization across devices

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
- .NET 8 SDK
- Avalonia UI templates (optional, for project creation)

### **Building**
1. Open `Boxes/Boxes.slnx` in Visual Studio
2. Set `Boxes.Host` as startup project for tray icon testing
3. Build solution (F6) or run individual projects (F5)

### **Testing Desktop Boxes**
1. **Through Visual Studio**: Run `Boxes.Avalonia` project directly
2. **Through Tray Icon**: Run `Boxes.Host` and use "Create Box" from tray menu
3. **Command Line**: `dotnet run --project Boxes/Boxes.Avalonia/Boxes.Avalonia.csproj`

### **Architecture Benefits**
- **Native Performance**: C++ host for optimal desktop integration
- **Modern UX**: Avalonia UI for beautiful, responsive interface
- **Cross-Platform Ready**: Avalonia enables future macOS/Linux support
- **Maintainability**: Clean separation between native host and managed UI

## 📝 Notes

- **Avalonia Migration**: Successfully migrated from WinUI 3 to Avalonia UI for better stability and cross-platform compatibility
- **Visual Effects**: Implemented using Avalonia's rendering system with semi-transparent backgrounds and blur effects
- **Project Architecture**: Native C++ host with managed Avalonia UI for optimal performance and modern UX
- **Current Focus**: Desktop container functionality is working - file organization features in development