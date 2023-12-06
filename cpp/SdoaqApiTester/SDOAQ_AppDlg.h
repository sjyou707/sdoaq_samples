
// SDOAQ_AppDlg.h : header file
//

#pragma once

#include "SDOAQ_Lib.h"
#include "SDOAQ_CalibrationFile.h"

//----------------------------------------------------------------------------
enum EUserMessage
{
	EUM_LOG = 0xA000, // LPARAM is a pointer of CString
	EUM_ERROR, // LPARAM is a pointer of CString
	EUM_INITDONE,
	EUM_RECEIVE_ZSTACK,
	EUM_RECEIVE_EDOF,
	EUM_RECEIVE_AF,
	EUM_RECEIVE_SNAP,
};

struct tMsgParaReceiveAf
{
	int lastFilledRingBufferEntry;
	double dbBestFocusStep;
	double dbScore;
	double dbMatchedStep;
};

//============================================================================
class CSDOAQ_Dlg : public CDialogEx
{
public:
	CSDOAQ_Dlg(CWnd* pParent = nullptr);	// standard constructor
	virtual ~CSDOAQ_Dlg();

protected:
	virtual BOOL PreTranslateMessage(MSG* pMsg);

protected:
	HICON m_hIcon;

	// Generated message map functions
	virtual BOOL OnInitDialog();

	DECLARE_MESSAGE_MAP()
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();

	afx_msg void OnTimer(UINT_PTR nIDEvent);
	afx_msg void OnClose();
	afx_msg void OnDestroy();
	afx_msg void OnSize(UINT nType, int cx, int cy);

	afx_msg LRESULT OnUmLog(WPARAM wSeverity, LPARAM lpMessage);
	afx_msg LRESULT OnUmError(WPARAM wErrorCode, LPARAM lpMessage);
	afx_msg LRESULT OnInitDone(WPARAM wErrorCode, LPARAM lpMessage);
	afx_msg LRESULT OnReceiveZstack(WPARAM wErrorCode, LPARAM lLastFilledRingBufferEntry);
	afx_msg LRESULT OnReceiveEdof(WPARAM wErrorCode, LPARAM lLastFilledRingBufferEntry);
	afx_msg LRESULT OnReceiveAF(WPARAM wErrorCode, LPARAM lMsgParaReceiveAf);
	afx_msg LRESULT OnReceiveSnap(WPARAM wErrorCode, LPARAM lLastFilledRingBufferEntry);

public:
	afx_msg void OnSdoaqInitialize();
	afx_msg void OnSdoaqFinalize();
	afx_msg void OnSelectedCombobox();
	afx_msg void OnSdoaqSetParameter();
	afx_msg void OnSdoaqSetROI();
	afx_msg void OnSdoaqSetAFROI();
	afx_msg void OnSdoaqSetRingBufSize();
	afx_msg void OnSdoaqSetFocusSet();
	afx_msg void OnSdoaqSetSnapFocusSet();
	afx_msg void OnSdoaqSetEdofResize();
	afx_msg void OnSdoaqSingleShotStack();
	afx_msg void OnSdoaqPlayStack();
	afx_msg void OnSdoaqStopStack();
	afx_msg void OnSdoaqSingleShotEdof();
	afx_msg void OnSdoaqPlayEdof();
	afx_msg void OnSdoaqStopEdof();
	afx_msg void OnSdoaqSingleShotAF();
	afx_msg void OnSdoaqPlayAF();
	afx_msg void OnSdoaqStopAF();
	afx_msg void OnSdoaqSnap();
	afx_msg void OnSdoaqSetCalibrationFile();
	afx_msg void OnSdoaqComboObjective();

	void ReadySdoaqDll(void);
	void ShowViewer(void);

private:
	void BuildParameterID_Combobox(void);
	eParameterId GetCurrentParameterID(void);
	void BuildCalibrationFile_Combobox(void);

public:
	enum
	{
		TIMER_LOG = 0
	};

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

#if defined(USE_SDOAL_API_2_4_0)
		AcquisitionFixedParametersEx afp;
#else
		sAcquisitionFixedParameters afp;
#endif

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

private:
	int m_nRingBufferSize = 3;

	int m_nContiStack = 0;
	int m_nContiEdof = 0;
	int m_nContiAF = 0;

public:
	enum { DFLT_FOCUS_STEP = 160 };
	std::vector<int> m_vFocusSet;
	std::vector<int> m_vSnapFocusSet;

	int nMaxWidth, nMaxHeight;

	SDOAQ_CalibrationFile m_calFile;
	std::vector<CString> m_vsCalibList;
	// calibration table 의 x,y,z 범위(unit um). 3D rendering 에 사용
	double dxRangeStart = 0.0;
	double dxRangeEnd = 0.0;
	double dyRangeStart = 0.0;
	double dyRangeEnd = 0.0;
	double dzRangeStart = 0.0;
	double dzRangeEnd = 0.0;

	//----------------------------------------------------------------------------
	// LOG WINDOW
	//----------------------------------------------------------------------------
	CString m_sLog;
	CString m_sLogFileName;
	HANDLE m_hLogFile = INVALID_HANDLE_VALUE;
	DWORD m_tickLastLog = 0;

	void Log(LPCTSTR p_log_str);
	void PrintLog(void);
	void ApiError(LPCTSTR sApi, int eCode);
	CString GetCurrentDir()
	{
		TCHAR currentDir[MAX_PATH];
		::GetCurrentDirectory(MAX_PATH, currentDir);
		return currentDir;
	}

	//----------------------------------------------------------------------------
	// IMAGE VIEWER
	//----------------------------------------------------------------------------
private:
	std::vector<WSIOVOID> m_vhwndIV;
	WSIOVOID m_hwnd3D = NULL;
	void print_wsio_last_error(void);
	void print_wsgl_last_error(void);

public:
	void ImageViewer(size_t uViewer, const char* title = NULL, int title_no = 0
		, const tTestSet& SET = tTestSet(), void* data = NULL);
	void ImageViewer(size_t uViewer, const char* title, int title_no
		, int width, int height, int colorbytes, void* data = NULL);
	void FloatViewer(bool onoff, size_t uViewer, const char* title = NULL, int title_no = 0
		, const tTestSet& SET = tTestSet(), void* data = NULL);
	void Viewer3D(bool onoff, const tTestSet& SET, void* p_pointcloud, void* p_edof);
};
