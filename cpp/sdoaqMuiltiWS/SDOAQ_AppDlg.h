
// SDOAQ_AppDlg.h : header file
//

#pragma once

#include "SDOAQ_Lib.h"
#include "SDOAQ_CalibrationFile.h"

//----------------------------------------------------------------------------
#define NUMS_WS 3

enum EUserMessage
{
	EUM_LOG = 0xA000, // LPARAM is a pointer of CString
	EUM_ERROR, // LPARAM is a pointer of CString
	EUM_INITDONE,
	EUM_RECEIVE_ZSTACK, // WPARAM = MAKEWPARAM(error, multi_ws_id)
	EUM_RECEIVE_EDOF, // WPARAM = MAKEWPARAM(error, multi_ws_id)
	EUM_RECEIVE_AF, // WPARAM = MAKEWPARAM(error, multi_ws_id)
	EUM_RECEIVE_SNAP, // WPARAM = MAKEWPARAM(error, multi_ws_id)
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
	afx_msg LRESULT OnReceiveZstack(WPARAM wparam, LPARAM lLastFilledRingBufferEntry);
	afx_msg LRESULT OnReceiveEdof(WPARAM wparam, LPARAM lLastFilledRingBufferEntry);
	afx_msg LRESULT OnReceiveAF(WPARAM wparam, LPARAM lMsgParaReceiveAf);
	afx_msg LRESULT OnReceiveSnap(WPARAM wparam, LPARAM lLastFilledRingBufferEntry);

	afx_msg void OnUpdateWsSelection();
	afx_msg void OnSdoaqInitialize();
	afx_msg void OnSdoaqFinalize();
	afx_msg void OnSelectedCombobox() { UpdateSelectedCombobox(true); }
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

	void BuildEnvironment(void);
	void ReadySdoaqDll(void);
	void ShowViewer(void);

private:
	void BuildParameterID_Combobox(void);
	eParameterId GetCurrentParameterID(void);
	void BuildCalibrationFile_Combobox(void);
	void UpdateSelectedCombobox(bool o_log);

	//----------------------------------------------------------------------------
private:
	CString m_sScriptFile;
	CString m_sLogPath;


	//----------------------------------------------------------------------------
	// MULTI WS
	//----------------------------------------------------------------------------
public:
	enum e_sdoaq_ws
	{
		SINGLE_WS_SDOAQ = 0,	// SDOAQ operates as a single WiseScope engine that controls one wisescope module. Script data must not contain WSI_1.
		MULTI_1WS_SDOAQ = 1,	// SDOAQ operates as a multi WiseScope engine that controls one wisescope module. Script data requires WSI_1.
		MULTI_2WS_SDOAQ = 2,	// SDOAQ operates as a multi WiseScope engine that controls two wisescope module. Script data requires WSI_1 and WSI_2.
		MULTI_3WS_SDOAQ = 3,	// SDOAQ operates as a multi WiseScope engine that controls three wisescope module. Script data requires WSI_1, WSI_2 and WSI_3.
	};
	int MULWS = SINGLE_WS_SDOAQ;
	unsigned m_cur_ws = MAXINT;
	std::vector<int> vWSSET;

	//----------------------------------------------------------------------------
public:
	enum
	{
		TIMER_LOG = 0
	};

	enum { DFLT_FOCUS_STEP = 160 };

	struct tTestSet
	{
		tTestSet()
			: m_nColorByte(COLORBYTES)
		{
			// acA2040-55uc camera default
			afp.cameraRoiTop = 0;
			afp.cameraRoiLeft = 0;
			afp.cameraRoiWidth = (2064 / 4) * 4;
			afp.cameraRoiHeight = 1544;
			afp.cameraBinning = 1;

			ui.vFocusSet.push_back(DFLT_FOCUS_STEP);
			ui.vSnapFocusSet.push_back(DFLT_FOCUS_STEP);
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

		AcquisitionFixedParametersEx afp;

		inline int PixelSize(void) const { return afp.cameraRoiWidth * afp.cameraRoiHeight; }
		inline int ImgSize(void) const { return PixelSize() * m_nColorByte; }
		inline int DataSize(void) const { return PixelSize() * sizeof(float); }
		inline int PixelWidth(void) const { return afp.cameraRoiWidth; }
		inline int PixelHeight(void) const { return afp.cameraRoiHeight; }

		struct tRingBuf
		{
			tRingBuf(void)
				: active(false)
				, ppBuf(NULL)
				, pSizes(NULL)
				, numsBuf(0)
			{ ; }
			bool active;
			void** ppBuf;	// pointer to the data (image) buffer
			size_t* pSizes;	// array of data sizes (image sizes) for each data buffer
			size_t numsBuf;	// number of actual data in the ring buffer
		} rb;

		struct tFocus
		{
			tFocus(void)
				: numsFocus(10)
			{ ; }
			size_t numsFocus;
			std::vector<int> vFocusSet;
		} focus;

		int m_nColorByte;

		struct t_ui
		{
			CString ROI;
			CString AFROI;
			CString RING_BUF_SIZE;
			CString FOCUS_SET;
			CString SNAPFOCUS_SET;
			CString EDOF_RESIZE_RATIO;

			void update_editwnd(CWnd& wnd);

			int nRingBufferSize = 3;
			int nContiStack = 0;
			int nContiEdof = 0;
			int nContiAF = 0;
			std::vector<int> vFocusSet;
			std::vector<int> vSnapFocusSet;
		} ui;
	};
	tTestSet VSET[NUMS_WS];

public:
	int nMaxWidth, nMaxHeight;

	SDOAQ_CalibrationFile m_calFile;
	std::vector<CString> m_vsCalibList;
	// x,y and z range of calibration table (unit um). used for 3D rendering.
	double dxRangeStart;
	double dxRangeEnd;
	double dyRangeStart;
	double dyRangeEnd;
	double dzRangeStart;
	double dzRangeEnd;

	//----------------------------------------------------------------------------
	// LOG WINDOW
	//----------------------------------------------------------------------------
	CString m_sLog;
	CString m_sLogFileName;
	HANDLE m_hLogFile;
	DWORD m_tickLastLog;

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
	enum { ONLY_WS0_3D = 0 };
	struct
	{
		std::vector<HWND> vhwnd_iv;
		HWND hwnd_3d = NULL;
	} m_vVW[NUMS_WS];
	void print_wsio_last_error(void);
	void print_wsgl_last_error(HWND hwnd_3d);

public:
	void ImageViewer(HWND hwnd_viewer, const char* title = NULL, int title_no = 0
		, const tTestSet& SET = tTestSet(), void* data = NULL);
	void ImageViewer(HWND hwnd_viewer, const char* title, int title_no
		, int width, int height, int colorbytes, void* data = NULL);
	void FloatViewer(bool onoff, HWND hwnd_viewer, const char* title = NULL, int title_no = 0
		, const tTestSet& SET = tTestSet(), void* data = NULL);
	void Viewer3D(bool onoff, HWND hwnd_3d, const tTestSet& SET, void* p_pointcloud, void* p_edof);
};
