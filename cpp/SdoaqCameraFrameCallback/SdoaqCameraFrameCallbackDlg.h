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
	afx_msg void OnClose();
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnBnClickedCheckFrameCallback();
	afx_msg void OnBnClickedSwTrigger();
	afx_msg void OnBnClickedTriggerFreerun();
	afx_msg void OnBnClickedTriggerSoftware();
	afx_msg void OnBnClickedTriggerExternal();
};
