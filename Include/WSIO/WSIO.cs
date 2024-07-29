/*	WSIO_.cs

	Copyright (c) SD Optics,Inc. All rights reserved.

	========================================================================================================================================================
	Revision history
	========================================================================================================================================================
	Version     date      Author         Descriptions
	--------------------------------------------------------------------------------------------------------------------------------------------------------
*/

using System;
using System.Runtime.InteropServices;
using System.Text;

//====================================================================================================================================
// WSIO (WiseScope integrated Objects) Common Definitioin & API
//------------------------------------------------------------------------------------------------------------------------------------

//#define X86

namespace SDOWSIO
{
	public static partial class WSIO
	{
#if X86
		private const string WSIO_DLL = "WSIODLL_R32.dll";
#else
		private const string WSIO_DLL = "WSIODLL_R64.dll";
#endif

		//------------------------------------------------------------------------------------------------------------------------------------
		// WSIO Version = Ver.MAJOR.MINOR.SUB
		//------------------------------------------------------------------------------------------------------------------------------------
		public static string WSIO_VERSION
		{
			get
			{
				try
				{
					IntPtr versionInfo = WSIO_GetVersion(0);
					return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(versionInfo);
				}
				catch
				{
					return "None";
				}
			}
		}

		//============================================================================
		// ENUMERATED DEFINITION
		//----------------------------------------------------------------------------
		// WSIO RETURN VALUE
		// - Zero(0) or Positive(+) number : Positive meaning (Success, Pass, OK, ...)
		// - Negative(-) number : Negative meaning (Error, ...)
		//----------------------------------------------------------------------------		
		public enum WSIORV
		{
			//==========================================
			WSIORV_SUCCESS						= 0,
			WSIORV_SUCCESS_TRUE					= 1,
			WSIORV_SUCCESS_FALSE				= 2,
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
			//WSIORV_NOWINDOWENVIRONMENT		= -2015,
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
			//------------------------------------------
			WSIORV_ERROR_NEXT					= -2039,
			//------------------------------------------
			WSIORV_ERROR_END					= -4999
			//==========================================
		};

		public static string Get_WSIORV(int rv)
		{
			var array = Enum.GetValues(typeof(WSIORV));

			for (int i = 0; i < array.Length; i++)
			{
				if (rv == (int)array.GetValue(i))
				{
					return ((WSIORV)i).ToString();
				}
			}
			return rv.ToString();
		}

		//----------------------------------------------------------------------------
		// WSIO LOG TYPE
		//----------------------------------------------------------------------------
		public enum WSIOLOGTYPE
		{
			WSIOLOGTYPE_PROGRESS		= 0x1000,
			WSIOLOGTYPE_TEST			= 0x2000,
			WSIOLOGTYPE_TEMP			= 0x2001,
			WSIOLOGTYPE_ERROR			= 0x4000,
			WSIOLOGTYPE_WARNING			= 0x5000,
			WSIOLOGTYPE_WSIMESSAGE		= 0x0010,
			WSIOLOGTYPE_LOG				= 0x6000,
			WSIOLOGTYPE_DATA			= 0x8000,
		};

		//----------------------------------------------------------------------------
		// WSIO IMAGE FORMAT
		//----------------------------------------------------------------------------
		public enum WSIOIMGFORMAT
		{
			WSIOIMGFORMAT_BMP			= 0,
			WSIOIMGFORMAT_JPG			= 1,
			WSIOIMGFORMAT_NONE			= 2,
			WSIOIMGFORMAT_BIN			= 3,
			WSIOIMGFORMAT_CSV			= 4,
			WSIOIMGFORMAT_UNDEFINED		= 5,

			WSIOIMGFORMAT_CZI			= 6,
			WSIOIMGFORMAT_TIFF			= 7,
			WSIOIMGFORMAT_ZIP			= 8,
			WSIOIMGFORMAT_TXT			= 9,
		};

		//============================================================================
		// WSIO MAT STRUCTURE
		//----------------------------------------------------------------------------
		public enum WSIOMATTYPE
		{
			WSIOMATTYPE_UNDEFINED		= 0,
			WSIOMATTYPE_INTEGER			= 1,
			WSIOMATTYPE_REAL			= 2,
			WSIOMATTYPE_GBR				= 3,
			WSIOMATTYPE_RGB				= 4,
		};

		//----------------------------------------------------------------------------
		public struct WSIOMAT
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
			public string	description;	// [offset 0] null terminated string
			public byte		reserved_16;
			public byte		pixel_type;		// [offset 17]
			public byte		reserved_18;
			public byte		pixel_bytes;	// [offset 19]
			public int		total_size;		// [offset 20]
			public int		cols;			// [offset 24] width
			public int		rows;			// [offset 28] height
			public int		line_bytes;		// [offset 32] = cols * pixel_bytes + pad bytes
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 220)]
			public byte[]	reserved_36;
			//public byte data_ptr[1];		// [offset 256] // data from here
		};

		//============================================================================
		// C-LANGAUAGE STYLE FUNCTION DEFINITION
		//----------------------------------------------------------------------------
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void WSIOFUNC_WSIHANDLE(IntPtr wsi_handle);
		
		//----------------------------------------------------------------------------
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void WSIOFUNC_REPLYPARAMS(IntPtr wsif_handle, string wsif_params_str);

		//====================================================================================================================================
		// WSIO (WiseScope integrated Objects) Common API
		//------------------------------------------------------------------------------------------------------------------------------------

		//============================================================================
		// WSIO BASIC FUNCTIONS
		//----------------------------------------------------------------------------
		// GET WSIO & SDC VERSION
		//----------------------------------------------------------------------------
		[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr WSIO_GetVersion(int flag_detail);

		//----------------------------------------------------------------------------
		// SET MAIN WINDOW HANDLE
		//----------------------------------------------------------------------------
		[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern WSIORV WSIO_SetMainHWND(IntPtr main_hwnd);

		//============================================================================
		// LAST ERROR
		//----------------------------------------------------------------------------
		[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern WSIORV WSIO_LastErrorCode();
		
		//----------------------------------------------------------------------------
		[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int WSIO_LastErrorString(StringBuilder dest_buffer, int size_of_dest_buffer);

        //============================================================================
        // LOG SYSTEM
        //----------------------------------------------------------------------------
        [DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern WSIORV WSIO_LogEnable(WSIOLOGTYPE log_type, int log_enable);
		
		//----------------------------------------------------------------------------
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void WSIOFUNC_LOG(WSIOLOGTYPE log_type, string log_str);

		[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern WSIORV WSIO_RegiCbf_Log(WSIOFUNC_LOG cbf);

		//====================================================================================================================================

	}
}
