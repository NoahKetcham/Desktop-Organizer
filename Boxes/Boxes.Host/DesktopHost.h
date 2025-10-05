#pragma once

#include <Windows.h>
#include <dcomp.h>  // DirectComposition
#include <d2d1.h>   // Direct2D
#include <wincodec.h> // For WIC imaging if needed

class DesktopHost
{
public:
    DesktopHost(HINSTANCE hInstance, HWND hwndParent);
    ~DesktopHost();

    bool Initialize();
    void Shutdown();

    // WorkerW window management
    HWND GetWorkerWindow();
    bool AttachToWorkerW();

    // TODO: DirectComposition and Direct2D methods for future visual effects
    // bool InitializeDirectComposition();
    // bool InitializeDirect2D();
    // void SetupVisualTree();
    // void Render();
    // void Resize(UINT width, UINT height);

private:
    // WorkerW related
    HINSTANCE m_hInstance;
    HWND m_hwndParent;
    HWND m_hwndWorkerW;
    HWND m_hwndShellDll;

    // TODO: DirectComposition and Direct2D members for future visual effects
    // IDCompositionDesktopDevice* m_dcompDevice;
    // IDCompositionTarget* m_dcompTarget;
    // IDCompositionVisual* m_rootVisual;
    // ID2D1Factory* m_d2dFactory;
    // ID2D1HwndRenderTarget* m_renderTarget;
    // ID2D1SolidColorBrush* m_brush;

    // State
    bool m_initialized;
    RECT m_desktopRect;

    // Helper methods
    bool FindWorkerW();
    // TODO: Cleanup methods for future DirectComposition/Direct2D use
    // void CleanupDirectComposition();
    // void CleanupDirect2D();
};
