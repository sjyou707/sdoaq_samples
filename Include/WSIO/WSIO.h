/*	WSIO.h

	Copyright (c) SD Optics,Inc. All rights reserved.

	========================================================================================================================================================
	Revision history
	========================================================================================================================================================
	Version     date      Author         Descriptions
	--------------------------------------------------------------------------------------------------------------------------------------------------------
*/

#pragma once

//====================================================================================================================================
// WSIO (WiseScope integrated Objects) Common Definitioin & API
//------------------------------------------------------------------------------------------------------------------------------------
// WSIO Version = Ver.MAJOR.MINOR.SUB
//------------------------------------------------------------------------------------------------------------------------------------

//#define Vyymmdd_wsio

#if defined(Vyymmdd_wsio)
	#define WSIO_VERSION	_T("Ver.3.x.x + "  __DATE__ "") //
#else
	//#define WSIO_VERSION	_T("Ver.3.27.xx") // 2023-xx-xx , add here
	#define WSIO_VERSION	_T("Ver.3.27.0") // 2024-01-08 , ADD WSUT_GetRainbowBGR, WSUT_GenerateRainbowColorMap, IV MR list line type deletion issue.
	//#define WSIO_VERSION	_T("Ver.3.26.3") // 2023-12-05 , MR Script set 초기화 오류 수정
	//#define WSIO_VERSION	_T("Ver.3.26.2") // 2023-11-15 , WSUT_IV_AttachRawImgData(|_V2|F3) 인자 오류 로그 출력에서 예외발생 수정
	//#define WSIO_VERSION	_T("Ver.3.26.1") // 2023-11-14 , WSUT_IV_AttachRawImgData(|_V2|F3) 수정 
	//#define WSIO_VERSION	_T("Ver.3.26.0") // 2023-11-08 , WSIF speed up, ADD WSUT_GetBmpAttributes, WSGL_(S|G)etZscaleRatio, WSUTIVOSDTYPE_TEXT_BOUNDARY, WSUTIVRESOURCE_OBJ(TEXT|BACK|LINE)COLOR
	//#define WSIO_VERSION	_T("Ver.3.25.5") // 2023-10-13 , FIX NO-TYPE_BUG@MULTI-FOCUS@IVIEWER
	//#define WSIO_VERSION	_T("Ver.3.25.4") // 2023-10-11 , send notify alert befor init.
	//#define WSIO_VERSION	_T("Ver.3.25.3.R1") // 2023-10-13 , FIX NO-TYPE_BUG@MULTI-FOCUS@IVIEWER
	//#define WSIO_VERSION	_T("Ver.3.25.3") // 2023-09-22 , UPGRADE IV OBJ FUNCTION, WSUT_IV_AttachRawImgData_F3
	//#define WSIO_VERSION	_T("Ver.3.25.2") // 2023-09-06 , ADD WSIOIMGFORMAT_TXT, WSUTIVOSDTYPE_TEXT_VALUE, WSUTIVGUISHAPE_DRAW_ASSISTANT, WSUTIVGUISHAPE_PERPENDICULAR,WSUTIVGUISHAPE_8D,WSUTIVGUISHAPE_4D
	//#define WSIO_VERSION	_T("Ver.3.25.0") // 2023-08-11 , ADD WSIF_(RegiCbf|SetItem|ReqItem)_AlgorithmParam
	//#define WSIO_VERSION	_T("Ver.3.24.0") // 2023-08-04 , ADD WSUT_IV_SetResource, WSUTIVRESOURCE, WSUTIVGUISHAPE, WSUTIVOSDTYPE, WSUTIVOBJFUNC_BIT_ONDRAWING
	//#define WSIO_VERSION	_T("Ver.3.23.0") // 2023-06-26 , ADD 43 WSGL_xxxx APIs.
	//#define WSIO_VERSION	_T("Ver.3.22.0") // 2023-04-13 , ADD WSIF_Set_DefectResultName, WSUT_IV_SetPixelWidth, WSUT_IV_ActivateFunction, WSUT_IV_(En|De)code(Wparam|Rect|Point|Point2), WSUT_IV_SetFunctionLineData
	//#define WSIO_VERSION	_T("Ver.3.21.0") // 2023-02-24 , ADD wSIF_Set_DefectCode
	//#define WSIO_VERSION	_T("Ver.3.20.1") // 2022-11-14 , UPGRADE WSIF SYNC CODE ( <- WSIF LOCK ISSUE )
	//#define WSIO_VERSION	_T("Ver.3.20.0") // 2022-10-26 , ADD WSIF_(RegiCbf|Set|Get)_DetectedCavity, WSUT_IV_(RegiCbf_|Set)FunctionScript, WSUT_IV_UpdateManualFocus
	//#define WSIO_VERSION	_T("Ver.3.19.0") // 2022-08-30 , ADD WSIF_SetItem_UploadStorage, WSIF_(RegiCbf|SetItem|GetItem)_ExpectedCavity
	//#define WSIO_VERSION	_T("Ver.3.18.2") // 2022-06-13 , GENERATE 32bit DLL
	//#define WSIO_VERSION	_T("Ver.3.18.1") // 2022-03-02 , CHANGE the maximum waiting time of WSI-algorithm execution from 30 seconds t 300 seconds.
	//#define WSIO_VERSION	_T("Ver.3.18.0") // 2022-02-07 , ADD WSIF_SetItem_LAS_Info
	//#define WSIO_VERSION	_T("Ver.3.17.1") // 2022-01-21 , ADD WSIF_Finalize_TID , Finalize service // RENAME: WSIFMINMOVEOKTIME -> WSIFDEFMOVEOKDELAYTIME
	//#define WSIO_VERSION	_T("Ver.3.17.0") // 2022-01-03 , ADD WSIF_ResetCommonData, WSIF_(RegiCbf|Set|Get)_TCODE, CODE service
	//#define WSIO_VERSION	_T("Ver.3.16.0") // 2021-11-23 , ADD WSIF_RegiCbf_Alert
	//#define WSIO_VERSION	_T("Ver.3.15.1") // 2021-11-19 , UPG WSIO_ALGORITHM.h file only
	//#define WSIO_VERSION	_T("Ver.3.15.0") // 2021-09-14 , ADD WSUT_Uint64FromLineScript.
	//#define WSIO_VERSION	_T("Ver.3.14.5") // 2021-08-30 , Supports long REQUEST_RAW_INSPECT message.
	//#define WSIO_VERSION	_T("Ver.3.14.4") // 2021-07-29 , IEDB service , ADD WSUT_WriteImageFile , ADD WSIF_(SetItem|ReqItem|RegiCbf)_AlgorithmSectionID
											 // 2021-01-12 , ADD WSIF_(Request|RegiCbf)_UMover , ADD WSIF_(Request|RegiCbf)_Mover_xxx, WSIF_Mover_xxx, xxx=QN_Move, QW_Move, QN_GoHome, QW_GoHome, QW_GoHome2D, Stop, CurrentPosition, Ready, MoveDone)
	//#define WSIO_VERSION	_T("Ver.3.13.0") // 2020-11-17 , ADD WSIF_Request(_|_Raw)CaptureWait
	//#define WSIO_VERSION	_T("Ver.3.12.2") // 2020-11-06 , RECOVER TCP IF
	//#define WSIO_VERSION	_T("Ver.3.12.1") // 2020-10-08 , UPG RegiCbf_ThreadRunAlgorithm
	//#define WSIO_VERSION	_T("Ver.3.12.0") // 2020-10-05 , ADD WSIF_Request_VTTrigger
	//#define WSIO_VERSION	_T("Ver.3.11.0") // 2020-09-25 , ADD WSIF_RegiCbf_UInspect_BESTFOCUS
	//#define WSIO_VERSION	_T("Ver.3.10.1") // 2020-09-08 , ADD WSUT_IV_SetFunctionRectData, WSUT_IV_Decoding8BytesToRect
	//#define WSIO_VERSION	_T("Ver.3.10.0") // 2020-09-07 , ADD WSUT_IV_SetFunctionActivation, WSUTIVRESOURCE_MR(BRIGHTNESS|AUTOFOCUS)
	//#define WSIO_VERSION	_T("Ver.3.9.0") // 2020-09-01 , ADD WSIF_Request_VTBestFocusStep, WSUT_IntFromCmdLine
	//#define WSIO_VERSION	_T("Ver.3.8.0") // 2020-07-15 , ADD WSIF_RegiCbf_UInspect(|_Code)
	//#define WSIO_VERSION	_T("Ver.3.7.0") // 2020-05-27 , ADD WSIF_Request_(UInspect|VTProfile|VTCommand|RawVTCommand)
	//#define WSIO_VERSION	_T("Ver.3.6.3") // 2020-04-14 , MOD WSISC_DoRunAlgorithm, add wsio error message
	//#define WSIO_VERSION	_T("Ver.3.6.2") // 2020-02-27 , ADD WSUT_IV_SetFlipMode (WSUTIVFLIPBITS_ROTATE_R90, WSUTIVFLIPBITS_ROTATE_R270)
	//#define WSIO_VERSION	_T("Ver.3.6.1") // 2020-02-19 , ADD WSIF_(SetItem|ReqItem|RegiCbf)_UserStorageAttr_A
	//#define WSIO_VERSION	_T("Ver.3.6.0") // 2020-02-12 , ADD WSIF_SetItem_UserStorageAttributes_A
	//#define WSIO_VERSION	_T("Ver.3.5.0") // 2020-01-16 , ADD WSUT_IV_AttahRawImgData_V2
	//#define WSIO_VERSION	_T("Ver.3.4.0") // 2019-12-04 , ADD WSISC_Request_RunShellCommand, WSISC_(Request_|RegiCbf_Request)General
	//#define WSIO_VERSION	_T("Ver.3.3.0") // 2019-09-27 , ADD WSIF_SetItem_LGIT_LAS_Info
	//#define WSIO_VERSION	_T("Ver.3.2.1") // 2019-09-10 , MOD WSUT_IpFromCmdLine
	//#define WSIO_VERSION	_T("Ver.3.2.0") // 2019-08-20 , ADD WSIF_Request_SavingTid
	//#define WSIO_VERSION	_T("Ver.3.1.0") // 2019-07-31 , ADD WSIF_Request_Ready_(Inspect|RawInspect)
	//#define WSIO_VERSION	_T("Ver.3.0.3") // 2019-07-19 , UPG WSISC_(Request|Reply)_ChangeMonitorImage::image_name_str -> allow NULL. 
	//#define WSIO_VERSION	_T("Ver.3.0.3") // 2019-07-12 , UPG WSIF_SetItem_LightingPattern - multi setting
#endif

//============================================================================
// WSIODLL_EXPORTS
//----------------------------------------------------------------------------
#if defined(WSIODLL_EXPORTS)
#define WSIODLL_API	__declspec(dllexport)	// for DLL source
#else
#define WSIODLL_API extern //	__declspec(dllimport)	// for USER source
#endif

//============================================================================
// ENUMERATED DEFINITION
//----------------------------------------------------------------------------
// WSIO RETURN VALUE
// - Zero(0) or Positive number : Positive meaning (Success, Pass, OK, ...)
// - Negative number : Negative meaning (Error, ...)
//----------------------------------------------------------------------------

typedef int	WSIORV;
enum WSIORV_Enum
{
	//==========================================
	WSIORV_SUCCESS						=  0,
	WSIORV_SUCCESS_TRUE					=  1,
	WSIORV_SUCCESS_FALSE				=  2,
	//==========================================
	WSIORV_ERROR_BEGIN					= -2000,
	//------------------------------------------
	WSIORV_FAIL							= -2001,
	WSIORV_NODLL						= -2038,		// Failed to load dll module
	WSIORV_DEPRECATED_API				= -2037,		// This api is deprecated.

	WSIORV_WSIFCLASS_INVALID			= -2002,		// WSIFCLASS is invalid.
	WSIORV_WSIHANDLE_NOTEXIST			= -2003,		// WSIHANDLE does not exist.
	WSIORV_WSIHANDLE_NOTOPENED			= -2004,		// WSIHANDLE is not opened.
	WSIORV_WSIHANDLE_NOTESTABLISHED		= -2020,		// WSIHANDLE is not established.
	WSIORV_WSINETTYPE_INVALID			= -2005,		// WSIFNETTYPE is invalid.
	WSIORV_WSIO_OPENFAILED				= -2006,		// 
	WSIORV_WSIOLOGTYPE_INVALID			= -2007,		// 
	WSIORV_USERRECORDID_INVALID			= -2008,		// USER RECORD ID is invalid.
	WSIORV_USERSTRINGID_INVALID			= -2009,		// USER STRING ID is invalid.
	WSIORV_INTERNAL_ERROR				= -2010,
	//WSIORV_WSIFTIDSTR_TOOLONG			= -2011,		// WSIOTIDSTR is too long
	WSIORV_ARG_STRISTOOLONG				= -2012,		// String argument is too long
	WSIORV_ARG_BLOCKISTOOLONG			= -2021,		// Block argument is too long
	WSIORV_ARG_BUFISNOTSUFFICIENT		= -2013,		// String buffer argument is not sufficent.
	WSIORV_ARG_NULLPOINTER				= -2035,		// There is one or more null pointer arguments.
	WSIORV_TIMEOUT						= -2014,
	//WSIORV_NOWINDOWENVIRONMENT			= -2015,
	WSIORV_MAINHWNDALREADYASSIGNED		= -2016,
	WSIORV_FAILEDTOCREATEWINDOW			= -2017,
	WSIORV_HWNDISNOTWINDOW				= -2018,
	WSIORV_IMAGEVIEWER_TOOMANY			= -2019,
	WSIORV_IMAGEVIEWER_NOTFOUND			= -2022,
	WSIORV_FAILEDTOSETWINDOWPOS			= -2023,
	WSIORV_FAILEDTOATTACHFILE			= -2024,
	WSIORV_FAILEDTOATTACHIMAGEDATA		= -2029,
	WSIORV_NOIMGVIEWRESOURCE			= -2025,		// The resource ID is not applicable.
	WSIORV_NOIMGVIEWIMAGEMODE			= -2026,
	WSIORV_NOTYETIMPLEMENTED			= -2027,
	WSIORV_ARG_INVALIDALGORITHMID		= -2028,
	WSIORV_NOTOKEN						= -2030,
	WSIORV_TOKENHASINVALIDDATA			= -2031,
	WSIORV_CAMERA_BUSY					= -2032,
	WSIORV_FAILTOOPENSHAREDMEMORY		= -2033,
	WSIORV_NOTIMPLEMENTED				= -2034,
	WSIORV_FAILEDTOWRITEFILE			= -2036,		// File writing error
	WSIORV_FAILEDTOREADFILE				= -2039,
	//------------------------------------------
	WSIORV_ERROR_NEXT					= -2040,
	//------------------------------------------
	WSIORV_ERROR_END					= -4999
	//==========================================
};

//----------------------------------------------------------------------------
// WSIO LOG TYPE
//----------------------------------------------------------------------------

typedef int WSIOLOGTYPE;
enum WSIOLOGTYPE_Enum
{
	WSIOLOGTYPE_PROGRESS				= 0x1000,
	WSIOLOGTYPE_TEST					= 0x2000,
	WSIOLOGTYPE_TEMP					= 0x2001,
	WSIOLOGTYPE_ERROR					= 0x4000,
	WSIOLOGTYPE_WARNING					= 0x5000,
	WSIOLOGTYPE_WSIMESSAGE				= 0x0010,
	WSIOLOGTYPE_LOG						= 0x6000,
	WSIOLOGTYPE_DATA					= 0x8000,
};

//----------------------------------------------------------------------------
// WSIO IMAGE FORMAT
//----------------------------------------------------------------------------

typedef int WSIOIMGFORMAT;
enum WSIOIMGFORMAT_Enum
{
	WSIOIMGFORMAT_BMP = 0,
	WSIOIMGFORMAT_JPG = 1,
	WSIOIMGFORMAT_NONE = 2,
	WSIOIMGFORMAT_BIN = 3,
	WSIOIMGFORMAT_CSV = 4,
	WSIOIMGFORMAT_UNDEFINED = 5,

	WSIOIMGFORMAT_CZI = 6,
	WSIOIMGFORMAT_TIFF = 7,
	WSIOIMGFORMAT_ZIP = 8,
	WSIOIMGFORMAT_TXT = 9,
};


//============================================================================
// CONSTANT DEFINITION
//----------------------------------------------------------------------------
// LENGTH OF STRING
//----------------------------------------------------------------------------
//#define WSIOSIZEOF_TID	(255) //The size limit of TID has been removed. However, if you need this definition, you can remove the comment mark and use the value as 255.
#define WSIOSIZEOF_MAXPATH				(512)

//============================================================================
// C-LANGAUAGE STYLE TYPE DEFINITION
//----------------------------------------------------------------------------
typedef void*				WSIOVOID;
typedef char				WSIOCHAR;
typedef const char*			WSIOCSTR;	// constant string
typedef char*				WSIOPSTR;	// non-constant string
typedef unsigned char		WSIOBYTE;
typedef unsigned short		WSIOUINT16;
typedef int					WSIOINT;
typedef unsigned			WSIOUINT;
typedef unsigned long long	WSIOUINT64;
typedef unsigned long		WSIORGB;
typedef float				WSIOFLOAT;
typedef double				WSIODOUBLE;

typedef WSIOVOID			WSIHANDLE;

//============================================================================
// WSIO MAT STRUCTURE
//----------------------------------------------------------------------------
typedef unsigned char WSIOMATTYPE;
enum WSIOMATTYPE_Enum
{
	WSIOMATTYPE_UNDEFINED = 0,
	WSIOMATTYPE_INTEGER = 1,
	WSIOMATTYPE_REAL = 2,
	WSIOMATTYPE_GBR = 3,
	WSIOMATTYPE_RGB = 4,
};

//----------------------------------------------------------------------------
struct WSIOMAT
{
	WSIOCHAR description[16]; // [offset 0] null terminated string
	WSIOBYTE reserved_16;
	WSIOMATTYPE pixel_type; // [offset 17]
	WSIOBYTE reserved_18;
	WSIOBYTE pixel_bytes; // [offset 19]
	WSIOINT total_size; // [offset 20]
	WSIOINT cols; // [offset 24] width
	WSIOINT rows; // [offset 28] height
	WSIOINT line_bytes; // [offset 32] = cols * pixel_bytes + pad bytes
	WSIOBYTE reserved_36[220];
	//WSIOBYTE data_ptr[1]; // [offset 256] // data from here
};
#define WSIOMAT_DATA_OFFSET 256

//============================================================================
// C-LANGAUAGE STYLE FUNCTION DEFINITION
//----------------------------------------------------------------------------
typedef void(*WSIOFUNC_WSIHANDLE)(WSIHANDLE wsi_handle);
//----------------------------------------------------------------------------
typedef void(*WSIOFUNC_REPLYPARAMS)(WSIHANDLE wsif_handle, WSIOCSTR wsif_params_str);
//----------------------------------------------------------------------------

//====================================================================================================================================
// WSIO (WiseScope integrated Objects) Common API
//------------------------------------------------------------------------------------------------------------------------------------

//============================================================================
// WSIO BASIC FUNCTIONS
//----------------------------------------------------------------------------
// GET WSIO & SDC VERSION
//----------------------------------------------------------------------------
WSIODLL_API	WSIOCSTR		WSIO_GetVersion(
			WSIOINT					flag_detail
			);

//----------------------------------------------------------------------------
// SET MAIN WINDOW HANDLE
//----------------------------------------------------------------------------
WSIODLL_API	WSIORV			WSIO_SetMainHWND(
			WSIOVOID				main_hwnd
			);

//============================================================================
// LAST ERROR
//----------------------------------------------------------------------------
WSIODLL_API	WSIORV			WSIO_LastErrorCode(
			void
			);
//----------------------------------------------------------------------------
WSIODLL_API	WSIOINT			WSIO_LastErrorString(
			WSIOPSTR				dest_buffer,
			WSIOINT					size_of_dest_buffer
			);

//============================================================================
// LOG SYSTEM
//----------------------------------------------------------------------------
WSIODLL_API	WSIORV			WSIO_LogEnable(
			WSIOLOGTYPE				log_type,
			WSIOINT					log_enable	// NOT_ZERO:enable, ZERO:disable
			);
//----------------------------------------------------------------------------
typedef void (*WSIOFUNC_LOG)(
			WSIOLOGTYPE				log_type,
			WSIOCSTR				log_str
			);
WSIODLL_API	WSIORV			WSIO_RegiCbf_Log(WSIOFUNC_LOG cbf);

//====================================================================================================================================
