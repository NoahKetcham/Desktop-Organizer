#include "TrayIcon.h"
#include <shellapi.h>
#include "resource.h"

TrayIcon::TrayIcon(HINSTANCE hInstance, HWND hwndParent, UINT iconId)
    : m_hInstance(hInstance)
    , m_hwndParent(hwndParent)
    , m_iconId(iconId)
    , m_initialized(false)
{
    ZeroMemory(&m_nid, sizeof(m_nid));
    m_nid.cbSize = sizeof(NOTIFYICONDATAW);
    m_nid.hWnd = hwndParent;
    m_nid.uID = 1;
    m_nid.uFlags = NIF_ICON | NIF_MESSAGE | NIF_TIP;
    m_nid.uCallbackMessage = WM_USER + 1;
    m_nid.hIcon = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_BOXESHOST));

    // Set tooltip
    wcscpy_s(m_nid.szTip, L"Boxes Desktop Organizer");
}

TrayIcon::~TrayIcon()
{
    if (m_initialized)
    {
        Shell_NotifyIconW(NIM_DELETE, &m_nid);
    }
    if (m_nid.hIcon)
    {
        DestroyIcon(m_nid.hIcon);
    }
}

bool TrayIcon::Initialize()
{
    if (m_initialized)
        return true;

    m_initialized = Shell_NotifyIconW(NIM_ADD, &m_nid) != FALSE;
    return m_initialized;
}

void TrayIcon::Show()
{
    if (!m_initialized)
        return;

    m_nid.uFlags = NIF_ICON | NIF_MESSAGE | NIF_TIP;
    Shell_NotifyIconW(NIM_MODIFY, &m_nid);
}

void TrayIcon::Hide()
{
    if (!m_initialized)
        return;

    m_nid.uFlags = NIF_ICON | NIF_MESSAGE | NIF_TIP;
    Shell_NotifyIconW(NIM_DELETE, &m_nid);
    m_initialized = false;
}

void TrayIcon::UpdateTooltip(const wchar_t* tooltip)
{
    if (!m_initialized || !tooltip)
        return;

    wcscpy_s(m_nid.szTip, tooltip);
    m_nid.uFlags = NIF_TIP;
    Shell_NotifyIconW(NIM_MODIFY, &m_nid);
}
