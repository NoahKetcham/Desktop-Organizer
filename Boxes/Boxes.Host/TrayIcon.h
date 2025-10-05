#pragma once

#include <Windows.h>

class TrayIcon
{
public:
    TrayIcon(HINSTANCE hInstance, HWND hwndParent, UINT iconId);
    ~TrayIcon();

    bool Initialize();
    void Show();
    void Hide();
    void UpdateTooltip(const wchar_t* tooltip);

private:
    HINSTANCE m_hInstance;
    HWND m_hwndParent;
    UINT m_iconId;
    NOTIFYICONDATAW m_nid;
    bool m_initialized;
};
