#include "DesktopHost.h"
#include <dwmapi.h>  // For DWM composition APIs
#include <Shlwapi.h> // For Path functions
#include <stdio.h>
#include "resource.h" // For app constants

// TODO: Re-enable these when implementing DirectComposition/Direct2D visual effects
//#pragma comment(lib, "dcomp.lib")
//#pragma comment(lib, "d2d1.lib")
#pragma comment(lib, "dwmapi.lib")
#pragma comment(lib, "Shlwapi.lib")

DesktopHost::DesktopHost(HINSTANCE hInstance, HWND hwndParent)
    : m_hInstance(hInstance)
    , m_hwndParent(hwndParent)
    , m_hwndWorkerW(nullptr)
    , m_hwndShellDll(nullptr)
    // TODO: Initialize DirectComposition/Direct2D members when implementing visual effects
    // , m_dcompDevice(nullptr)
    // , m_dcompTarget(nullptr)
    // , m_rootVisual(nullptr)
    // , m_d2dFactory(nullptr)
    // , m_renderTarget(nullptr)
    // , m_brush(nullptr)
    , m_initialized(false)
{
    ZeroMemory(&m_desktopRect, sizeof(m_desktopRect));
}

DesktopHost::~DesktopHost()
{
    Shutdown();
}

bool DesktopHost::Initialize()
{
    if (m_initialized)
        return true;

    // Get desktop dimensions
    if (!GetWindowRect(GetDesktopWindow(), &m_desktopRect))
    {
        fprintf(stderr, "DesktopHost: Failed to get desktop rect\n");
        OutputDebugStringW(L"DesktopHost: Failed to get desktop rect\n");
        return false;
    }

    fprintf(stderr, "DesktopHost: Got desktop dimensions\n");
    OutputDebugStringW(L"DesktopHost: Got desktop dimensions\n");

    // Find and attach to WorkerW
    if (!FindWorkerW() || !AttachToWorkerW())
    {
        return false;
    }

    // TODO: Initialize DirectComposition and Direct2D for visual effects
    // For now, just create the basic desktop host window
    // InitializeDirectComposition();
    // InitializeDirect2D();
    // SetupVisualTree();

    m_initialized = true;
    return true;
}

void DesktopHost::Shutdown()
{
    if (!m_initialized)
        return;

    // TODO: Cleanup Direct2D and DirectComposition when implementing visual effects
    // CleanupDirect2D();
    // CleanupDirectComposition();

    if (m_hwndShellDll && IsWindow(m_hwndShellDll))
    {
        DestroyWindow(m_hwndShellDll);
    }

    m_initialized = false;
}

HWND DesktopHost::GetWorkerWindow()
{
    return m_hwndWorkerW;
}

bool DesktopHost::AttachToWorkerW()
{
    if (!m_hwndWorkerW || !IsWindow(m_hwndWorkerW))
    {
        fprintf(stderr, "DesktopHost: WorkerW window is invalid\n");
        OutputDebugStringW(L"DesktopHost: WorkerW window is invalid\n");
        return false;
    }

    fprintf(stderr, "DesktopHost: Creating child window in WorkerW...\n");
    OutputDebugStringW(L"DesktopHost: Creating child window in WorkerW...\n");

    // Try multiple approaches to create the desktop overlay window

    // Method 1: Try creating as a child window (original approach)
    m_hwndShellDll = CreateWindowExW(
        WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOPMOST | WS_EX_TOOLWINDOW,
        L"Static",
        L"BoxesDesktopHost",
        WS_POPUP | WS_VISIBLE,
        0, 0,
        m_desktopRect.right - m_desktopRect.left,
        m_desktopRect.bottom - m_desktopRect.top,
        nullptr,  // No parent - create as top-level window
        nullptr,
        m_hInstance,
        nullptr
    );

    if (m_hwndShellDll)
    {
        // Position it over the WorkerW window
        SetWindowPos(m_hwndShellDll, m_hwndWorkerW, 0, 0, 0, 0,
                     SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
    }

    if (!m_hwndShellDll)
    {
        // Method 2: Try creating with HWND_MESSAGE (message-only window)
        m_hwndShellDll = CreateWindowExW(
            WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOPMOST | WS_EX_TOOLWINDOW,
            L"Static",
            L"BoxesDesktopHost",
            WS_POPUP | WS_VISIBLE,
            0, 0, 1, 1,  // Small size initially
            HWND_MESSAGE,  // Message-only window
            nullptr,
            m_hInstance,
            nullptr
        );

        if (m_hwndShellDll)
        {
            // Resize and position over desktop
            SetWindowPos(m_hwndShellDll, nullptr,
                         0, 0,
                         m_desktopRect.right - m_desktopRect.left,
                         m_desktopRect.bottom - m_desktopRect.top,
                         SWP_NOZORDER | SWP_NOACTIVATE);
        }
    }

    if (!m_hwndShellDll)
    {
        // Method 3: Try creating with minimal styles
        m_hwndShellDll = CreateWindowExW(
            WS_EX_TOPMOST,
            L"Static",
            L"BoxesDesktopHost",
            WS_POPUP,
            0, 0,
            m_desktopRect.right - m_desktopRect.left,
            m_desktopRect.bottom - m_desktopRect.top,
            nullptr,
            nullptr,
            m_hInstance,
            nullptr
        );
    }

    if (!m_hwndShellDll)
    {
        fprintf(stderr, "DesktopHost: Failed to create overlay window with all methods\n");
        OutputDebugStringW(L"DesktopHost: Failed to create overlay window with all methods\n");
        return false;
    }

    fprintf(stderr, "DesktopHost: Overlay window created successfully\n");
    OutputDebugStringW(L"DesktopHost: Overlay window created successfully\n");

    // Make it transparent and topmost
    if (!SetLayeredWindowAttributes(m_hwndShellDll, 0, 0, LWA_ALPHA))
    {
        fprintf(stderr, "DesktopHost: Warning - failed to set layered window attributes\n");
        OutputDebugStringW(L"DesktopHost: Warning - failed to set layered window attributes\n");
        // Don't return false - window might still work
    }

    fprintf(stderr, "DesktopHost: Overlay window configured\n");
    OutputDebugStringW(L"DesktopHost: Overlay window configured\n");
    return true;
}

// TODO: Implement DirectComposition initialization for visual effects
/*
bool DesktopHost::InitializeDirectComposition()
{
    HRESULT hr;

    // Create DirectComposition device
    hr = DCompositionCreateDevice(nullptr, IID_PPV_ARGS(&m_dcompDevice));
    if (FAILED(hr))
    {
        return false;
    }

    // Create composition target for our window
    hr = m_dcompDevice->CreateTargetForHwnd(m_hwndShellDll, TRUE, &m_dcompTarget);
    if (FAILED(hr))
    {
        m_dcompDevice->Release();
        m_dcompDevice = nullptr;
        return false;
    }

    // Create root visual
    hr = m_dcompDevice->CreateVisual(IID_PPV_ARGS(&m_rootVisual));
    if (FAILED(hr))
    {
        m_dcompTarget->Release();
        m_dcompTarget = nullptr;
        m_dcompDevice->Release();
        m_dcompDevice = nullptr;
        return false;
    }

    // Set root visual as target's root
    hr = m_dcompTarget->SetRoot(m_rootVisual);
    if (FAILED(hr))
    {
        m_rootVisual->Release();
        m_rootVisual = nullptr;
        m_dcompTarget->Release();
        m_dcompTarget = nullptr;
        m_dcompDevice->Release();
        m_dcompDevice = nullptr;
        return false;
    }

    // Commit the composition
    hr = m_dcompDevice->Commit();
    if (FAILED(hr))
    {
        return false;
    }

    return true;
}
*/

// TODO: Implement visual tree setup for DirectComposition effects
/*
void DesktopHost::SetupVisualTree()
{
    // For now, just set up a basic visual tree
    // Later we'll add blur effects, tint layers, etc.

    if (!m_rootVisual || !m_renderTarget)
        return;

    // Set the render target as content for the root visual
    // Note: Direct2D render targets need to be handled differently with DirectComposition
    // For now, we'll set to nullptr and handle rendering separately
    m_rootVisual->SetContent(nullptr);

    // Commit changes
    m_dcompDevice->Commit();
}
*/

// TODO: Implement rendering methods for visual effects
/*
void DesktopHost::Render()
{
    if (!m_renderTarget || !m_brush)
        return;

    // Begin drawing
    m_renderTarget->BeginDraw();

    // Clear with a semi-transparent background (for demo)
    m_renderTarget->Clear(D2D1::ColorF(0.0f, 0.5f, 1.0f, 0.1f)); // Light blue, 10% opacity

    // Draw a simple rectangle (demo)
    D2D1_RECT_F rect = D2D1::RectF(100.0f, 100.0f, 300.0f, 200.0f);
    m_renderTarget->FillRectangle(rect, m_brush);

    // End drawing
    HRESULT hr = m_renderTarget->EndDraw();
    if (FAILED(hr))
    {
        // Handle error (device lost, etc.)
    }

    // Commit to composition
    m_dcompDevice->Commit();
}

void DesktopHost::Resize(UINT width, UINT height)
{
    if (m_renderTarget)
    {
        m_renderTarget->Resize(D2D1::SizeU(width, height));
    }
}
*/

bool DesktopHost::FindWorkerW()
{
    // Find the WorkerW window (desktop background window)
    // This is the window that hosts the desktop icons

    fprintf(stderr, "DesktopHost: Finding WorkerW window...\n");
    OutputDebugStringW(L"DesktopHost: Finding WorkerW window...\n");

    // Method 1: Try to find Progman and send message to create WorkerW
    HWND hProgman = FindWindowW(L"Progman", nullptr);
    if (hProgman)
    {
        fprintf(stderr, "DesktopHost: Found Progman window\n");
        OutputDebugStringW(L"DesktopHost: Found Progman window\n");
        // Send message to Progman to ensure WorkerW exists
        SendMessageTimeout(hProgman, 0x052C, 0, 0, SMTO_NORMAL, 1000, nullptr);

        // Find WorkerW window
        m_hwndWorkerW = FindWindowExW(nullptr, nullptr, L"WorkerW", nullptr);
        if (m_hwndWorkerW)
        {
            fprintf(stderr, "DesktopHost: Found WorkerW window via Progman\n");
            OutputDebugStringW(L"DesktopHost: Found WorkerW window via Progman\n");
            return true;
        }
        fprintf(stderr, "DesktopHost: WorkerW not found via Progman\n");
        OutputDebugStringW(L"DesktopHost: WorkerW not found via Progman\n");
    }
    else
    {
        fprintf(stderr, "DesktopHost: Progman window not found\n");
        OutputDebugStringW(L"DesktopHost: Progman window not found\n");
    }

    // Method 2: Alternative approach - look for SHELLDLL_DefView
    fprintf(stderr, "DesktopHost: Looking for SHELLDLL_DefView...\n");
    OutputDebugStringW(L"DesktopHost: Looking for SHELLDLL_DefView...\n");
    m_hwndWorkerW = FindWindowExW(nullptr, nullptr, L"SHELLDLL_DefView", nullptr);
    if (m_hwndWorkerW)
    {
        fprintf(stderr, "DesktopHost: Found SHELLDLL_DefView window\n");
        OutputDebugStringW(L"DesktopHost: Found SHELLDLL_DefView window\n");
        return true;
    }
    fprintf(stderr, "DesktopHost: SHELLDLL_DefView not found\n");
    OutputDebugStringW(L"DesktopHost: SHELLDLL_DefView not found\n");

    // Method 3: Fallback to desktop window itself
    fprintf(stderr, "DesktopHost: Using desktop window as fallback...\n");
    OutputDebugStringW(L"DesktopHost: Using desktop window as fallback...\n");
    m_hwndWorkerW = GetDesktopWindow();
    if (m_hwndWorkerW)
    {
        fprintf(stderr, "DesktopHost: Using desktop window as WorkerW\n");
        OutputDebugStringW(L"DesktopHost: Using desktop window as WorkerW\n");
        return true;
    }

    fprintf(stderr, "DesktopHost: Failed to find any desktop window\n");
    OutputDebugStringW(L"DesktopHost: Failed to find any desktop window\n");
    return false;
}

// TODO: Implement Direct2D initialization for visual effects
/*
bool DesktopHost::InitializeDirect2D()
{
    if (!m_hwndShellDll)
        return false;

    HRESULT hr;

    // Create D2D factory
    hr = D2D1CreateFactory(D2D1_FACTORY_TYPE_SINGLE_THREADED, &m_d2dFactory);
    if (FAILED(hr))
    {
        return false;
    }

    // Create render target
    RECT rc;
    GetClientRect(m_hwndShellDll, &rc);

    D2D1_SIZE_U size = D2D1::SizeU(rc.right - rc.left, rc.bottom - rc.top);
    D2D1_RENDER_TARGET_PROPERTIES rtProps = D2D1::RenderTargetProperties();
    D2D1_HWND_RENDER_TARGET_PROPERTIES hwndProps = D2D1::HwndRenderTargetProperties(m_hwndShellDll, size);

    hr = m_d2dFactory->CreateHwndRenderTarget(rtProps, hwndProps, &m_renderTarget);
    if (FAILED(hr))
    {
        m_d2dFactory->Release();
        m_d2dFactory = nullptr;
        return false;
    }

    // Create a brush
    hr = m_renderTarget->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::White), &m_brush);
    if (FAILED(hr))
    {
        m_renderTarget->Release();
        m_renderTarget = nullptr;
        m_d2dFactory->Release();
        m_d2dFactory = nullptr;
        return false;
    }

    return true;
}
*/

// TODO: Implement cleanup methods for DirectComposition/Direct2D
/*
void DesktopHost::CleanupDirectComposition()
{
    if (m_rootVisual)
    {
        m_rootVisual->Release();
        m_rootVisual = nullptr;
    }

    if (m_dcompTarget)
    {
        m_dcompTarget->Release();
        m_dcompTarget = nullptr;
    }

    if (m_dcompDevice)
    {
        m_dcompDevice->Release();
        m_dcompDevice = nullptr;
    }
}

void DesktopHost::CleanupDirect2D()
{
    if (m_brush)
    {
        m_brush->Release();
        m_brush = nullptr;
    }

    if (m_renderTarget)
    {
        m_renderTarget->Release();
        m_renderTarget = nullptr;
    }

    if (m_d2dFactory)
    {
        m_d2dFactory->Release();
        m_d2dFactory = nullptr;
    }
}
*/
