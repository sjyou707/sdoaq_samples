
// SdoaqEdofDlg.h : header file
//

#pragma once

#include "SDOAQ_Lib.h"

//----------------------------------------------------------------------------
enum EUserMessage
{
	EUM_LOG = 0xA000, // LPARAM is a pointer of CString
	EUM_ERROR, // LPARAM is a pointer of CString
	EUM_INITDONE,
	EUM_RECEIVE_EDOF,
};

// CSdoaqEdofDlg dialog
class CSdoaqEdofDlg : public CDialogEx
{
// Construction
public:
	CSdoaqEdofDlg(CWnd* pParent = nullptr);	// standard constructor

// Dialog Data
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_SDOAQEDOF_DIALOG };
#endif

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
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnClose();

	afx_msg LRESULT OnInitDone(WPARAM wErrorCode, LPARAM lpMessage);
	afx_msg LRESULT OnReceiveEdof(WPARAM wErrorCode, LPARAM lLastFilledRingBufferEntry);

	afx_msg void OnSdoaqSetCalibrationFile();
	afx_msg void OnSdoaqSetROI();
	afx_msg void OnSdoaqSetFocusSet();
	afx_msg void OnSdoaqSetEdofResize();
	afx_msg void OnSdoaqSetEdofKernelSize();
	afx_msg void OnSdoaqSetEdofIteration();
	afx_msg void OnSdoaqSetEdofThreshold();
	afx_msg void OnSdoaqSingleShotEdof();
	afx_msg void OnSdoaqPlayEdof();
	afx_msg void OnSdoaqStopEdof();

public:
	struct tTestSet
	{
		tTestSet()
		{
			afp.cameraRoiTop = 0;
			afp.cameraRoiLeft = 0;
			afp.cameraRoiWidth = (2064 / 4) * 4;
			afp.cameraRoiHeight = 1544;
			afp.cameraBinning = 1;
		}
		~tTestSet()
		{
			ClearBuffer();
		}
		void ClearBuffer(void)
		{
			if (rb.ppBuf)
			{
				for (size_t uidx = 0; uidx < rb.numsBuf; uidx++)
				{
					delete[] rb.ppBuf[uidx];
				}
				delete[] rb.ppBuf; rb.ppBuf = NULL;
				delete[] rb.pSizes; rb.pSizes = NULL;
				rb.numsBuf = 0;
			}
		}

		sAcquisitionFixedParameters afp;

		inline int PixelSize(void) const { return afp.cameraRoiWidth * afp.cameraRoiHeight; }
		inline int ImgSize(void) const { return IsMonoCameraInstalled() ? PixelSize() : PixelSize() * COLORBYTES; }
		inline int DataSize(void) const { return PixelSize() * sizeof(float); }
		inline int PixelWidth(void) const { return afp.cameraRoiWidth; }
		inline int PixelHeight(void) const { return afp.cameraRoiHeight; }

		struct tRingBuf
		{
			bool active = false;
			void** ppBuf = NULL; // 데이타(이미지) 버퍼 포인터
			size_t* pSizes = NULL; // 각 데이타 버퍼의 데이타 크기(이미지 크기) 배열
			size_t numsBuf = 0; // 링버퍼 배열 요소 개수 * 포커스 개수 ==> 링 버퍼의 실제 데이타 개수
		} rb;

		struct tFocus
		{
			size_t numsFocus = 10;
			std::vector<int> vFocusSet;
		} focus;

		int m_nColorByte = COLORBYTES;
	} SET;


	std::vector<int> m_vFocusSet;
	int m_nRingBufferSize = 3;
	int m_nContiEdof = 0;

	void ImageViewer(const char* title = NULL, int title_no = 0, const tTestSet& SET = tTestSet(), void* data = NULL);
	void ImageViewer(const char* title, int title_no, int width, int height, int colorbytes, void* data = NULL);
};
