#pragma once


class CSdoaqCameraFrameCallbackDlg : public CDialogEx
{
public:
	CSdoaqCameraFrameCallbackDlg(CWnd* pParent = nullptr);

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
	afx_msg void OnBnClickedCheckFrameCallback();
	afx_msg void OnBnClickedSwTrigger();
	afx_msg void OnBnClickedTriggerMode(UINT uID);

	afx_msg void OnSetFov();
	afx_msg void OnSetExposureTime();
	afx_msg void OnSetGain();
	afx_msg void OnSetWhitebalance();

	afx_msg void OnSetStringRegister();
	afx_msg void OnSetIntegerRegister();
	afx_msg void OnSetDoubleRegister();
	afx_msg void OnSetBoolRegister();
};
