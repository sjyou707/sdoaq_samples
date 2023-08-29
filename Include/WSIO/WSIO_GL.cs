using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SDOWSIO
{
	public static partial class WSIO
	{
		public static class GL
		{
			//====================================================================================================================================
			// WSIO Graphic Library Definition & API
			//------------------------------------------------------------------------------------------------------------------------------------
			//	이 파일(WSIO_GL.h)에는 서비스로 제공되는 3D 그래픽 라이브러리 기능을 추가한다.
			//
			//------------------------------------------------------------------------------------------------------------------------------------

			//============================================================================
			// ENUMERATED DEFINITION
			//----------------------------------------------------------------------------

			//----------------------------------------------------------------------------
			// WSGLKEY
			//----------------------------------------------------------------------------
			public enum WSGLKEY
			{
				EKEY_QUERY				= -1,
				EKEY_NULL				= 0,

				/////////////////////////////////////
				//DO NOT CHANGE ESFM_xxxx VALUE

				//enum surface mode (surfacer - mouse left button)
				ESFM_RAWDATA			= 101,

				ESFM_CMAP_RAINBOW		= 111,
				ESFM_CMAP_RGB			= 112,
				ESFM_CMAP_GREY			= 113,
				ESFM_CMAP_JET			= 114,
				ESFM_CMAP_HSV			= 115,

				ESFM_140				= 140,

				ESFM_STATIC_151			= 151,
				ESFM_XYZ_RGB_161		= 161,
				/////////////////////////////////////

				//enum function mode (guider - mouse left button)
				EFUN_NAVIGATION			= 0x10101,
				EFUN_MEASURE			= 0x10102,
				EFUN_PROFILE			= 0x10103,
				EFUN_COMPARE			= 0x10104,

				//enum rotation type (guider - mouse right button)
				EROT_3D					= 0x10201,
				EROT_PLAIN_XY			= 0x10202,

				//enum profile type
				EPFT_XY_Z				= 0x1030C,
				EPFT_XZ_Y				= 0x1030A,
				EPFT_YZ_X				= 0x10306,

				//enum projection mode
				EPJM_ORTHOGRAPHIC		= 0x10401,
				EPJM_PERSPECTIVE		= 0x10402,

				//enum matrix direction
				EDIR_XY					= 0x105C0,
				EDIR_XrY				= 0x105C4,

				//enum user string
				ESTR_FUNCTION			= 0x20101,
				ESTR_PROJECTION			= 0x20102,
				ESTR_ROTATION			= 0x20103,

				ESTR_X_KEY_TO_REMOVE_LAST_LINE	= 0x20201,
				ESTR_X_KEY_TO_REMOVE_ALL_LINES	= 0x20202,
				ESTR_X_KEY_TO_SET_DATUM_PLANE	= 0x20203,
			};

			//----------------------------------------------------------------------------
			// EDisplayAttributes
			//----------------------------------------------------------------------------
			public enum EDisplayAttributes
			{
				// 마우스 포인터에 픽커를 표시한다.
				EDA_SHOW_PICKER_ON_MOUSE			= (1 << 1),

				// 픽커가 가리키는 포인트의 정보를 보여준다.
				EDA_SHOW_PICKED_POINT_INFORMATION	= (1 << 2),

				// 픽커가 3D 물체로 가려지지 않고 항상 보이게 한다.
				EDA_NOHIDE_PICKER					= (1 << 3),

				// 왼쪽 버튼 클릭으로 측정을 한다.
				// 설정을 해제하더라도, 그 동안 측정한 데이타는 유지한다.
				EDA_MEASURE_PICKERS_ON_LCLCK_BUTTON	= (1 << 4),

				// 측정 데이타 리스트와 포인트 번호를 화면에 표시한다.
				EDA_SHOW_LIST_MEASURED_DATA			= (1 << 5),

				// small object, xy plane grid 를 화면에 표시한다.
				EDA_SHOW_GUIDER_OBJECTS				= (1 << 6),

				// reserved. 오른쪽 버튼 더블 클릭으로 xx 를 한다.
				EDA_XXX_ON_RDBCLCK_BUTTON			= (1 << 7),

				// scale object 를 화면에 표시한다.
				EDA_SHOW_SCALE_OBJECTS				= (1 << 8),

				// color map 을 화면에 표시한다.
				EDA_SHOW_COLORMAPBAR_OBJECTS		= (1 << 9),

				// 선택된 object 의 outline 을 표시한다.
				EDA_SHOW_OUTLINE					= (1 << 10),

				// object 의 교차부분을 표시하지 않는다.
				EDA_HIDE_INTERSECTION				= (1 << 11),

				// reserved. 오른쪽 버튼 클릭
				EDA_ON_RCLCK_BUTTON					= (1 << 12),

				// bounding box 를 표시한다.
				EDA_SHOW_BOUNDING_BOX				= (1 << 13),

				// reserved. set datum plane
				EDA_SET_DATAUM_PLANE				= (1 << 14),

				// display transparent objects by blending
				EDA_BLEND							= (1 << 15),

				// NEXT
				EDA_NEXT							= (1 << 16),
			};

			//----------------------------------------------------------------------------
			// EDisplayMode
			//----------------------------------------------------------------------------
			public enum EDisplayMode
			{
				EDM_NULL					= 0,

				// { COLOR DATA TYPE }
				// If none is set, it is assumed to be RGB-FLOAT(0~1) COLOR TYPE
				// EDM_BGR_BYTE: BGR-BYTE(0~255) COLOR TYPE
				EDM_BGR_BYTE				= (1 << 0),

				// { DIMENSION }
				// EDM_DIMENSION_FIXEDXY_25D: It consists of Z (height) data for the XY plane.
				EDM_DIMENSION_FIXEDXY_25D	= (1 << 4),
				// EDM_DIMENSION_CALXY_25D: It consists of Z (height) data for a calibrated XY plane.
				EDM_DIMENSION_CALXY_25D		= (1 << 5),

				// { NDC Factor }
				// EDM_NDC_XY_ONLY: Only X and Y data are used for normalizing. When normalizing, the Z axis is excluded. (Because there is inaccurate noise on the Z axis.)
				EDM_NDC_XY_ONLY				= (1 << 8),
			};

			//----------------------------------------------------------------------------
			public struct tPara_Display25D
			{
				public uint		width;
				public uint		height;
				public double	z_offset1;
				public double	z_offset2;
				public uint		z_slices;
				public double	scx1;
				public double	scx2;
				public double	scy1;
				public double	scy2;
				public double	scz1;
				public double	scz2;
			};

			//----------------------------------------------------------------------------
			public const string GL_MG_ONSTAGE = "onstage";
			public const string GL_MG_GUEST = "guest";

			public const int NUMOF_TRIANGLE_INDICES_OF_PLANE_XY_PER_PIXEL = 6;
			public const int NUMOF_TEXCOORD_BUF_NUMBER_PER_PIXEL = 2;

			//====================================================================================================================================
			// WSIO Graphic Library API
			//------------------------------------------------------------------------------------------------------------------------------------

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern int WSGL_GetMajorVersion();

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern int WSGL_GetMinorVersion();

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern int WSGL_GetPatchVersion();

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSGL_Initialize(IntPtr parent_hwnd, out IntPtr p_hd3v);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSGL_Finalize(IntPtr hd3v);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern bool WSGL_IsOnRun(IntPtr hd3v);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSGL_ShowWindow(IntPtr hd3v, bool show, int left, int top, int right, int bottom);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSGL_ResetDisplay(IntPtr hd3v);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSGL_Display_BG(IntPtr hd3v);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSGL_Display_25D(
						IntPtr hd3v,
						string sz_group,
						string sz_name,
						float[] p_vertex,
						uint nums_vertex_data,
						byte[] p_image,
						uint nums_image_data,
						int mode,
						float unit_microns,
						tPara_Display25D[] p_para
						);


			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSGL_SetDisplayAttributes(IntPtr hd3v, int attributes);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSGL_ClearAllMeasureData(IntPtr hd3v, bool flagRendering);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSGL_ClearLastMeasureData(IntPtr hd3v, bool flagRendering);

			//====================================================================================================================================
		}
	}
}
