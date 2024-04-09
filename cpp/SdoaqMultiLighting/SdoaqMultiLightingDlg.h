
// SdoaqMultiLightingDlg.h : header file
//

#pragma once

#include "SDOAQ_Lib.h"

//----------------------------------------------------------------------------
enum EUserMessage
{
	EUM_LOG = 0xA000, // LPARAM is a pointer of CString
	EUM_ERROR, // LPARAM is a pointer of CString
	EUM_INITDONE,
	EUM_RECEIVE_ZSTACK,
};

class CSdoaqMultiLightingDlg : public CDialogEx
{
public:
	CSdoaqMultiLightingDlg(CWnd* pParent = nullptr);	// standard constructor

protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	afx_msg void OnClose();
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnBtnInitialize();
	afx_msg void OnBtnFinalize();
	afx_msg void OnCbnSelLighting();
	afx_msg void OnBtnSet();
	afx_msg void OnCbnSelActiveLighting();
	afx_msg void OnBtnAcqStack();

	afx_msg LRESULT OnInitDone(WPARAM wErrorCode, LPARAM lpMessage);

private:
	void BuildLightingList();
	void BuildAcquisitionSet();

public:
	struct tTestSet
	{
		tTestSet()
		{
			afp.cameraRoiTop = 0;
			afp.cameraRoiLeft = 0;
			afp.cameraRoiWidth = (2040 / 4) * 4;
			afp.cameraRoiHeight = 1086;
			afp.cameraBinning = 1;
		}
		~tTestSet()
		{
		}

		AcquisitionFixedParametersEx afp;

		inline int PixelSize(void) const { return afp.cameraRoiWidth * afp.cameraRoiHeight; }
		inline int ImgSize(void) const { return IsMonoCameraInstalled() ? PixelSize() : PixelSize() * COLORBYTES; }

		struct tFocus
		{
			size_t numsFocus = 10;
			std::vector<int> vFocusSet;
		} focus;

		int m_nColorByte = COLORBYTES;
	} SET;

	void ImageViewer(const char* title = NULL, int title_no = 0, const tTestSet& SET = tTestSet(), void* data = NULL);
	void ImageViewer(const char* title, int title_no, int width, int height, int colorbytes, void* data = NULL);
};
