#include <Windows.h>
#include <shellapi.h>
#include <string>
#include <stdio.h>
#include <shlwapi.h>
#include "TrayIcon.h"
#include "DesktopHost.h"
#include "resource.h"

// Forward declarations
LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
void ShowContextMenu(HWND hwnd, POINT pt);

// Global instances
TrayIcon* g_trayIcon = nullptr;
DesktopHost* g_desktopHost = nullptr;
HWND g_hwnd = nullptr; // Hidden main window

int WINAPI wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nShowCmd)
{
    // Debug: Application starting
    fprintf(stderr, "Boxes.Host starting...\n");
    OutputDebugStringW(L"Boxes.Host starting...\n");

    // Register window class
    WNDCLASSEXW wcex = {};
    wcex.cbSize = sizeof(WNDCLASSEX);
    wcex.style = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc = WindowProc;
    wcex.hInstance = hInstance;
    wcex.lpszClassName = L"BoxesHostWindow";
    wcex.hIcon = LoadIconW(hInstance, MAKEINTRESOURCEW(IDI_BOXESHOST));
    wcex.hCursor = LoadCursor(nullptr, IDC_ARROW);

    if (!RegisterClassExW(&wcex))
    {
        fprintf(stderr, "Failed to register window class\n");
        OutputDebugStringW(L"Failed to register window class\n");
        return 1;
    }

    fprintf(stderr, "Window class registered successfully\n");
    OutputDebugStringW(L"Window class registered successfully\n");

    // Create hidden main window (needed for tray icon and message handling)
    g_hwnd = CreateWindowExW(
        0,                              // Optional window styles
        L"BoxesHostWindow",             // Window class
        L"Boxes Host",                  // Window text
        WS_OVERLAPPEDWINDOW,            // Window style
        CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, // Size and position
        nullptr,                        // Parent window
        nullptr,                        // Menu
        hInstance,                      // Instance handle
        nullptr                         // Additional application data
    );

    if (!g_hwnd)
    {
        fprintf(stderr, "Failed to create main window\n");
        OutputDebugStringW(L"Failed to create main window\n");
        return 1;
    }

    fprintf(stderr, "Main window created successfully\n");
    OutputDebugStringW(L"Main window created successfully\n");

    // Initialize tray icon
    g_trayIcon = new TrayIcon(hInstance, g_hwnd, IDI_BOXESHOST);
    if (!g_trayIcon->Initialize())
    {
        fprintf(stderr, "Failed to initialize tray icon\n");
        OutputDebugStringW(L"Failed to initialize tray icon\n");
        delete g_trayIcon;
        DestroyWindow(g_hwnd);
        return 1;
    }

    fprintf(stderr, "Tray icon initialized successfully\n");
    OutputDebugStringW(L"Tray icon initialized successfully\n");

    // Initialize desktop host (WorkerW window and DirectComposition)
    fprintf(stderr, "Initializing DesktopHost...\n");
    OutputDebugStringW(L"Initializing DesktopHost...\n");
    g_desktopHost = new DesktopHost(hInstance, g_hwnd);
    if (!g_desktopHost->Initialize())
    {
        // Debug: Check what went wrong
        HWND testWorkerW = g_desktopHost->GetWorkerWindow();
        if (!testWorkerW) {
            fprintf(stderr, "DesktopHost failed: WorkerW window not found\n");
            OutputDebugStringW(L"DesktopHost failed: WorkerW window not found\n");
        } else {
            fprintf(stderr, "DesktopHost failed: WorkerW found but initialization failed\n");
            OutputDebugStringW(L"DesktopHost failed: WorkerW found but initialization failed\n");
        }

        delete g_desktopHost;
        delete g_trayIcon;
        DestroyWindow(g_hwnd);
        return 1;
    }

    fprintf(stderr, "DesktopHost initialized successfully\n");
    OutputDebugStringW(L"DesktopHost initialized successfully\n");

    // Show tray icon
    g_trayIcon->Show();

    // Debug: Application started successfully
    fprintf(stderr, "Boxes.Host started successfully - tray icon should be visible\n");
    OutputDebugStringW(L"Boxes.Host started successfully - tray icon should be visible\n");

    // Main message loop
    MSG msg = {};
    while (GetMessage(&msg, nullptr, 0, 0))
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    // Cleanup
    if (g_desktopHost)
    {
        g_desktopHost->Shutdown();
        delete g_desktopHost;
    }

    if (g_trayIcon)
    {
        g_trayIcon->Hide();
        delete g_trayIcon;
    }

    DestroyWindow(g_hwnd);
    return static_cast<int>(msg.wParam);
}

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    switch (uMsg)
    {
    case WM_USER + 1:
        OutputDebugStringW(L"Tray icon message received\n");
        fprintf(stderr, "Tray icon message received: wParam=%d, lParam=%d\n", (int)wParam, (int)lParam);
        if (lParam == WM_RBUTTONUP || lParam == WM_LBUTTONUP)
        {
            OutputDebugStringW(L"Tray icon clicked - showing context menu\n");
            fprintf(stderr, "Tray icon clicked - showing context menu\n");
            POINT pt;
            GetCursorPos(&pt);
            ShowContextMenu(hwnd, pt);
        }
        break;

    case WM_COMMAND:
        switch (LOWORD(wParam))
        {
        case ID_TRAY_OPEN_SETTINGS:
            // Launch the WinUI 3 settings app
            OutputDebugStringW(L"TRAY ICON CLICKED: Open Settings\n");
            fprintf(stderr, "TRAY ICON CLICKED: Open Settings\n");

            // Launch the WinUI 3 settings app
            {
                OutputDebugStringW(L"Attempting to launch settings app\n");

                WCHAR hostPath[MAX_PATH];
                WCHAR nonPackagedPath[MAX_PATH];

                // Use absolute path for reliability
                wcscpy_s(nonPackagedPath, L"C:\\Users\\noahk\\OneDrive\\Documents\\GitHub\\Desktop-Organizer\\Boxes\\Boxes\\bin\\x64\\Debug\\net8.0-windows10.0.19041.0\\Boxes.exe");

                OutputDebugStringW(L"Trying to launch non-packaged settings app...");

                if (PathFileExistsW(nonPackagedPath)) {
                    OutputDebugStringW(L"Non-packaged executable found, launching...");
                    HINSTANCE result = ShellExecuteW(nullptr, L"open", nonPackagedPath, nullptr, nullptr, SW_SHOWNORMAL);
                    if ((INT_PTR)result <= 32) {
                        WCHAR errorMsg[256];
                        swprintf_s(errorMsg, L"Settings app launch failed with error code: %lld", (INT_PTR)result);
                        OutputDebugStringW(errorMsg);
                        MessageBoxW(nullptr, L"Failed to launch settings app. Please check if Windows App SDK runtime is installed.", L"Error", MB_OK | MB_ICONERROR);
                    } else {
                        OutputDebugStringW(L"Settings app launch succeeded");
                    }
                } else {
                    OutputDebugStringW(L"Non-packaged executable not found");
                    MessageBoxW(nullptr, L"Settings app executable not found. Please rebuild the project.", L"Error", MB_OK | MB_ICONERROR);
                }
            }
            break;

    case ID_TRAY_CREATE_BOX:
        // Create a new desktop box
        OutputDebugStringW(L"TRAY ICON CLICKED: Create Box\n");
        fprintf(stderr, "TRAY ICON CLICKED: Create Box\n");
        
        // Launch the BoxLauncher to create an actual desktop box
        {
            WCHAR hostPath[MAX_PATH];
            WCHAR boxLauncherPath[MAX_PATH];

            // Use absolute path for reliability - Launch Avalonia app instead
            wcscpy_s(boxLauncherPath, L"C:\\Users\\noahk\\OneDrive\\Documents\\GitHub\\Desktop-Organizer\\Boxes\\Boxes.Avalonia\\bin\\x64\\Debug\\net8.0\\Boxes.Avalonia.exe");

            OutputDebugStringW(L"Launching desktop box...");

            if (PathFileExistsW(boxLauncherPath)) {
                OutputDebugStringW(L"BoxLauncher found, launching...");
                HINSTANCE result = ShellExecuteW(nullptr, L"open", boxLauncherPath, nullptr, nullptr, SW_SHOWNORMAL);
                if ((INT_PTR)result <= 32) {
                    WCHAR errorMsg[256];
                    swprintf_s(errorMsg, L"Desktop box launch failed with error code: %lld", (INT_PTR)result);
                    OutputDebugStringW(errorMsg);
                    MessageBoxW(nullptr, L"Failed to launch desktop box. The BoxLauncher may need to be built first.", L"Error", MB_OK | MB_ICONERROR);
                } else {
                    OutputDebugStringW(L"Desktop box launch succeeded");
                }
            } else {
                OutputDebugStringW(L"BoxLauncher not found");
                MessageBoxW(nullptr, L"Desktop box launcher not found. Please build the project first.", L"Error", MB_OK | MB_ICONERROR);
            }
        }
        break;

        case ID_TRAY_EXIT:
            PostQuitMessage(0);
            break;
        }
        break;

    case WM_DESTROY:
        PostQuitMessage(0);
        break;

    default:
        return DefWindowProc(hwnd, uMsg, wParam, lParam);
    }
    return 0;
}

void ShowContextMenu(HWND hwnd, POINT pt)
{
    OutputDebugStringW(L"ShowContextMenu called\n");
    fprintf(stderr, "ShowContextMenu called at (%d, %d)\n", pt.x, pt.y);
    
    HMENU hMenu = CreatePopupMenu();
    if (!hMenu) {
        OutputDebugStringW(L"Failed to create popup menu\n");
        fprintf(stderr, "Failed to create popup menu\n");
        return;
    }

    InsertMenuW(hMenu, 0, MF_BYPOSITION | MF_STRING, ID_TRAY_OPEN_SETTINGS, L"Open Settings");
    InsertMenuW(hMenu, 1, MF_BYPOSITION | MF_STRING, ID_TRAY_CREATE_BOX, L"Create Box");
    InsertMenuW(hMenu, 2, MF_BYPOSITION | MF_STRING, ID_TRAY_EXIT, L"Exit");

    SetForegroundWindow(hwnd);
    TrackPopupMenu(hMenu, TPM_BOTTOMALIGN | TPM_LEFTALIGN, pt.x, pt.y, 0, hwnd, nullptr);
    DestroyMenu(hMenu);
    
    OutputDebugStringW(L"Context menu displayed\n");
    fprintf(stderr, "Context menu displayed\n");
}
