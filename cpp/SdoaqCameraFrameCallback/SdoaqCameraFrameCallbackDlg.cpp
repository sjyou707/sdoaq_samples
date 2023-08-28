
// SdoaqCameraFrameCallbackDlg.cpp : implementation file
//

#include "pch.h"
#include "framework.h"
#include "SdoaqCameraFrameCallback.h"
#include "SdoaqCameraFrameCallbackDlg.h"
#include "afxdialogex.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

//============================================================================
// SDOAQ , WSIO LIBRARY & HEADER
//----------------------------------------------------------------------------
#if defined(_DEBUG)
#pragma comment(lib, "../../Include/SDOAQ/SDOAQd.lib")
#pragma comment(lib, "../../Include/WSIO/WSIODLL_D64.lib")
#else
#pragma comment(lib, "../../Include/SDOAQ/SDOAQ.lib")
#pragma comment(lib, "../../Include/WSIO/WSIODLL_R64.lib")
#endif
#include "..\..\Include\SDOAQ\SDOAQ_WS.h"
#include "..\..\Include\SDOAQ\SDOAQ_LLAPI.h"
#include "..\..\Include\WSIO\WSIO_UTIL.h"


//----------------------------------------------------------------------------
static WSIOVOID g_hViewer = NULL;
//----------------------------------------------------------------------------
static void g_SDOAQ_InitDoneCallback(eErrorCode errorCode, char* pErrorMessage);
static void g_SDOAQ_FrameCallback(eErrorCode errorCode, unsigned char* pBuffer, size_t BufferSize, FrameDescriptor* pFrameDescriptor);
static void g_SDOAQ_SetCameraTriggerMode(eCameraTriggerMode ctm);
//----------------------------------------------------------------------------
static void g_LogLine(LPCTSTR sFormat, ...)
{
	static CString g_sLog;
	va_list args; va_start(args, sFormat);
	CString add_log; add_log.FormatV(sFormat, args);
	g_sLog += add_log + _T("\r\n");
	if (g_sLog.GetLength() >= 1400 * 40) { g_sLog = g_sLog.Right(1000 * 40); }
	if (theApp.m_pMainWnd)
	{
		CEdit* p_wnd = (CEdit*)theApp.m_pMainWnd->GetDlgItem(IDC_LOG);
		if (p_wnd)
		{
			p_wnd->SetWindowText(g_sLog);
			//theApp.m_pMainWnd->PostMessage(WM_VSCROLL, SB_BOTTOM);
		}
	}
}
//============================================================================

CSdoaqCameraFrameCallbackDlg::CSdoaqCameraFrameCallbackDlg(CWnd* pParent /*=nullptr*/)
	: CDialogEx(IDD_SDOAQCAMERAFRAMECALLBACK_DIALOG, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CSdoaqCameraFrameCallbackDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

//----------------------------------------------------------------------------
BEGIN_MESSAGE_MAP(CSdoaqCameraFrameCallbackDlg, CDialogEx)
	ON_WM_CLOSE()
	ON_WM_SIZE()
	ON_COMMAND(IDC_CHECK_FRAME_CALLBACK, &OnBnClickedCheckFrameCallback)
	ON_COMMAND(IDC_SW_TRIGGER, &OnBnClickedSwTrigger)
	ON_COMMAND(IDC_TRIGGER_FREERUN, &OnBnClickedTriggerFreerun)
	ON_COMMAND(IDC_TRIGGER_SOFTWARE, &OnBnClickedTriggerSoftware)
	ON_COMMAND(IDC_TRIGGER_EXTERNAL, &OnBnClickedTriggerExternal)
END_MESSAGE_MAP()


//============================================================================
// WINDOWS MESSAGE HANDLER
//----------------------------------------------------------------------------
BOOL CSdoaqCameraFrameCallbackDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	SetIcon(m_hIcon, TRUE);
	SetIcon(m_hIcon, FALSE);

	GetDlgItem(IDC_CHECK_FRAME_CALLBACK)->EnableWindow(FALSE);
	GetDlgItem(IDC_SW_TRIGGER)->EnableWindow(FALSE);

	g_LogLine(_T("================================================"));
	g_LogLine(_T(" SDOAQ CAMERA FRAME CALLBACK SAMPLE"));
	g_LogLine(_T("================================================"));

	const int ver_major = ::SDOAQ_GetMajorVersion();
	const int ver_minor = ::SDOAQ_GetMinorVersion();
	const int ver_patch = ::SDOAQ_GetPatchVersion();
	g_LogLine(_T("SDOAQ DLL version is \"%d.%d.%d\""), ver_major, ver_minor, ver_patch);

	g_LogLine(_T("start initialization..."));
	const eErrorCode rv_sdoaq = ::SDOAQ_Initialize(NULL, NULL, g_SDOAQ_InitDoneCallback);
	if (ecNoError != rv_sdoaq)
	{
		g_LogLine(_T("SDOAQ_Initialize() returns error(%d)."), rv_sdoaq);
	}

	const WSIORV rv_wsio = ::WSUT_IV_CreateImageViewer((WSIOCSTR)_T("VIEWER")
		, (WSIOVOID)(this->m_hWnd), &g_hViewer, NULL
		, WSUTIVOPMODE_VISION | WSUTIVOPMODE_TOPTITLE);
	if (WSIORV_SUCCESS > rv_wsio)
	{
		g_LogLine(_T("WSUT_IV_CreateImageViewer() returns error(%d)."), rv_wsio);
	}

	SendMessage(WM_SIZE); // invoke WSUT_IV_ShowWindow call with size.

	return TRUE;  // return TRUE  unless you set the focus to a control
}

//----------------------------------------------------------------------------
void CSdoaqCameraFrameCallbackDlg::OnClose()
{
	(void)::SDOAQ_Finalize();
	(void)::WSUT_IV_DestroyImageViewer(g_hViewer);

	CDialogEx::OnClose();
}

//----------------------------------------------------------------------------
void CSdoaqCameraFrameCallbackDlg::OnSize(UINT nType, int cx, int cy)
{
	CDialogEx::OnSize(nType, cx, cy);

	CWnd* p_wnd = GetDlgItem(IDC_VIEWER);
	if (p_wnd)
	{
		CRect rc;
		GetDlgItem(IDC_VIEWER)->GetWindowRect(rc);
		ScreenToClient(rc);
		rc.top += 24;

		const WSIORV rv_wsio = ::WSUT_IV_ShowWindow(g_hViewer, (WSIOINT)true, rc.left, rc.top, rc.right, rc.bottom);
		if (WSIORV_SUCCESS > rv_wsio)
		{
			g_LogLine(_T("WSUT_IV_ShowWindow() returns error(%d)."), rv_wsio);
		}
	}
	else
	{
		// before OnInitDialog()
	}
}

//----------------------------------------------------------------------------
void CSdoaqCameraFrameCallbackDlg::OnBnClickedCheckFrameCallback()
{
	eErrorCode rv_sdoaq;
	if (((CButton*)GetDlgItem(IDC_CHECK_FRAME_CALLBACK))->GetCheck() == BST_CHECKED)
	{
		g_LogLine(_T("register frame callback function!"));
		rv_sdoaq = ::SDOAQ_SetFrameCallback(g_SDOAQ_FrameCallback);
	}
	else
	{
		g_LogLine(_T("stop frame callback function!"));
		rv_sdoaq = ::SDOAQ_SetFrameCallback(NULL);
	}
	if (ecNoError != rv_sdoaq)
	{
		g_LogLine(_T("SDOAQ_SetFrameCallback() returns error(%d)."), rv_sdoaq);
	}
}

//----------------------------------------------------------------------------
void CSdoaqCameraFrameCallbackDlg::OnBnClickedSwTrigger()
{
	if (((CButton*)theApp.m_pMainWnd->GetDlgItem(IDC_CHECK_FRAME_CALLBACK))->GetCheck() != BST_CHECKED)
	{
		g_LogLine(_T(">>> You must check \"camera frame callback\" to receive frames!"));
		return;
	}

	static int g_sw_trigger_no = 0;
	g_LogLine(_T("execute software trigger (%d)"), ++g_sw_trigger_no);
	const eErrorCode rv_sdoaq = ::SDOAQ_ExecCameraSoftwareTrigger();
	if (ecNoError != rv_sdoaq)
	{
		g_LogLine(_T("SDOAQ_ExecCameraSoftwareTrigger() returns error(%d)."), rv_sdoaq);
	}
}

//----------------------------------------------------------------------------
void CSdoaqCameraFrameCallbackDlg::OnBnClickedTriggerFreerun()
{
	g_SDOAQ_SetCameraTriggerMode(ctmFreerun);

	if (((CButton*)theApp.m_pMainWnd->GetDlgItem(IDC_CHECK_FRAME_CALLBACK))->GetCheck() != BST_CHECKED)
	{
		g_LogLine(_T(">>> You must check \"camera frame callback\" to receive frames!"));
	}
}

//----------------------------------------------------------------------------
void CSdoaqCameraFrameCallbackDlg::OnBnClickedTriggerSoftware()
{
	g_SDOAQ_SetCameraTriggerMode(ctmSoftware);

	if (((CButton*)theApp.m_pMainWnd->GetDlgItem(IDC_CHECK_FRAME_CALLBACK))->GetCheck() != BST_CHECKED)
	{
		g_LogLine(_T(">>> You must check \"camera frame callback\" to receive frames!"));
	}
}

//----------------------------------------------------------------------------
void CSdoaqCameraFrameCallbackDlg::OnBnClickedTriggerExternal()
{
	g_SDOAQ_SetCameraTriggerMode(ctmExternal);

	if (((CButton*)theApp.m_pMainWnd->GetDlgItem(IDC_CHECK_FRAME_CALLBACK))->GetCheck() != BST_CHECKED)
	{
		g_LogLine(_T(">>> You must check \"camera frame callback\" to receive frames!"));
	}
}


//============================================================================
// CALLBACK FUNCTION:
//----------------------------------------------------------------------------
static void g_SDOAQ_InitDoneCallback(eErrorCode errorCode, char* pErrorMessage)
{
	if (errorCode == ecNoError)
	{
		g_LogLine(_T("Initialization OK!!!"));

		g_LogLine(_T("register SDOAQ low level authorization"));
		::SDOAQ_RegisterLowLevelAuthorization();

		if (theApp.m_pMainWnd)
		{
			theApp.m_pMainWnd->GetDlgItem(IDC_CHECK_FRAME_CALLBACK)->EnableWindow(TRUE);
			theApp.m_pMainWnd->GetDlgItem(IDC_SW_TRIGGER)->EnableWindow(TRUE);
			((CButton*)theApp.m_pMainWnd->GetDlgItem(IDC_TRIGGER_SOFTWARE))->SetCheck(TRUE);
			g_SDOAQ_SetCameraTriggerMode(ctmSoftware);
		}
	}
	else
	{
		g_LogLine(_T("g_SDOAQ_InitDoneCallback() returns error(%d:%s)."), errorCode, (LPCTSTR)CA2W(pErrorMessage));
	}
}

//----------------------------------------------------------------------------
static void g_SDOAQ_FrameCallback(eErrorCode errorCode, unsigned char* pBuffer, size_t BufferSize, FrameDescriptor* pFrameDescriptor)
{
	if (errorCode == ecNoError && pBuffer && BufferSize && pFrameDescriptor)
	{
		// RECEIVED VALID FRAME

		static int g_frame_id = 0;
		static int nums_skip_to_display = 0;
		static ULONGLONG g_tick_next = 0;

		++g_frame_id;
		if (g_tick_next < GetTickCount64())
		{
			char sz_frame_id[256];
			sprintf_s(sz_frame_id, sizeof sz_frame_id, "Received Frame = %d  (Displayed Frame = %d)", g_frame_id, g_frame_id - nums_skip_to_display);
			const WSIORV rv_wsio = ::WSUT_IV_AttachRawImgData_V2(g_hViewer
				, pFrameDescriptor->pixelsWidth, pFrameDescriptor->pixelsHeight, pFrameDescriptor->bytesLine, pFrameDescriptor->bytesPixel
				, pBuffer, (int)BufferSize, sz_frame_id);
			if (WSIORV_SUCCESS > rv_wsio)
			{
				g_LogLine(_T("WSUT_IV_AttachRawImgData_V2() returns error(%d)."), rv_wsio);
			}

			g_tick_next = GetTickCount64() + 50; // minimum 50 msec
		}
		else
		{
			++nums_skip_to_display;
		}
	}
}

//----------------------------------------------------------------------------
static void g_SDOAQ_SetCameraTriggerMode(eCameraTriggerMode ctm)
{
	LPCTSTR sz;
	switch (ctm)
	{
	case ctmFreerun: sz = _T("Freerun"); break;
	case ctmSoftware: sz = _T("Software"); break;
	case ctmExternal: sz = _T("Eexternal"); break;
	default: sz = _T("invalid"); break;
	}

	g_LogLine(_T("set camera trigger mode -> %s"), sz);
	const eErrorCode rv_sdoaq = ::SDOAQ_SetCameraTriggerMode(ctm);
	if (ecNoError != rv_sdoaq)
	{
		g_LogLine(_T("SDOAQ_SetCameraTriggerMode() returns error(%d)."), rv_sdoaq);
	}
}
