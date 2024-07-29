#pragma once


class CSdoaqMultiCameraFrameCallbackDlg : public CDialogEx
{
public:
	CSdoaqMultiCameraFrameCallbackDlg(CWnd* pParent = nullptr);

protected:
	virtual void DoDataExchange(CDataExchange* pDX);

protected:
	HICON m_hIcon;

	virtual BOOL OnInitDialog();
	DECLARE_MESSAGE_MAP()
public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	afx_msg void OnClose();
	afx_msg void OnSize(UINT nType, int cx, int cy);

	//----------------------------------------------------------------------------
	// function for wisescope 1
	//----------------------------------------------------------------------------
	afx_msg void OnBnClickedSwTrigger_ws1();
	afx_msg void OnSetFov_ws1();
	afx_msg void OnSetExposureTime_ws1();
	afx_msg void OnSetGain_ws1();
	afx_msg void OnSetWhitebalance_ws1();


	//----------------------------------------------------------------------------
	// function for wisescope 2
	//----------------------------------------------------------------------------
	afx_msg void OnBnClickedSwTrigger_ws2();
	afx_msg void OnSetFov_ws2();
	afx_msg void OnSetExposureTime_ws2();
	afx_msg void OnSetGain_ws2();
	afx_msg void OnSetWhitebalance_ws2();

	// currently selected WiseScope ID
	unsigned m_cur_ws = MAXINT;
	void UpdateWsSelection(int newWSId);
};
