#include "stdafx.h"

#include "SDOAQ_App.h"
#include "SDOAQ_AppDlg.h"
#include "afxdialogex.h"
#include "SDOAQ_Callback.h"

//----------------------------------------------------------------------------
CSDOAQ_Dlg::CSDOAQ_Dlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(IDD_SDOAQ_APP_DIALOG, pParent)
	, dxRangeStart()
	, dxRangeEnd()
	, dyRangeStart()
	, dyRangeEnd()
	, dzRangeStart()
	, dzRangeEnd()
	, m_hLogFile(INVALID_HANDLE_VALUE)
	, m_tickLastLog(0)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

}

//----------------------------------------------------------------------------
CSDOAQ_Dlg::~CSDOAQ_Dlg()
{
}

//----------------------------------------------------------------------------
BEGIN_MESSAGE_MAP(CSDOAQ_Dlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_WM_SIZE()
	ON_WM_CLOSE()
	ON_WM_DESTROY()
	ON_WM_TIMER()
	ON_MESSAGE(EUM_LOG, OnUmLog)
	ON_MESSAGE(EUM_ERROR, OnUmError)
	ON_MESSAGE(EUM_INITDONE, OnInitDone)
	ON_MESSAGE(EUM_RECEIVE_ZSTACK, OnReceiveZstack)
	ON_MESSAGE(EUM_RECEIVE_EDOF, OnReceiveEdof)
	ON_MESSAGE(EUM_RECEIVE_AF, OnReceiveAF)
	ON_MESSAGE(EUM_RECEIVE_SNAP, OnReceiveSnap)
	ON_BN_CLICKED(IDC_RADIO_WS1, OnUpdateWsSelection)
	ON_BN_CLICKED(IDC_RADIO_WS2, OnUpdateWsSelection)
	ON_BN_CLICKED(IDC_INITIALIZE, OnSdoaqInitialize)
	ON_BN_CLICKED(IDC_FINALIZE, OnSdoaqFinalize)
	ON_CBN_SELENDOK(IDC_COMBO_PARAMETER, OnSelectedCombobox)
	ON_BN_CLICKED(IDC_SET_PARAMETER, OnSdoaqSetParameter)
	ON_BN_CLICKED(IDC_SET_ROI, OnSdoaqSetROI)
	ON_BN_CLICKED(IDC_SET_AFROI, OnSdoaqSetAFROI)
	ON_BN_CLICKED(IDC_SET_RING_BUF_SIZE, OnSdoaqSetRingBufSize)
	ON_BN_CLICKED(IDC_SET_FOCUS_SET, OnSdoaqSetFocusSet)
	ON_BN_CLICKED(IDC_SET_SNAPFOCUS_SET, OnSdoaqSetSnapFocusSet)
	ON_BN_CLICKED(IDC_SET_EDOF_RESIZE_RATIO, OnSdoaqSetEdofResize)
	ON_BN_CLICKED(IDC_ACQ_STACK, OnSdoaqSingleShotStack)
	ON_BN_CLICKED(IDC_CONTI_STACK, OnSdoaqPlayStack)
	ON_BN_CLICKED(IDC_STOP_STACK, OnSdoaqStopStack)
	ON_BN_CLICKED(IDC_ACQ_EDOF, OnSdoaqSingleShotEdof)
	ON_BN_CLICKED(IDC_CONTI_EDOF, OnSdoaqPlayEdof)
	ON_BN_CLICKED(IDC_STOP_EDOF, OnSdoaqStopEdof)
	ON_BN_CLICKED(IDC_ACQ_AF, OnSdoaqSingleShotAF)
	ON_BN_CLICKED(IDC_CONTI_AF, OnSdoaqPlayAF)
	ON_BN_CLICKED(IDC_STOP_AF, OnSdoaqStopAF)
	ON_BN_CLICKED(IDC_SNAP, OnSdoaqSnap)
	ON_BN_CLICKED(IDC_SET_CALIBRATION, OnSdoaqSetCalibrationFile)
	ON_CBN_SELENDOK(IDC_COMBO_OBJECTIVE, OnSdoaqComboObjective)
END_MESSAGE_MAP()

//============================================================================
BOOL CSDOAQ_Dlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// Add "About..." menu item to system menu.

	// IDM_ABOUTBOX must be in the system command range.
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		BOOL bNameValid;
		CString strAboutMenu;
		bNameValid = strAboutMenu.LoadString(IDS_ABOUTBOX);
		ASSERT(bNameValid);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	// TODO: Add extra initialization here
	BuildEnvironment();

	//-------------------------------------------------------------------------------------
	// Register multiple wisescopes uses before initialization
	//-------------------------------------------------------------------------------------
	GetDlgItem(IDC_RADIO_WS1)->ShowWindow(MULWS == MULTI_2WS_SDOAQ ? SW_SHOW : SW_HIDE);
	GetDlgItem(IDC_RADIO_WS2)->ShowWindow(MULWS == MULTI_2WS_SDOAQ ? SW_SHOW : SW_HIDE);
	switch (MULWS)
	{
	case MULTI_2WS_SDOAQ:
		((CButton*)GetDlgItem(IDC_RADIO_WS1))->SetCheck(BST_CHECKED);
		((CButton*)GetDlgItem(IDC_RADIO_WS2))->SetCheck(BST_UNCHECKED);
		::SDOAQ_RegisterMultiWsApi();
		OnUpdateWsSelection();
		break;
	case MULTI_1WS_SDOAQ:
		::SDOAQ_RegisterMultiWsApi();
		m_cur_ws = 0;
		break;
	case SINGLE_WS_SDOAQ:
	default:
		m_cur_ws = 0;
		break;
	}

	BuildParameterID_Combobox();

	((CButton*)GetDlgItem(IDC_CHECK_STEPMAP))->SetCheck(1);
	((CButton*)GetDlgItem(IDC_CHECK_EDOF))->SetCheck(1);
	//((CButton*)GetDlgItem(IDC_CHECK_QUALITYMAP))->SetCheck(1);
	((CButton*)GetDlgItem(IDC_CHECK_HEIGHTMAP))->SetCheck(1);
	((CButton*)GetDlgItem(IDC_CHECK_POINTCLOUD))->SetCheck(1);

	//BuildCalibrationFile_Combobox();

	//// CREATE IMAGE VIEWER
	for (auto& ws : vWSSET)
	{
		auto& vw = m_vVW[ws];
		vw.vhwnd_iv.resize(3, NULL);
		for (auto& each : vw.vhwnd_iv)
		{
			if (WSIORV_SUCCESS <= ::WSUT_IV_CreateImageViewer((WSIOCSTR)_T("Main viewer")
				, (WSIOVOID)(this->m_hWnd), (WSIOVOID*)&each, 0
				, WSUTIVOPMODE_VISION | WSUTIVOPMODE_TOPTITLE | WSUTIVOPMODE_FRAMEOUTER
			))
			{
				(void)WSUT_IV_SetColor(each, WSUTIVRESOURCE_OUTERFRAME, RGB(70, 130, 180));
			}
			else
			{
				each = NULL;
				print_wsio_last_error();
			}
		}
		if (ws == ONLY_WS0_3D)
		{
			auto rv = ::WSGL_Initialize(m_hWnd, (WSIOVOID*)&vw.hwnd_3d);
			if (WSIORV_SUCCESS <= rv)
			{
				(void)::WSGL_SetDisplayAttributes(vw.hwnd_3d, EDA_SHOW_GUIDER_OBJECTS
					| EDA_SHOW_SCALE_OBJECTS
					| EDA_SHOW_COLORMAPBAR_OBJECTS
					| EDA_NOHIDE_PICKER
					//| EDA_SHOW_PICKER_ON_MOUSE
					//| EDA_SHOW_PICKED_POINT_INFORMATION
					//| EDA_MEASURE_PICKERS_ON_LCLCK_BUTTON
					//| EDA_SHOW_LIST_MEASURED_DATA
				);
				(void)::WSGL_Display_BG(vw.hwnd_3d);
			}
			else
			{
				Log(FString(_T("[WSGL Error] >> return error %d."), rv));
				print_wsgl_last_error(vw.hwnd_3d);
			}
		}
	}

	ShowViewer();

	ShowWindow(SW_MAXIMIZE);

	return TRUE;  // return TRUE  unless you set the focus to a control
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::tTestSet::t_ui::update_editwnd(CWnd& wnd)
{
	wnd.GetDlgItem(IDC_EDIT_ROI)->SetWindowText(ROI);
	wnd.GetDlgItem(IDC_EDIT_AFROI)->SetWindowText(AFROI);
	wnd.GetDlgItem(IDC_EDIT_RING_BUF_SIZE)->SetWindowText(RING_BUF_SIZE);
	wnd.GetDlgItem(IDC_EDIT_FOCUS_SET)->SetWindowText(FOCUS_SET);
	wnd.GetDlgItem(IDC_EDIT_SNAPFOCUS_SET)->SetWindowText(SNAPFOCUS_SET);
	wnd.GetDlgItem(IDC_EDIT_EDOF_RESIZE_RATIO)->SetWindowText(EDOF_RESIZE_RATIO);
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::BuildParameterID_Combobox(void)
{
	auto p_combo = (CComboBox*)GetDlgItem(IDC_COMBO_PARAMETER);
	if (p_combo)
	{
		p_combo->ResetContent();
		auto ADD_PI = [&](LPCTSTR name, eParameterId value)
		{
			p_combo->SetItemData(p_combo->AddString(name), value);
		};

		ADD_PI(_T("CameraExposureTime"), piCameraExposureTime);
		ADD_PI(_T("CameraFullFrameSizeX"), piCameraFullFrameSizeX);
		ADD_PI(_T("CameraFullFrameSizeY"), piCameraFullFrameSizeY);
		ADD_PI(_T("CameraPixelSizeX"), piCameraPixelSizeX);
		ADD_PI(_T("CameraPixelSizeY"), piCameraPixelSizeY);
		ADD_PI(_T("CameraBinning"), piCameraBinning);
		ADD_PI(_T("CameraGain"), piCameraGain);
		ADD_PI(_T("WhiteBalanceRed"), piWhiteBalanceRed);
		ADD_PI(_T("WhiteBalanceGreen"), piWhiteBalanceGreen);
		ADD_PI(_T("WhiteBalanceBlue"), piWhiteBalanceBlue);
		if ("sdzeiss lighting device")
		{
			ADD_PI(_T("InnerRingIntensity"), piInnerRingIntensity);
			ADD_PI(_T("MiddleRingIntensity"), piMiddleRingIntensity);
			ADD_PI(_T("OuterRingIntensity"), piOuterRingIntensity);
			ADD_PI(_T("CoaxIntensity"), piCoaxIntensity);
		}
		if ("lcbpwm lighting device")
		{
			ADD_PI(_T("Channel 1 Intensity"), piIntensityGeneralChannel_1);
			ADD_PI(_T("Channel 2 Intensity"), piIntensityGeneralChannel_2);
			ADD_PI(_T("Channel 3 Intensity"), piIntensityGeneralChannel_3);
			ADD_PI(_T("Channel 4 Intensity"), piIntensityGeneralChannel_4);
			ADD_PI(_T("Channel 5 Intensity"), piIntensityGeneralChannel_5);
			ADD_PI(_T("Channel 6 Intensity"), piIntensityGeneralChannel_6);
			ADD_PI(_T("Channel 7 Intensity"), piIntensityGeneralChannel_7);
			ADD_PI(_T("Channel 8 Intensity"), piIntensityGeneralChannel_8);
		}
		ADD_PI(_T("FocusPosition"), piFocusPosition);
		ADD_PI(_T("ReflexCorrectionAlgorithm"), piReflexCorrectionAlgorithm);
		ADD_PI(_T("ReflexCorrectionPattern"), piReflexCorrectionPattern);
		ADD_PI(_T("MaxStacksPerSecond"), piMaxStacksPerSecond);
		ADD_PI(_T("ObjectiveId"), piObjectiveId);
		ADD_PI(_T("edof_calc_resize_ratio"), pi_edof_calc_resize_ratio);
		ADD_PI(_T("edof_calc_pixelwise_kernel_size"), pi_edof_calc_pixelwise_kernel_size);
		ADD_PI(_T("edof_calc_pixelwise_iteration"), pi_edof_calc_pixelwise_iteration);
		ADD_PI(_T("edof_depthwise_kernel_size"), pi_edof_depthwise_kernel_size);
		ADD_PI(_T("edof_depth_quality_th"), pi_edof_depth_quality_th);
		ADD_PI(_T("edof_bilateral_sigma_color"), pi_edof_bilateral_sigma_color);
		ADD_PI(_T("edof_bilateral_sigma_space"), pi_edof_bilateral_sigma_space);
		ADD_PI(_T("edof_num_thread"), pi_edof_num_thread);
		ADD_PI(_T("edof_is_scale_correction_enabled"), pi_edof_is_scale_correction_enabled);
		ADD_PI(_T("edof_scale_correction_dst_step"), pi_edof_scale_correction_dst_step);
		ADD_PI(_T("af_sharpness_measure_method"), pi_af_sharpness_measure_method);
		ADD_PI(_T("af_resampling_method"), pi_af_resampling_method);
		ADD_PI(_T("af_stability_method"), pi_af_stability_method);
		ADD_PI(_T("af_stability_debounce_count"), pi_af_stability_debounce_count);
		ADD_PI(_T("SaveFileFormat"), piSaveFileFormat);
		ADD_PI(_T("SavePixelBits - not supported yet"), piSavePixelBits);
		ADD_PI(_T("FocusLeftTop"), piFocusLeftTop);
		ADD_PI(_T("FocusRightBottom"), piFocusRightBottom);
		ADD_PI(_T("CameraColor"), piCameraColor);
		ADD_PI(_T("FocusMeasureMethod"), piFocusMeasureMethod);
		ADD_PI(_T("SingleFocus"), piSingleFocus);
		//p_combo->SetCurSel(0);
	}
}

//----------------------------------------------------------------------------
eParameterId CSDOAQ_Dlg::GetCurrentParameterID(void)
{
	auto p_combo = (CComboBox*)GetDlgItem(IDC_COMBO_PARAMETER);
	if (p_combo)
	{
		auto cur_sel = p_combo->GetCurSel();
		if (cur_sel >= 0)
		{
			return (eParameterId)p_combo->GetItemData(p_combo->GetCurSel());
		}
	}
	return (eParameterId)piInvalidParameter;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::BuildCalibrationFile_Combobox()
{
	// 1. make calib file list from current directory
	std::vector<CString> vsCalibList;
	m_calFile.GetCalibList(GetCurrentDir(), vsCalibList);
	//m_calFile.GetCalibList(FString(_T("%s\\%s"), GetCurrentDir(), _T("Calib")), vsCalibList);

	// 2. parse file
	m_calFile.calibData.clear();
	for (auto it = vsCalibList.begin(); it != vsCalibList.end(); it++)
	{
		m_calFile.BuildCalibData(*it);
	}

	auto p_combo = (CComboBox*)GetDlgItem(IDC_COMBO_OBJECTIVE);
	p_combo->ResetContent();
	for (auto it = m_calFile.calibData.begin(); it != m_calFile.calibData.end(); it++)
	{
		p_combo->AddString(it->objective);
	}
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::ShowViewer(void)
{
	auto p_wnd = GetDlgItem(IDC_IMAGE);
	if (p_wnd)
	{
		CRect rc_ws[2];
		p_wnd->GetWindowRect(rc_ws[0]);
		ScreenToClient(rc_ws[0]);
		if (MULWS == MULTI_2WS_SDOAQ)
		{
			rc_ws[1] = rc_ws[0];
			rc_ws[0].right = rc_ws[1].left = (rc_ws[0].left + rc_ws[0].right) / 2;
		}

		for (auto& ws : vWSSET)
		{
			auto& vw = m_vVW[ws];
			auto& rc = rc_ws[ws];

			const int cx = rc.Width() / 2;
			const int cy = rc.Height() / 2;

			std::vector<CRect> vrc;
			vrc.push_back(CRect(rc.left + 0, rc.top + 0, rc.left + cx, rc.top + cy));
			vrc.push_back(CRect(rc.left + cx, rc.top + 0, rc.right, rc.top + cy));
			vrc.push_back(CRect(rc.left + 0, rc.top + cy, rc.left + cx, rc.bottom));
			CRect rc3d(rc.left + cx, rc.top + cy, rc.right, rc.bottom);

			for (size_t uid = 0; uid < vw.vhwnd_iv.size(); uid++)
			{
				::WSUT_IV_ShowWindow(vw.vhwnd_iv[uid], (WSIOINT)true, vrc[uid].left, vrc[uid].top, vrc[uid].right, vrc[uid].bottom);
			}
			if (ws == ONLY_WS0_3D)
			{
				(void)::WSGL_ShowWindow(vw.hwnd_3d, (WSIOINT)true, rc3d.left, rc3d.top, rc3d.right, rc3d.bottom);
			}
		}
	}
}

//----------------------------------------------------------------------------
// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.
//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CRect Rect;
		GetWindowRect(Rect);
		ScreenToClient(&Rect);
		InvalidateRect(Rect);

		CDialogEx::OnPaint();
	}
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSize(UINT nType, int cx, int cy)
{
	CDialogEx::OnSize(nType, cx, cy);

	ShowViewer();
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnClose()
{
	::SDOAQ_Finalize();

	for (auto& ws : vWSSET)
	{
		auto& vw = m_vVW[ws];
		for (auto& each : vw.vhwnd_iv)
		{
			(void)::WSUT_IV_DestroyImageViewer(each);
		}
		if (ws == ONLY_WS0_3D)
		{
			(void)::WSGL_Finalize(vw.hwnd_3d);
		}
	}
	CDialogEx::OnClose();
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnDestroy()
{
	t_vmsgid vmsgid;
	vmsgid.push_back(EUM_LOG);
	vmsgid.push_back(EUM_ERROR);
	DeleteAllWinmsgWithWStringPtrInLparam(m_hWnd, vmsgid);

	__super::OnDestroy();
}

//----------------------------------------------------------------------------
// The system calls this function to obtain the cursor to display while the user drags
//  the minimized window.
//----------------------------------------------------------------------------
HCURSOR CSDOAQ_Dlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

//---------------------------------------------------------------------------
void CSDOAQ_Dlg::OnTimer(UINT_PTR nIDEvent)
{
	switch (nIDEvent)
	{
	case TIMER_LOG:
		PrintLog();
		break;
	}

	__super::OnTimer(nIDEvent);
}

//----------------------------------------------------------------------------
BOOL CSDOAQ_Dlg::PreTranslateMessage(MSG* pMsg)
{
	// TODO: Add your specialized code here and/or call the base class
	if (pMsg->message == WM_KEYDOWN)
	{
		auto& hwnd_3d = m_vVW[ONLY_WS0_3D].hwnd_3d;
		if (pMsg->wParam == VK_RETURN)
		{
			return TRUE;
		}
		else if (pMsg->wParam == VK_ESCAPE)
		{
			return TRUE;
		}
		else if (pMsg->wParam == VK_BACK)
		{
			if (::WSGL_IsOnRun(hwnd_3d))
			{
				(void)::WSGL_ClearLastMeasureData(hwnd_3d, true);
			}
		}
		else if (pMsg->wParam == VK_DELETE)
		{
			if (::WSGL_IsOnRun(hwnd_3d))
			{
				(void)::WSGL_ClearAllMeasureData(hwnd_3d, true);
			}
		}
		else if (pMsg->wParam == VK_HOME)
		{
			if (::WSGL_IsOnRun(hwnd_3d))
			{
				(void)::WSGL_ResetDisplay(hwnd_3d);
			}
		}
	}

	return CDialogEx::PreTranslateMessage(pMsg);
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::Log(LPCTSTR p_log_str)
{
	CString sLogFileName = FString(_T("%s\\%s.log"), m_sLogPath, CTime::GetCurrentTime().Format("%Y.%m.%d"));
	DWORD dwDummy;
	if (m_sLogFileName != sLogFileName)
	{
		if (m_hLogFile != INVALID_HANDLE_VALUE)
		{
			CloseHandle(m_hLogFile);
		}

		m_hLogFile = CreateFile(sLogFileName, GENERIC_WRITE, FILE_SHARE_READ, NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
		m_sLogFileName = sLogFileName;
	}
	if (m_hLogFile != INVALID_HANDLE_VALUE)
	{
		SetFilePointer(m_hLogFile, 0, NULL, FILE_END);
		CString sLog = p_log_str + CString("\r\n");
		WriteFile(m_hLogFile, CStringA(sLog), CStringA(sLog).GetLength(), &dwDummy, NULL);
	}

	m_sLog += p_log_str;
	m_sLog += "\r\n";

	if (m_sLog.GetLength() > 4 * 64 * 1024)
	{
		m_sLog = m_sLog.Right(3 * 64 * 1024);
	}

	const DWORD LOG_PERIOD = 100;
	DWORD tickDiff = GetTickCount() - m_tickLastLog;
	if (tickDiff < LOG_PERIOD)
	{
		SetTimer(TIMER_LOG, LOG_PERIOD - tickDiff, NULL);
	}
	else
	{
		PrintLog();
	}
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::PrintLog(void)
{
	KillTimer(TIMER_LOG);
	m_tickLastLog = GetTickCount();

	CEdit* const pWnd = (CEdit*)AfxGetMainWnd()->GetDlgItem(IDC_LOG);
	if (pWnd)
	{
		pWnd->SetWindowText(m_sLog);
		const int nLen = pWnd->GetWindowTextLength();
		pWnd->SetSel(nLen, nLen);
	}
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::ApiError(LPCTSTR sApi, int eCode)
{
	Log(FString(_T("[API Error] >> %s() return error %d (=%s)."), sApi, eCode, GetSdoaqErrorString(eCode)));
}

//----------------------------------------------------------------------------
LRESULT CSDOAQ_Dlg::OnUmLog(WPARAM wSeverity, LPARAM lpMessage)
{
	CString* pMessage = (CString*)lpMessage;
	if (pMessage)
	{
		LPCTSTR sSeverity;
		switch ((eLogSeverity)wSeverity)
		{
		case lsError: sSeverity = _T("Error"); break;
		case lsWarning: sSeverity = _T("Warning"); break;
		case lsInfo: sSeverity = _T("Info"); break;
		case lsVerbose: sSeverity = _T("Verbose"); break;
		default: sSeverity = _T("Unknown"); break;
		}
		Log(FString(_T("[LogCallback : %s] %s"), sSeverity, *pMessage));
		delete pMessage;
	}
	return 0;
}

//----------------------------------------------------------------------------
LRESULT CSDOAQ_Dlg::OnUmError(WPARAM wErrorCode, LPARAM lpMessage)
{
	CString* pMessage = (CString*)lpMessage;

	Log(FString(_T("[ErrorCallback : ErrorCode:%d] %s"), wErrorCode, pMessage ? *pMessage : _T("")));

	if (pMessage)
	{
		delete pMessage;
	}
	return 0;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::BuildEnvironment(void)
{
	m_sScriptFile = FString(_T("%s\\wisescope.script.txt"), GetCurrentDir());
	m_sLogPath = FString(_T("%s\\Log"), GetCurrentDir());

	MULWS = SINGLE_WS_SDOAQ;
	(void)::WSUT_IntFromLineScript((CStringA)m_sScriptFile, "Number of WSM", &MULWS);

	vWSSET.clear();
	for (int wsi = 0; wsi < (MULWS == MULTI_2WS_SDOAQ ? 2 : 1); wsi++)
	{
		vWSSET.push_back(wsi);
	}

	(void)::CreateDirectory(m_sLogPath, NULL);

	const int nMajorVersion = ::SDOAQ_GetMajorVersion();
	const int nMinorVersion = ::SDOAQ_GetMinorVersion();
	const int nPatchVersion = ::SDOAQ_GetPatchVersion();
	const int nAlgoVersion = ::SDOAQ_GetAlgorithmVersion();

	SetWindowText(FString(_T("MULTI SDOAQ API TESTER (dll %d.%d.%d)"), nMajorVersion, nMinorVersion, nPatchVersion));

	Log(_T(">> =================================================="));
	Log(_T(">> Welcome to MULTI SDOAQ API Tester"));
	Log(_T(">> =================================================="));

	Log(FString(_T(">> SDOAQ dll version: %d.%d.%d"), nMajorVersion, nMinorVersion, nPatchVersion));
	Log(FString(_T(">> sdedof dll version: %d.%d"), nAlgoVersion / 1000, nAlgoVersion % 1000));

	if (m_sScriptFile.GetLength())
	{
		::SDOAQ_SetSystemScriptFilename((CStringA)m_sScriptFile);
		Log(FString(_T(">> Script file: %s"), m_sScriptFile));
	}
	Log(FString(_T(">> Log path: %s"), m_sLogPath));
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::ReadySdoaqDll(void)
{
	BuildCalibrationFile_Combobox();
}

//----------------------------------------------------------------------------
LRESULT CSDOAQ_Dlg::OnInitDone(WPARAM wErrorCode, LPARAM lpMessage)
{
	CString* pMessage = (CString*)lpMessage;

	Log(FString(_T("[InitDoneCallback : ErrorCode:%d] %s"), wErrorCode, pMessage ? *pMessage : _T("")));

	if (ecNoError == wErrorCode)
	{
		GetDlgItem(IDC_INITIALIZE)->EnableWindow(FALSE);

		ReadySdoaqDll();

		for (auto& each : vWSSET)
		{
			// Walk the wisescope list, and process all camera-related basic settings about each module
			m_cur_ws = each;
			::SDOAQ_SelectMultiWs(m_cur_ws + 1);
			VSET[m_cur_ws].m_nColorByte = IsMonoCameraInstalled() ? MONOBYTES : COLORBYTES;

			// It is appropriate that some of the items in the initial settings below are actually read from SDOAQ
#if 0
			int nDummy;
			(void)::SDOAQ_GetIntParameterRange(piCameraFullFrameSizeX, &nDummy, &nMaxWidth);
			(void)::SDOAQ_GetIntParameterRange(piCameraFullFrameSizeY, &nDummy, &nMaxHeight);
			GetDlgItem(IDC_EDIT_ROI)->SetWindowText(FString(_T("0,0,%d,%d"), nMaxWidth, nMaxHeight));
#endif

			if (TRUE)
			{
				VSET[m_cur_ws].ui.ROI = _T("0,0,2040,1086");
				VSET[m_cur_ws].ui.AFROI = _T("956,479,128,128");
				VSET[m_cur_ws].ui.RING_BUF_SIZE = _T("3");
				VSET[m_cur_ws].ui.FOCUS_SET = _T("0-319-32");
				VSET[m_cur_ws].ui.SNAPFOCUS_SET = _T("0-319-16");
				VSET[m_cur_ws].ui.EDOF_RESIZE_RATIO = _T("0.5");
			}

			VSET[m_cur_ws].ui.update_editwnd(*this);
			OnSdoaqSetROI();
			OnSdoaqSetAFROI();
			OnSdoaqSetRingBufSize();
			OnSdoaqSetFocusSet();
			OnSdoaqSetSnapFocusSet();
		}

		// reset m_cur_ws to the actual UI settings
		OnUpdateWsSelection();

		double dbValue;
		eErrorCode rv = ::SDOAQ_GetDblParameterValue(pi_edof_calc_resize_ratio, &dbValue);
		if (ecNoError == rv)
		{
			CString sValue;
			sValue.Format(_T("%.3lf"), dbValue);
			SetDlgItemText(IDC_EDIT_EDOF_RESIZE_RATIO, sValue);
		}
	}

	if (pMessage)
	{
		delete pMessage;
	}

	return 0;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnUpdateWsSelection()
{
	const bool ws1 = (((CButton*)GetDlgItem(IDC_RADIO_WS1))->GetCheck() == BST_CHECKED);
	const bool ws2 = (((CButton*)GetDlgItem(IDC_RADIO_WS2))->GetCheck() == BST_CHECKED);
	const auto new_ws = (ws2 ? 1 : 0);

	if (m_cur_ws != new_ws)
	{
		m_cur_ws = new_ws;
		::SDOAQ_SelectMultiWs(new_ws + 1);

		UpdateSelectedCombobox(false);
		VSET[m_cur_ws].ui.update_editwnd(*this);
	}
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqInitialize()
{
	for (auto& each : VSET)
	{
		each.rb.active = false;
	}

	eErrorCode rv = ::SDOAQ_Initialize(g_LogCallback, g_ErrorCallback, g_InitDoneCallback);

	(void)::SDOAQ_RegisterMoveokCallback(g_MoveokCallback);
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqFinalize()
{
	eErrorCode rv = ::SDOAQ_Finalize();
	GetDlgItem(IDC_INITIALIZE)->EnableWindow(TRUE);

	for (auto& each : VSET)
	{
		each.rb.active = false;
		each.ClearBuffer();
	}
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::UpdateSelectedCombobox(bool o_log)
{
	auto APIERROR = [&](LPCTSTR sz, eErrorCode e) { if (o_log) { ApiError(sz, e); }};

	// The parameter id selected in the combo box
	eParameterId paraID = GetCurrentParameterID();

	if (paraID == piInvalidParameter)
	{
		GetDlgItem(IDC_EDIT_VALUE)->SetWindowText(_T(""));
		return;
	}

	bool flagAvailable = false;
	eErrorCode rv = ::SDOAQ_IsParameterAvailable(paraID, &flagAvailable);
	if (ecNoError == rv)
	{
		CString sValue;
		if (flagAvailable)
		{
			eParameterType eType;
			rv = ::SDOAQ_GetParameterType(paraID, &eType);
			if (ecNoError == rv)
			{
				if (eType == ptInt)
				{
					int nValue;
					eErrorCode rv = ::SDOAQ_GetIntParameterValue(paraID, &nValue);
					if (ecNoError == rv)
					{
						if (paraID == piReflexCorrectionPattern)
						{
							sValue.Format(_T("0x%x"), nValue);
						}
						else
						{
							sValue.Format(_T("%d"), nValue);
						}
					}
					else
					{
						sValue = "Not supported parameter";
						APIERROR(FString(_T("SDOAQ_GetIntParameterValue[ParamID-%d]"), paraID), rv);
					}
				}
				else if (eType == ptDouble)
				{
					double dbValue;
					eErrorCode rv = ::SDOAQ_GetDblParameterValue(paraID, &dbValue);
					if (ecNoError == rv)
					{
						sValue.Format(_T("%.3lf"), dbValue);
					}
					else
					{
						sValue = "Not supported parameter";
						APIERROR(FString(_T("SDOAQ_GetDblParameterValue[ParamID-%d]"), paraID), rv);
					}
				}
				else // ptString
				{

				}
			}
			else
			{
				sValue = "Not supported parameter";
				APIERROR(FString(_T("SDOAQ_GetParameterType[ParamID-%d]"), paraID), rv);
			}
			GetDlgItem(IDC_EDIT_VALUE)->SetWindowText(sValue);
		}
	}
	else
	{
		APIERROR(FString(_T("SDOAQ_IsParameterAvailable[ParamID-%d]"), paraID), rv);
	}

	// Disable items that are not allowed to be written
	bool flagWritable = false;
	(void)::SDOAQ_IsParameterWritable(paraID, &flagWritable);
	const BOOL WRITABLE = flagWritable ? TRUE : FALSE;
	GetDlgItem(IDC_EDIT_VALUE)->EnableWindow(WRITABLE);
	GetDlgItem(IDC_SET_PARAMETER)->EnableWindow(WRITABLE);
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqSetParameter()
{
	// The parameter id selected in the combo box
	eParameterId paraID = GetCurrentParameterID();

	bool flagAvailable = false;
	eErrorCode rv = ::SDOAQ_IsParameterAvailable(paraID, &flagAvailable);
	if (ecNoError == rv)
	{
		bool flagWritable = false;
		rv = ::SDOAQ_IsParameterWritable(paraID, &flagWritable);
		if (ecNoError == rv)
		{
			if (flagAvailable && flagWritable)
			{
				eParameterType eType;
				(void)::SDOAQ_GetParameterType(paraID, &eType);

				CString sParameters;
				GetDlgItemText(IDC_EDIT_VALUE, sParameters);

				if (eType == ptInt)
				{
					sParameters.MakeLower();

					int nValue = 0;
					if (sParameters.Find(_T("x")) != -1
						&& paraID == piReflexCorrectionPattern
						)
					{
						swscanf_s(sParameters, L"%x", &nValue);
					}
					else
					{
						nValue = _ttoi(sParameters);
					}
					int nMin, nMax;

					rv = ::SDOAQ_GetIntParameterRange(paraID, &nMin, &nMax);
					if (ecNoError == rv)
					{
						if (nValue >= nMin && nValue <= nMax)
						{
							(void)::SDOAQ_SetIntParameterValue(paraID, nValue);
						}
						else
						{
							ApiError(FString(_T("SDOAQ_SetIntParameterValue[ParamID-%d] : value is out of range[%d ~ %d]"), paraID, nMin, nMax), ecUnknownError);
						}
					}
					else
					{
						ApiError(FString(_T("SDOAQ_GetIntParameterRange[ParamID-%d]"), paraID), rv);
					}

				}
				else if (eType == ptDouble)
				{
					double dbValue = _ttof(sParameters);
					double dbMin, dbMax;

					rv = ::SDOAQ_GetDblParameterRange(paraID, &dbMin, &dbMax);
					if (ecNoError == rv)
					{
						if (dbValue >= dbMin && dbValue <= dbMax)
						{
							(void)::SDOAQ_SetDblParameterValue(paraID, dbValue);
						}
						else
						{
							ApiError(FString(_T("SDOAQ_SetDblParameterValue[ParamID-%d] : value is out of range[%.1lf ~ %.1lf]"), paraID, dbMin, dbMax), ecUnknownError);
						}
					}
					else
					{
						ApiError(FString(_T("SDOAQ_GetDblParameterRange[ParamID-%d]"), paraID), rv);
					}
				}
				else // ptString
				{

				}
			}
		}
		else
		{
			ApiError(FString(_T("SDOAQ_IsParameterWritable[ParamID-%d]"), paraID), rv);
		}
	}
	else
	{
		ApiError(FString(_T("SDOAQ_IsParameterAvailable[ParamID-%d]"), paraID), rv);
	}
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqSetROI()
{
	ASSERT(m_cur_ws < TWO_WS);
	auto& SET = VSET[m_cur_ws];

	CString sParameters;
	GetDlgItemText(IDC_EDIT_ROI, sParameters);

	CString sLeft, sTop, sWidth, sHeight;
	AfxExtractSubString(sLeft, sParameters, 0, ',');
	AfxExtractSubString(sTop, sParameters, 1, ',');
	AfxExtractSubString(sWidth, sParameters, 2, ',');
	AfxExtractSubString(sHeight, sParameters, 3, ',');

	AcquisitionFixedParametersEx AFP;
	AFP.cameraRoiTop = _ttoi(sTop);
	AFP.cameraRoiLeft = _ttoi(sLeft);
	AFP.cameraRoiWidth = (_ttoi(sWidth) / 4) * 4;
	AFP.cameraRoiHeight = _ttoi(sHeight);
	AFP.cameraBinning = 1;
	AFP.callbackUserData = NULL;

	int nDummy;
	auto rv = ::SDOAQ_GetIntParameterRange(piCameraFullFrameSizeX, &nDummy, &nMaxWidth);
	if (ecNoError == rv)
	{
		if (AFP.cameraRoiLeft < 0 || AFP.cameraRoiLeft > nMaxWidth)
		{
			ApiError(FString(_T("Set cameraRoiLeft : value is out of range[ ~ %d]"), nMaxWidth), ecUnknownError);
			return;
		}
		if (AFP.cameraRoiWidth < 1 || AFP.cameraRoiWidth > nMaxWidth)
		{
			ApiError(FString(_T("Set cameraRoiWidth : value is out of range[ ~ %d]"), nMaxWidth), ecUnknownError);
			return;
		}
	}

	rv = ::SDOAQ_GetIntParameterRange(piCameraFullFrameSizeY, &nDummy, &nMaxHeight);
	if (ecNoError == rv)
	{
		if (AFP.cameraRoiTop < 0 || AFP.cameraRoiTop > nMaxHeight)
		{
			ApiError(FString(_T("Set cameraRoiTop : value is out of range[ ~ %d]"), nMaxHeight), ecUnknownError);
			return;
		}
		if (AFP.cameraRoiHeight < 1 || AFP.cameraRoiHeight > nMaxHeight)
		{
			ApiError(FString(_T("Set cameraRoiHeight : value is out of range[ ~ %d]"), nMaxHeight), ecUnknownError);
			return;
		}
	}

	if (!SET.rb.active)
	{
		SET.afp = AFP;
	}

	SET.ui.ROI = sParameters;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqSetAFROI()
{
	CString sParameters;
	GetDlgItemText(IDC_EDIT_AFROI, sParameters);

	CString sLeft, sTop, sWidth, sHeight;
	AfxExtractSubString(sLeft, sParameters, 0, ',');
	AfxExtractSubString(sTop, sParameters, 1, ',');
	AfxExtractSubString(sWidth, sParameters, 2, ',');
	AfxExtractSubString(sHeight, sParameters, 3, ',');

	CRect recAF(_ttoi(sLeft), _ttoi(sTop), _ttoi(sLeft) + _ttoi(sWidth), _ttoi(sTop) + _ttoi(sHeight));
	const auto rv1 = ::SDOAQ_SetIntParameterValue(piFocusLeftTop, ((recAF.left & 0x0000FFFF) << 16) | (recAF.top & 0x0000FFFF) << 0);
	const auto rv2 = ::SDOAQ_SetIntParameterValue(piFocusRightBottom, ((recAF.right & 0x0000FFFF) << 16) | (recAF.bottom & 0x0000FFFF) << 0);
	if (rv1 != ecNoError)
	{
		ApiError(FString(_T("Set AF ROI [ParamID-%d]"), piFocusLeftTop), rv1);
		return;
	}
	if (rv2 != ecNoError)
	{
		ApiError(FString(_T("Set AF ROI [ParamID-%d]"), piFocusRightBottom), rv2);
		return;
	}

	ASSERT(m_cur_ws < TWO_WS);
	VSET[m_cur_ws].ui.AFROI = sParameters;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqSetRingBufSize()
{
	CString sSize;
	GetDlgItemText(IDC_EDIT_RING_BUF_SIZE, sSize);

	ASSERT(m_cur_ws < TWO_WS);
	auto& SET = VSET[m_cur_ws];

	SET.ui.RING_BUF_SIZE = sSize;
	SET.ui.nRingBufferSize = max(1, _ttoi(sSize));
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqSetFocusSet()
{
	CString sFocusSet;
	GetDlgItemText(IDC_EDIT_FOCUS_SET, sFocusSet);

	ASSERT(m_cur_ws < TWO_WS);
	auto& SET = VSET[m_cur_ws];

	SET.ui.vFocusSet.clear();

	if (sFocusSet.Find(_T("-")) != -1)
	{
		int nLow, nHigh, nUnit;
		if (3 <= swscanf_s((LPCTSTR)sFocusSet, _T("%d-%d-%d"), &nLow, &nHigh, &nUnit))
		{
			for (int nFocus = nLow; nFocus <= nHigh; nFocus += nUnit)
			{
				SET.ui.vFocusSet.push_back(nFocus);
			}
		}
	}
	else
	{
		int posSeek = 0;
		do
		{
			CString sSeek = sFocusSet.Mid(posSeek);
			int nFocus;
			if (1 <= swscanf_s((LPCTSTR)sSeek, _T("%d"), &nFocus))
			{
				SET.ui.vFocusSet.push_back(nFocus);
			}
		} while (-1 != (posSeek = sFocusSet.Find(_T(" "), posSeek + 1)));
	}

	if (SET.ui.vFocusSet.size() == 0)
	{
		SET.ui.vFocusSet.push_back(DFLT_FOCUS_STEP);
	}

	SET.ui.FOCUS_SET = sFocusSet;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqSetSnapFocusSet()
{
	CString sSnapFocusSet;
	GetDlgItemText(IDC_EDIT_SNAPFOCUS_SET, sSnapFocusSet);

	ASSERT(m_cur_ws < TWO_WS);
	auto& SET = VSET[m_cur_ws];

	SET.ui.vSnapFocusSet.clear();

	if (sSnapFocusSet.Find(_T("-")) != -1)
	{
		int nLow, nHigh, nUnit;
		if (3 <= swscanf_s((LPCTSTR)sSnapFocusSet, _T("%d-%d-%d"), &nLow, &nHigh, &nUnit))
		{
			for (int nFocus = nLow; nFocus <= nHigh; nFocus += nUnit)
			{
				SET.ui.vSnapFocusSet.push_back(nFocus);
			}
		}
	}
	else
	{
		int posSeek = 0;
		do
		{
			CString sSeek = sSnapFocusSet.Mid(posSeek);
			int nFocus;
			if (1 <= swscanf_s((LPCTSTR)sSeek, _T("%d"), &nFocus))
			{
				SET.ui.vSnapFocusSet.push_back(nFocus);
			}
		} while (-1 != (posSeek = sSnapFocusSet.Find(_T(" "), posSeek + 1)));
	}

	if (SET.ui.vSnapFocusSet.size() == 0)
	{
		SET.ui.vSnapFocusSet.push_back(DFLT_FOCUS_STEP);
	}

	ASSERT(m_cur_ws < TWO_WS);
	VSET[m_cur_ws].ui.SNAPFOCUS_SET = sSnapFocusSet;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqSetEdofResize()
{
	CString sEdofResize;
	GetDlgItemText(IDC_EDIT_EDOF_RESIZE_RATIO, sEdofResize);

	auto resize_ratio = _ttof(sEdofResize);

	double dbMin, dbMax;
	auto rv = SDOAQ_GetDblParameterRange(pi_edof_calc_resize_ratio, &dbMin, &dbMax);
	if (ecNoError == rv)
	{
		if (resize_ratio >= dbMin && resize_ratio <= dbMax)
		{
			//SET.efp.resize_ratio = resize_ratio;
			::SDOAQ_SetDblParameterValue(pi_edof_calc_resize_ratio, resize_ratio);
		}
		else
		{
			ApiError(FString(_T("Set EDoF resize ratio : value is out of range[%.2lf ~ %.2lf]"), dbMin, dbMax), ecUnknownError);
			return;
		}
	}
	else
	{
		ApiError(_T("SDOAQ_GetDblParameterRange [pi_edof_calc_resize_ratio]"), rv);
		return;
	}

	ASSERT(m_cur_ws < TWO_WS);
	VSET[m_cur_ws].ui.EDOF_RESIZE_RATIO = sEdofResize;
}

//============================================================================
// PLAY FUNCTIONs
//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqSingleShotStack()
{
	ASSERT(m_cur_ws < TWO_WS);
	auto& SET = VSET[m_cur_ws];

	if (m_cur_ws == ONLY_WS0_3D)
	{
		(void)::WSGL_Display_BG(m_vVW[m_cur_ws].hwnd_3d);
	}

	if (SET.rb.active)
	{
		return;
	}

	auto& AFP = SET.afp;
	auto& FOCUS = SET.focus;

	FOCUS.numsFocus = SET.ui.vFocusSet.size();
	FOCUS.vFocusSet.resize(FOCUS.numsFocus);
	copy(SET.ui.vFocusSet.begin(), SET.ui.vFocusSet.end(), FOCUS.vFocusSet.begin());

	int* pPositions = new int[FOCUS.numsFocus];
	unsigned char** ppFocusImages = new unsigned char*[FOCUS.numsFocus];
	size_t* pFocusImageBufferSizes = new size_t[FOCUS.numsFocus];

	for (size_t pos = 0; pos < FOCUS.numsFocus; pos++)
	{
		pPositions[pos] = FOCUS.vFocusSet[pos];

		auto size = SET.ImgSize();
		ppFocusImages[pos] = new unsigned char[size];
		pFocusImageBufferSizes[pos] = size;
	}

	const auto tick_begin = GetTickCount64();
	AFP.callbackUserData = (void*)::GetTickCount64();
	const LPCTSTR sz_api = _T("SDOAQ_SingleShotFocusStackEx");
	const eErrorCode rv = ::SDOAQ_SingleShotFocusStackEx(
		&AFP,
		pPositions, (int)FOCUS.numsFocus,
		ppFocusImages, pFocusImageBufferSizes
	);
	const auto tick_end = GetTickCount64();
	Log(FString(_T(">> %s() takes : %llu ms / %d imgs"), sz_api, tick_end - tick_begin, FOCUS.numsFocus));

	if (ecNoError == rv)
	{
		++SET.ui.nContiStack;
		auto& vw = m_vVW[m_cur_ws];
		for (size_t uid = 0; uid < vw.vhwnd_iv.size(); uid++)
		{
			if (uid < FOCUS.numsFocus)
			{
				auto pos = uid;
				if (uid == vw.vhwnd_iv.size() - 1)
				{
					// last window -> last position
					pos = FOCUS.numsFocus - 1;
				}
				WSIOCHAR title[256];
				sprintf_s(title, sizeof title, "Zstack(%d)", pPositions[pos]);
				ImageViewer(vw.vhwnd_iv[uid], title, SET.ui.nContiStack, SET, ppFocusImages[pos]);
			}
			else
			{
				ImageViewer(vw.vhwnd_iv[uid]);
			}
		}
	}
	else
	{
		ApiError(sz_api, rv);
	}

	delete[] pFocusImageBufferSizes;
	for (size_t pos = 0; pos < FOCUS.numsFocus; pos++)
	{
		delete[] ppFocusImages[pos];
	}
	delete[] ppFocusImages;
	delete[] pPositions;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqPlayStack()
{
	ASSERT(m_cur_ws < TWO_WS);
	auto& SET = VSET[m_cur_ws];

	if (m_cur_ws == ONLY_WS0_3D)
	{
		(void)::WSGL_Display_BG(m_vVW[m_cur_ws].hwnd_3d);
	}

	if (SET.rb.active)
	{
		return;
	}

	auto& AFP = SET.afp;
	auto& FOCUS = SET.focus;

	FOCUS.numsFocus = SET.ui.vFocusSet.size();
	FOCUS.vFocusSet.resize(FOCUS.numsFocus);
	copy(SET.ui.vFocusSet.begin(), SET.ui.vFocusSet.end(), FOCUS.vFocusSet.begin());

	int* pPositions = new int[FOCUS.numsFocus];
	for (size_t pos = 0; pos < FOCUS.numsFocus; pos++)
	{
		pPositions[pos] = FOCUS.vFocusSet[pos];
	}

	if (SET.rb.ppBuf)
	{
		SET.ClearBuffer();
	}
	SET.rb.numsBuf = FOCUS.numsFocus * SET.ui.nRingBufferSize;
	SET.rb.ppBuf = (void**)new unsigned char*[SET.rb.numsBuf];
	SET.rb.pSizes = new size_t[SET.rb.numsBuf];
	auto size = SET.ImgSize();

	for (size_t uidx = 0; uidx < SET.rb.numsBuf; uidx++)
	{
		SET.rb.ppBuf[uidx] = (void*)new unsigned char[size];
		SET.rb.pSizes[uidx] = size;
	}

	AFP.callbackUserData = (void*)::GetTickCount64();
	const LPCTSTR sz_api = _T("SDOAQ_PlayFocusStackEx");
	const eErrorCode rv = ::SDOAQ_PlayFocusStackEx(
		&AFP,
		g_PlayFocusStackCallbackEx,
		pPositions, (int)FOCUS.numsFocus,
		SET.ui.nRingBufferSize, (unsigned char**)SET.rb.ppBuf, SET.rb.pSizes);

	if (ecNoError == rv)
	{
		SET.rb.active = true;
	}
	else
	{
		ApiError(sz_api, rv);
	}

	delete[] pPositions;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqStopStack()
{
	ASSERT(m_cur_ws < TWO_WS);
	auto& SET = VSET[m_cur_ws];

	SET.rb.active = false;
	(void)::SDOAQ_StopFocusStack();

	SET.ClearBuffer();
}

//----------------------------------------------------------------------------
LRESULT CSDOAQ_Dlg::OnReceiveZstack(WPARAM wparam, LPARAM lLastFilledRingBufferEntry)
{
	bool flag_last_done[TWO_WS] = { false,false };
	auto const vmsg = RetrieveMessages(m_hWnd, EUM_RECEIVE_ZSTACK, wparam, lLastFilledRingBufferEntry);
	for (auto it = vmsg.rbegin(); it != vmsg.rend(); it++)
	{
		const auto vid = HIWORD(it->wParam);
		const auto ecode = LOWORD(it->wParam);
		const int rbufidx = (int)it->lParam;
		if (vid < TWO_WS && ecNoError == ecode)
		{
			auto& SET = VSET[vid];
			if (SET.rb.active && false == flag_last_done[vid])
			{
				flag_last_done[vid] = true;
				if (SET.ui.nRingBufferSize == 1)
				{
					SET.rb.active = false;
				}

				auto nFocusNums = SET.focus.numsFocus;
				const int base_order = (rbufidx % (int)SET.rb.numsBuf)*(int)nFocusNums; //SET.ui.nRingBufferSize		

				++SET.ui.nContiStack;
				auto& vw = m_vVW[vid];
				for (size_t uid = 0; uid < vw.vhwnd_iv.size(); uid++)
				{
					if (uid < nFocusNums)
					{
						auto pos = uid;
						if (uid == vw.vhwnd_iv.size() - 1)
						{
							// last window -> last position
							pos = nFocusNums - 1;
						}
						WSIOCHAR title[256];
						sprintf_s(title, sizeof title, "Zstack(%d)", SET.focus.vFocusSet[pos]);
						ImageViewer(vw.vhwnd_iv[uid], title, SET.ui.nContiStack, SET, SET.rb.ppBuf[base_order + uid]);
					}
					else
					{
						ImageViewer(vw.vhwnd_iv[uid]);
					}
				}
				if (vid == ONLY_WS0_3D)
				{
					(void)::WSGL_Display_BG(vw.hwnd_3d);
				}
			}
		}
		else
		{
			ApiError(_T("SDOAQ_PlayCallbackEx"), ecode);
		}
	}

	return 0;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqSingleShotEdof()
{
	ASSERT(m_cur_ws < TWO_WS);
	auto& SET = VSET[m_cur_ws];

	if (SET.rb.active)
	{
		return;
	}

	auto& AFP = SET.afp;
	auto& FOCUS = SET.focus;

	FOCUS.numsFocus = SET.ui.vFocusSet.size();
	FOCUS.vFocusSet.resize(FOCUS.numsFocus);
	copy(SET.ui.vFocusSet.begin(), SET.ui.vFocusSet.end(), FOCUS.vFocusSet.begin());

	int* pPositions = new int[FOCUS.numsFocus];
	for (size_t pos = 0; pos < FOCUS.numsFocus; pos++)
	{
		pPositions[pos] = FOCUS.vFocusSet[pos];
	}

	float* pStepMapImageBuffer = new float[SET.PixelSize()];
	unsigned char* pEdofImageBuffer = new unsigned char[SET.ImgSize()];
	float* pQualityMapBuffer = new float[SET.PixelSize()];
	float* pHeightMapBuffer = new float[SET.PixelSize()];
	float* pPointCloudBuffer = new float[SET.PixelSize() * 3];

	size_t stepMapBufferSize = SET.DataSize();
	size_t edofImageBufferSize = SET.ImgSize();
	size_t qualityMapBufferSize = SET.DataSize();
	size_t heightMapBufferSize = SET.DataSize();
	size_t pointCloudBufferSize = SET.DataSize() * 3;

	if (((CButton*)GetDlgItem(IDC_CHECK_STEPMAP))->GetCheck() == BST_UNCHECKED)
	{
		stepMapBufferSize = 0;
	}
	if (((CButton*)GetDlgItem(IDC_CHECK_EDOF))->GetCheck() == BST_UNCHECKED)
	{
		edofImageBufferSize = 0;
	}
	if (((CButton*)GetDlgItem(IDC_CHECK_QUALITYMAP))->GetCheck() == BST_UNCHECKED)
	{
		qualityMapBufferSize = 0;
	}
	if (((CButton*)GetDlgItem(IDC_CHECK_HEIGHTMAP))->GetCheck() == BST_UNCHECKED)
	{
		heightMapBufferSize = 0;
	}
	if (((CButton*)GetDlgItem(IDC_CHECK_POINTCLOUD))->GetCheck() == BST_UNCHECKED)
	{
		pointCloudBufferSize = 0;
	}

	const auto tick_begin = GetTickCount64();
	AFP.callbackUserData = (void*)::GetTickCount64();
	const LPCTSTR sz_api = _T("SDOAQ_SingleShotEdofEx");
	const eErrorCode rv = ::SDOAQ_SingleShotEdofEx(
		&AFP,
		pPositions, (int)FOCUS.numsFocus,
		pStepMapImageBuffer, stepMapBufferSize,
		pEdofImageBuffer, edofImageBufferSize,
		pQualityMapBuffer, qualityMapBufferSize,
		pHeightMapBuffer, heightMapBufferSize,
		pPointCloudBuffer, pointCloudBufferSize
	);	
	const auto tick_end = GetTickCount64();
	Log(FString(_T(">> %s() takes : %llu ms / %d imgs"), sz_api, tick_end - tick_begin, FOCUS.numsFocus));

	if (ecNoError == rv)
	{
		if (m_cur_ws < TWO_WS)
		{
			++SET.ui.nContiEdof;
			auto& vw = m_vVW[m_cur_ws];
			if (pEdofImageBuffer && edofImageBufferSize)
			{
				ImageViewer(vw.vhwnd_iv[0], "EDoF", SET.ui.nContiEdof, SET, pEdofImageBuffer);
			}
			else
			{
				ImageViewer(vw.vhwnd_iv[0], "EDoF", SET.ui.nContiEdof);
			}
			FloatViewer(pStepMapImageBuffer && stepMapBufferSize, vw.vhwnd_iv[1], "StepMAP", SET.ui.nContiEdof, SET, pStepMapImageBuffer);
			//FloatViewer(pQualityMapBuffer && qualityMapBufferSize, vw.vhwnd_iv[2], "QualityMAP", SET.ui.nContiEdof, SET, pQualityMapBuffer);
			FloatViewer(pHeightMapBuffer && heightMapBufferSize, vw.vhwnd_iv[2], "HeightMAP", SET.ui.nContiEdof, SET, pHeightMapBuffer);
			if (m_cur_ws == ONLY_WS0_3D)
			{
				Viewer3D(pPointCloudBuffer && pointCloudBufferSize, vw.hwnd_3d, SET, pPointCloudBuffer, pEdofImageBuffer);
			}
		}
	}
	else
	{
		ApiError(sz_api, rv);
	}

	delete[] pStepMapImageBuffer;
	delete[] pEdofImageBuffer;
	delete[] pQualityMapBuffer;
	delete[] pHeightMapBuffer;
	delete[] pPointCloudBuffer;
	delete[] pPositions;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqPlayEdof()
{
	ASSERT(m_cur_ws < TWO_WS);
	auto& SET = VSET[m_cur_ws];

	if (SET.rb.active)
	{
		return;
	}

	auto& AFP = SET.afp;
	auto& FOCUS = SET.focus;

	auto nFocusNums = SET.ui.vFocusSet.size();
	FOCUS.vFocusSet.resize(FOCUS.numsFocus);
	copy(SET.ui.vFocusSet.begin(), SET.ui.vFocusSet.end(), FOCUS.vFocusSet.begin());

	int* pPositions = new int[nFocusNums];
	for (size_t pos = 0; pos < nFocusNums; pos++)
	{
		pPositions[pos] = FOCUS.vFocusSet[pos];
	}

	if (SET.rb.ppBuf)
	{
		SET.ClearBuffer();
	}

	size_t edofImageBufferSize = SET.ImgSize();
	size_t stepMapBufferSize = SET.DataSize();
	size_t qualityMapBufferSize = SET.DataSize();
	size_t heightMapBufferSize = SET.DataSize();
	size_t pointCloudBufferSize = SET.DataSize() * 3;

	if (((CButton*)GetDlgItem(IDC_CHECK_STEPMAP))->GetCheck() == BST_UNCHECKED)
	{
		stepMapBufferSize = 0;
	}
	if (((CButton*)GetDlgItem(IDC_CHECK_EDOF))->GetCheck() == BST_UNCHECKED)
	{
		edofImageBufferSize = 0;
	}
	if (((CButton*)GetDlgItem(IDC_CHECK_QUALITYMAP))->GetCheck() == BST_UNCHECKED)
	{
		qualityMapBufferSize = 0;
	}
	if (((CButton*)GetDlgItem(IDC_CHECK_HEIGHTMAP))->GetCheck() == BST_UNCHECKED)
	{
		heightMapBufferSize = 0;
	}
	if (((CButton*)GetDlgItem(IDC_CHECK_POINTCLOUD))->GetCheck() == BST_UNCHECKED)
	{
		pointCloudBufferSize = 0;
	}

	SET.rb.numsBuf = RBE2O_NUMS * SET.ui.nRingBufferSize;
	SET.rb.ppBuf = (void**)new unsigned char*[SET.rb.numsBuf];
	SET.rb.pSizes = new size_t[SET.rb.numsBuf];

	for (size_t uidx = 0; uidx + RBE2O_NUMS - 1 < SET.rb.numsBuf; uidx += RBE2O_NUMS)
	{
		SET.rb.ppBuf[uidx + RBE2O_EDOF] = edofImageBufferSize ? (void*)new unsigned char[edofImageBufferSize] : NULL;
		SET.rb.pSizes[uidx + RBE2O_EDOF] = edofImageBufferSize;

		SET.rb.ppBuf[uidx + RBE2O_SMAP] = stepMapBufferSize ? (void*)new float[stepMapBufferSize / sizeof(float)] : NULL;
		SET.rb.pSizes[uidx + RBE2O_SMAP] = stepMapBufferSize;

		SET.rb.ppBuf[uidx + RBE2O_QMAP] = qualityMapBufferSize ? (void*)new float[qualityMapBufferSize / sizeof(float)] : NULL;
		SET.rb.pSizes[uidx + RBE2O_QMAP] = qualityMapBufferSize;

		SET.rb.ppBuf[uidx + RBE2O_HMAP] = heightMapBufferSize ? (void*)new float[heightMapBufferSize / sizeof(float)] : NULL;
		SET.rb.pSizes[uidx + RBE2O_HMAP] = heightMapBufferSize;

		SET.rb.ppBuf[uidx + RBE2O_PCLOUD] = pointCloudBufferSize ? (void*)new float[pointCloudBufferSize / sizeof(float)] : NULL;
		SET.rb.pSizes[uidx + RBE2O_PCLOUD] = pointCloudBufferSize;
	}

	AFP.callbackUserData = (void*)::GetTickCount64();
	const LPCTSTR sz_api = _T("SDOAQ_PlayEdofEx");
	const eErrorCode rv = ::SDOAQ_PlayEdofEx(
		&AFP,
		g_PlayEdofCallbackEx,
		pPositions, (int)nFocusNums,
		SET.ui.nRingBufferSize,
		SET.rb.ppBuf,
		SET.rb.pSizes
	);	
	if (ecNoError == rv)
	{
		SET.rb.active = true;
	}
	else
	{
		ApiError(sz_api, rv);
	}

	delete[] pPositions;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqStopEdof()
{
	ASSERT(m_cur_ws < TWO_WS);
	auto& SET = VSET[m_cur_ws];

	SET.rb.active = false;
	(void)::SDOAQ_StopEdof();

	SET.ClearBuffer();
}

//----------------------------------------------------------------------------
LRESULT CSDOAQ_Dlg::OnReceiveEdof(WPARAM wparam, LPARAM lLastFilledRingBufferEntry)
{
	bool flag_last_done[TWO_WS] = { false,false };
	auto const vmsg = RetrieveMessages(m_hWnd, EUM_RECEIVE_EDOF, wparam, lLastFilledRingBufferEntry);
	for (auto it = vmsg.rbegin(); it != vmsg.rend(); it++)
	{
		const auto vid = HIWORD(it->wParam);
		const auto ecode = LOWORD(it->wParam);
		const int rbufidx = (int)it->lParam;
		if (vid < TWO_WS && ecNoError == ecode)
		{
			auto& SET = VSET[vid];
			if (SET.rb.active && false == flag_last_done[vid])
			{
				flag_last_done[vid] = true;
				if (SET.ui.nRingBufferSize == 1)
				{
					SET.rb.active = false;
				}

				const int base_order = (rbufidx % (int)SET.rb.numsBuf)*RBE2O_NUMS; //SET.ui.nRingBufferSize

				++SET.ui.nContiEdof;
				auto& vw = m_vVW[vid];
				ImageViewer(vw.vhwnd_iv[0], "EDoF", SET.ui.nContiEdof, SET, SET.rb.ppBuf[base_order + RBE2O_EDOF]);
				FloatViewer(true, vw.vhwnd_iv[1], "StepMAP", SET.ui.nContiEdof, SET, SET.rb.ppBuf[base_order + RBE2O_SMAP]);
				//FloatViewer(true, vw.vhwnd_iv[2], "QualityMAP", SET.ui.nContiEdof, SET, SET.rb.ppBuf[base_order + RBE2O_QMAP]);
				FloatViewer(true, vw.vhwnd_iv[2], "HeightMAP", SET.ui.nContiEdof, SET, SET.rb.ppBuf[base_order + RBE2O_HMAP]);
				if (vid == ONLY_WS0_3D)
				{
					Viewer3D(true, vw.hwnd_3d, SET, SET.rb.ppBuf[base_order + RBE2O_PCLOUD], SET.rb.ppBuf[base_order + RBE2O_EDOF]);
				}
			}
		}
		else
		{
			ApiError(_T("SDOAQ_PlayCallbackEx"), ecode);
		}
	}

	return 0;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqSingleShotAF()
{
	ASSERT(m_cur_ws < TWO_WS);
	auto& SET = VSET[m_cur_ws];

	if (m_cur_ws == ONLY_WS0_3D)
	{
		(void)::WSGL_Display_BG(m_vVW[m_cur_ws].hwnd_3d);
	}

	if (SET.rb.active)
	{
		return;
	}

	auto& AFP = SET.afp;
	auto& FOCUS = SET.focus;

	FOCUS.numsFocus = SET.ui.vFocusSet.size();
	FOCUS.vFocusSet.resize(FOCUS.numsFocus);
	copy(SET.ui.vFocusSet.begin(), SET.ui.vFocusSet.end(), FOCUS.vFocusSet.begin());

	int* pPositions = new int[FOCUS.numsFocus];
	for (size_t pos = 0; pos < FOCUS.numsFocus; pos++)
	{
		pPositions[pos] = SET.ui.vFocusSet[pos];
	}

	size_t AFImageBufferSize = SET.ImgSize();
	unsigned char* pAFImageBuffer = new unsigned char[AFImageBufferSize];
	double dbBestFocusStep;
	double dbScore;
	double dbMatchedFocusStep;

	AFP.callbackUserData = (void*)::GetTickCount64();
	const LPCTSTR sz_api = _T("SDOAQ_SingleShotAFEx");
	const eErrorCode rv = ::SDOAQ_SingleShotAFEx(
		&AFP,
		pPositions, (int)FOCUS.numsFocus,
		pAFImageBuffer,
		AFImageBufferSize,
		&dbBestFocusStep,
		&dbScore,
		&dbMatchedFocusStep
	);
	if (ecNoError == rv)
	{
		if (m_cur_ws < TWO_WS)
		{
			++SET.ui.nContiAF;
			auto& vw = m_vVW[m_cur_ws];
			if (pAFImageBuffer && AFImageBufferSize)
			{
				ImageViewer(vw.vhwnd_iv[0], "AF", SET.ui.nContiAF, SET, pAFImageBuffer);
				Log(FString(_T(">> Best focus step : %.4lf, Score : %.4lf, Matched step : %d"), dbBestFocusStep, dbScore, (int)(dbMatchedFocusStep + 0.5)));
			}
			else
			{
				ImageViewer(vw.vhwnd_iv[0], "AF", SET.ui.nContiAF);
			}
		}
	}
	else
	{
		ApiError(sz_api, rv);
	}

	delete[] pAFImageBuffer;
	delete[] pPositions;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqPlayAF()
{
	ASSERT(m_cur_ws < TWO_WS);
	auto& SET = VSET[m_cur_ws];

	if (m_cur_ws == ONLY_WS0_3D)
	{
		(void)::WSGL_Display_BG(m_vVW[m_cur_ws].hwnd_3d);
	}

	if (SET.rb.active)
	{
		return;
	}

	auto& AFP = SET.afp;
	auto& FOCUS = SET.focus;

	FOCUS.numsFocus = SET.ui.vFocusSet.size();
	FOCUS.vFocusSet.resize(FOCUS.numsFocus);
	copy(SET.ui.vFocusSet.begin(), SET.ui.vFocusSet.end(), FOCUS.vFocusSet.begin());

	int* pPositions = new int[FOCUS.numsFocus];
	for (size_t pos = 0; pos < FOCUS.numsFocus; pos++)
	{
		pPositions[pos] = SET.ui.vFocusSet[pos];
	}

	if (SET.rb.ppBuf)
	{
		SET.ClearBuffer();
	}

	SET.rb.numsBuf = SET.ui.nRingBufferSize;
	SET.rb.ppBuf = (void**)new unsigned char*[SET.rb.numsBuf];
	SET.rb.pSizes = new size_t[SET.rb.numsBuf];

	for (size_t uidx = 0; uidx < SET.rb.numsBuf;)
	{
		SET.rb.ppBuf[uidx] = (void*)new unsigned char[SET.ImgSize()];
		SET.rb.pSizes[uidx] = SET.ImgSize();
		uidx++;
	}

	AFP.callbackUserData = (void*)::GetTickCount64();
	const LPCTSTR sz_api = _T("SDOAQ_PlayAFEx");
	const eErrorCode rv = ::SDOAQ_PlayAFEx(
		&AFP,
		g_PlayAFCallbackEx2,
		pPositions, (int)FOCUS.numsFocus,
		SET.ui.nRingBufferSize,
		SET.rb.ppBuf,
		SET.rb.pSizes
	);
	if (ecNoError == rv)
	{
		SET.rb.active = true;
	}
	else
	{
		ApiError(sz_api, rv);
	}

	delete[] pPositions;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqStopAF()
{
	ASSERT(m_cur_ws < TWO_WS);
	auto& SET = VSET[m_cur_ws];

	SET.rb.active = false;
	(void)::SDOAQ_StopAF();

	SET.ClearBuffer();
}

//----------------------------------------------------------------------------
LRESULT CSDOAQ_Dlg::OnReceiveAF(WPARAM wparam, LPARAM lMsgParaReceiveAf)
{
	bool flag_last_done[TWO_WS] = { false,false };
	auto const vmsg = RetrieveMessages(m_hWnd, EUM_RECEIVE_AF, wparam, lMsgParaReceiveAf);
	for (auto it = vmsg.rbegin(); it != vmsg.rend(); it++)
	{
		const auto vid = HIWORD(it->wParam);
		const auto ecode = LOWORD(it->wParam);
		auto prx = (tMsgParaReceiveAf*)it->lParam;
		if (vid < TWO_WS && ecNoError == ecode && prx)
		{
			auto& SET = VSET[vid];
			if (SET.rb.active && false == flag_last_done[vid])
			{
				flag_last_done[vid] = true;
				if (SET.ui.nRingBufferSize == 1)
				{
					SET.rb.active = false;
				}

				tMsgParaReceiveAf ParaAF = *prx;
				const int base_order = (ParaAF.lastFilledRingBufferEntry % (int)SET.rb.numsBuf);

				++SET.ui.nContiAF;
				auto& vw = m_vVW[vid];
				ImageViewer(vw.vhwnd_iv[0], "AF", SET.ui.nContiAF, SET, (BYTE*)SET.rb.ppBuf[base_order + 0]);
				Log(FString(_T(">> Best focus step : %.4lf,\tScore : %.4lf"), ParaAF.dbBestFocusStep, ParaAF.dbScore));
				if (vid == ONLY_WS0_3D)
				{
					(void)::WSGL_Display_BG(vw.hwnd_3d);
				}
			}
			if (prx)
			{
				delete prx;
			}
		}
		else
		{
			ApiError(_T("SDOAQ_PlayAfCallbackEx2"), ecode);
		}
	}

	return 0;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqSnap()
{
	ASSERT(m_cur_ws < TWO_WS);
	auto& SET = VSET[m_cur_ws];

	if (SET.rb.active)
	{
		auto nFocusNums = SET.ui.vSnapFocusSet.size();
		int* pPositions = new int[nFocusNums];
		for (int pos = 0; pos < nFocusNums; pos++)
		{
			pPositions[pos] = SET.ui.vSnapFocusSet[pos];
		}

		CString sCurrentTime;
		sCurrentTime.Format(CTime::GetCurrentTime().Format("%Y.%h.%d. %Hh %Mm %Ss"));
		CString sSnapPath;
		sSnapPath.Format(_T("%s\\Snap\\%s"), GetCurrentDir(), sCurrentTime);

		CStringA sSnapPathA = (CStringA)sSnapPath;
		SnapParameters snap_para;
		snap_para.version = (void*)2;
		snap_para.v2.sSnapPath = sSnapPathA;
		snap_para.v2.sConfigFilename = NULL;
		snap_para.v2.sConfigData = NULL;

		const auto callbackUserData = (void*)::GetTickCount64();
		::SDOAQ_PlaySnapEx(g_SnapCallbackEx, callbackUserData, pPositions, (int)nFocusNums, &snap_para);

		delete[] pPositions;
	}
	else
	{
		Log(_T(">> Snap is only valid while acquisition is in progress!"));
	}
}

//------------------------------------------------------------------------------------------------
LRESULT CSDOAQ_Dlg::OnReceiveSnap(WPARAM wparam, LPARAM lLastFilledRingBufferEntry)
{
	bool flag_last_done[TWO_WS] = { false,false };
	auto const vmsg = RetrieveMessages(m_hWnd, EUM_RECEIVE_SNAP, wparam, lLastFilledRingBufferEntry);
	for (auto it = vmsg.rbegin(); it != vmsg.rend(); it++)
	{
		const auto vid = HIWORD(it->wParam);
		const auto ecode = LOWORD(it->wParam);
		const int rbufidx = (int)it->lParam;
		if (vid < TWO_WS && ecNoError == ecode)
		{
			auto& SET = VSET[vid];
			if (SET.rb.active && false == flag_last_done[vid])
			{
				flag_last_done[vid] = true;
				;
			}
		}
		else
		{
			ApiError(_T("SDOAQ_SnapCallbackEx"), ecode);
		}
	}

	return 0;
}

//----------------------------------------------------------------------------
// Note 1: Set up the calibration file after initialization is complete.
// Note 2: Currently, only one calibration data is valid for multiple wisescope.
//		In other words, when calibration data is set, it is applied to all wisescope.
//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqSetCalibrationFile()
{
	CString sFilter = _T("calibration file (*.csv)|*.csv|");
	CFileDialog dlg(TRUE, _T("cvs"), NULL, OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT, sFilter);
	dlg.m_ofn.lpstrInitialDir = GetCurrentDir();
	if (dlg.DoModal() == IDOK)
	{
		(void)::SDOAQ_SetCalibrationFile(CT2A(dlg.GetPathName().GetBuffer()));
		Log(FString(_T(">> Calibration file [%s] is set."), dlg.GetPathName()));
	}
}

//----------------------------------------------------------------------------
// Note 1: Set up the calibration file after initialization is complete.
// Note 2: Currently, only one calibration data is valid for multiple wisescope.
//		In other words, when calibration data is set, it is applied to all wisescope.
//----------------------------------------------------------------------------
void CSDOAQ_Dlg::OnSdoaqComboObjective()
{
	CString sObjective;
	((CComboBox*)GetDlgItem(IDC_COMBO_OBJECTIVE))->GetLBText(((CComboBox*)GetDlgItem(IDC_COMBO_OBJECTIVE))->GetCurSel(), sObjective);

	// Set calibration data of the selected objective lens
	for (auto it_data = m_calFile.calibData.begin(); it_data != m_calFile.calibData.end(); it_data++)
	{
		auto& list = *it_data;
		if (0 == sObjective.Compare(list.objective))
		{
			const auto size = (int)list.calibTable.size();

			if (size == 0)
			{
				continue;
			}
			auto height = new double[size];
			auto pitchX = new double[size];
			auto pitchY = new double[size];
			auto scaleX = new double[size];
			auto scaleY = new double[size];
			auto shiftX = new double[size];
			auto shitfY = new double[size];

			int index = 0;
			for (auto it_table = list.calibTable.begin(); it_table != list.calibTable.end(); it_table++)
			{
				height[index] = it_table->obj_height;
				pitchX[index] = it_table->pixel_pitch_x;
				pitchY[index] = it_table->pixel_pitch_y;
				scaleX[index] = it_table->scale_x;
				scaleY[index] = it_table->scale_y;
				shiftX[index] = it_table->shift_x;
				shitfY[index] = it_table->shift_y;
				index++;
			}

			(void)::SDOAQ_SetExternalCalibrationTable(size,
				height, pitchX, pitchY,
				scaleX, scaleY, shiftX, shitfY,
				list.fieldCurvatureCoefs);

			delete[] height;
			delete[] pitchX;
			delete[] pitchY;
			delete[] scaleX;
			delete[] scaleY;
			delete[] shiftX;
			delete[] shitfY;

			auto xRange = (list.calibTable[index / 2].pixel_pitch_x * nMaxWidth) / 2;
			dxRangeStart = -xRange; dxRangeEnd = xRange;
			auto yRange = (list.calibTable[index / 2].pixel_pitch_y * nMaxHeight) / 2;
			dyRangeStart = -yRange; dyRangeEnd = yRange;
			dzRangeStart = list.calibTable[index - 1].obj_height; dzRangeEnd = list.calibTable[0].obj_height;

			break;
		}
	}
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::print_wsio_last_error(void)
{
	WSIOCHAR sLastError[4 * 1024];
	::WSIO_LastErrorString(sLastError, sizeof sLastError);
	Log(FString(_T("[WSIO ERROR] %s"), (CString)sLastError));
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::print_wsgl_last_error(HWND hwnd_3d)
{
	CString* sLastError = NewWString(::WSGL_GetLastErrorString(hwnd_3d));
	Log(FString(_T("[WSGL ERROR] %s"), (CString)*sLastError));
	delete sLastError;
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::ImageViewer(HWND hwnd_viewer, const char* title, int title_no
	, const tTestSet& SET, void* data)
{
	ImageViewer(hwnd_viewer, title, title_no
		, SET.afp.cameraRoiWidth, SET.afp.cameraRoiHeight, SET.m_nColorByte, data);
}

void CSDOAQ_Dlg::ImageViewer(HWND hwnd_viewer, const char* title, int title_no
	, int width, int height, int colorbytes, void* data)
{
	WSIOCHAR full_title[256] = { 0 };
	if (title)
	{
		sprintf_s(full_title, sizeof full_title, "%s %d", title, title_no);
	}

	const unsigned size = (data ? width * height * colorbytes : 0);
	if (WSIORV_SUCCESS > ::WSUT_IV_AttachRawImgData_V2(hwnd_viewer, width, height, width*colorbytes, colorbytes, data, size, full_title))
	{
		print_wsio_last_error();
	}
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::FloatViewer(bool onoff, HWND hwnd_viewer, const char* title, int title_no
	, const tTestSet& SET, void* data)
{
	if (onoff && data)
	{
		auto pSeek = (float*)data;
		auto low = *pSeek;
		auto high = *pSeek;

		const unsigned pixels = SET.PixelSize();
		for (unsigned uSampling = 0; uSampling < pixels; uSampling += 100)
		{
			float v = *(pSeek + uSampling);
			if (v < low)
			{
				low = v;
			}
			if (high < v)
			{
				high = v;
			}
		}
		const auto inc = (float)256 / (high - low);

		auto const pImg = new unsigned char[SET.PixelSize()];
		auto pDest = pImg;
		auto pSrc = (float*)data;
		for (unsigned loop = 0; loop < pixels; loop++)
		{
			*pDest++ = (const unsigned char)((*pSrc++ - low)*inc);
		}

		ImageViewer(hwnd_viewer, title, title_no, SET.afp.cameraRoiWidth, SET.afp.cameraRoiHeight, MONOBYTES, pImg);

		delete[] pImg;
	}
	else
	{
		ImageViewer(hwnd_viewer, title, title_no);
	}
}

//----------------------------------------------------------------------------
void CSDOAQ_Dlg::Viewer3D(bool onoff, HWND hwnd_3d, const tTestSet& SET, void* p_pointcloud, void* p_edof)
{
	if (onoff && p_pointcloud && p_edof)
	{
		tPara_Display25D Para;
		Para.width = SET.PixelWidth();
		Para.height = SET.PixelHeight();
		Para.z_offset1 = dzRangeStart;
		Para.z_offset2 = dzRangeEnd;
		Para.z_slices = (unsigned)SET.ui.vFocusSet.size();
		Para.scx1 = dxRangeStart;
		Para.scx2 = dxRangeEnd;
		Para.scy1 = dyRangeStart;
		Para.scy2 = dyRangeEnd;
		Para.scz1 = dzRangeStart;
		Para.scz2 = dzRangeEnd;

		(void)::WSGL_Display_25D(hwnd_3d, GL_MG_ONSTAGE, "main", (float*)p_pointcloud, SET.PixelSize() * XYZNUMS, p_edof, SET.ImgSize()
			, EDM_BGR_BYTE | EDM_DIMENSION_CALXY_25D | EDM_NDC_XY_ONLY, 1.0f, &Para);
	}
	else
	{
		(void)::WSGL_Display_BG(hwnd_3d);
	}
}