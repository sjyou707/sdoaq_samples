/*	WSIO_UTIL.cs

	Copyright (c) Optics,Inc. All rights reserved.

	========================================================================================================================================================
	Revision history
	========================================================================================================================================================
	Version     date      Author         Descriptions
	--------------------------------------------------------------------------------------------------------------------------------------------------------
*/

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SDOWSIO
{
	public static partial class WSIO
	{
		public static class UTIL
		{
			//====================================================================================================================================
			// WSIO UTILities API
			//------------------------------------------------------------------------------------------------------------------------------------
			//	이 파일(WSIO_UTIL.cs)에는 서비스로 제공되는 유틸리티 성격의 기능을 추가한다.
			//	여기에 추가되는 내용은 WSIO 본연의 기능은 아니다.
			//
			//	1. 샘플 코드에서 사용되는 기능
			//
			//	2. 사용자의 편의를 위해 제공되는 기능
			//
			//------------------------------------------------------------------------------------------------------------------------------------

			//============================================================================
			// ENUMERATED DEFINITION
			//----------------------------------------------------------------------------

			public enum WSUTIVOPMODE
			{
				WSUTIVOPMODE_NULL					= 0,
				WSUTIVOPMODE_INFOIMAGE				= (1 << 0),
				WSUTIVOPMODE_INFOPOS				= (1 << 1),
				WSUTIVOPMODE_INFORULLER				= (1 << 2),
				WSUTIVOPMODE_INFOOSD				= (1 << 4),
				WSUTIVOPMODE_IMGPROCESSING			= (1 << 3),
				WSUTIVOPMODE_TOPTITLE				= (1 << 5),
				WSUTIVOPMODE_TOPWINSIZE				= (1 << 6),
				WSUTIVOPMODE_RESERVED_07			= (1 << 7),
				WSUTIVOPMODE_RESERVED_08			= (1 << 8),
				WSUTIVOPMODE_RESERVED_09			= (1 << 9),
				WSUTIVOPMODE_ENLARGEMENT			= (1 << 10),
				WSUTIVOPMODE_FRAMEOUTER				= (1 << 11),
				WSUTIVOPMODE_FRAMEINNER				= (1 << 12),
				WSUTIVOPMODE_CREATE_WITH_BR_FUNC	= (1 << 20),
				WSUTIVOPMODE_RESERVED_22			= (1 << 22),
				WSUTIVOPMODE_RESERVED_23			= (1 << 23),
				WSUTIVOPMODE_RESERVED_24			= (1 << 24),
				WSUTIVOPMODE_RESERVED_25			= (1 << 25),
				WSUTIVOPMODE_RESERVED_26			= (1 << 26),
				WSUTIVOPMODE_RESERVED_27			= (1 << 27),
				WSUTIVOPMODE_VISION					= (WSUTIVOPMODE_INFOIMAGE | WSUTIVOPMODE_INFOPOS | WSUTIVOPMODE_INFORULLER
														| WSUTIVOPMODE_ENLARGEMENT | WSUTIVOPMODE_CREATE_WITH_BR_FUNC),
			};

			public enum WSUTIVRESOURCE
			{
				WSUTIVRESOURCE_UNUSEDAREA			= 0,
				WSUTIVRESOURCE_OUTERFRAME			= 1,
				WSUTIVRESOURCE_INNERFRAME			= 2,
				WSUTIVRESOURCE_STATICPAINT			= 3,
				WSUTIVRESOURCE_NOIMAGECOLOR			= 4,
				WSUTIVRESOURCE_MRBRIGHTNESS			= 11,
				WSUTIVRESOURCE_MRAUTOFOCUS			= 12,
				WSUTIVRESOURCE_MRDSLOP				= 14,
				WSUTIVRESOURCE_MRMULTIFOCUS			= 13,
				WSUTIVRESOURCE_MRMULTILINE			= 15,
				WSUTIVRESOURCE_MRMULTIRECT			= 18,
				WSUTIVRESOURCE_MRMULTIELLIPSE		= 19,
				WSUTIVRESOURCE_MRMULTIMEASURE		= 20, // MULTILINE, MULTIRECT, MUTIELLIPSE
				WSUTIVRESOURCE_GUIEFFECT			= 16,
				WSUTIVRESOURCE_OSDTYPE				= 17,
			};

			public enum WSUTIVGUISHAPE
			{
				WSUTIVGUISHAPE_4D					= (1 << 9),
				WSUTIVGUISHAPE_8D					= (1 << 10),
				WSUTIVGUISHAPE_PERPENDICULAR		= (1 << 11),
				WSUTIVGUISHAPE_BOUNDARY4			= (1 << 12),
				WSUTIVGUISHAPE_LINECROSS			= (1 << 13),
				WSUTIVGUISHAPE_DRAW_ASSISTANT		= (1 << 14),
			};

			public enum WSUTIVOSDTYPE
			{
				WSUTIVOSDTYPE_TEXT_ID				= (1 << 1),
				WSUTIVOSDTYPE_TEXT_TYPE				= (1 << 2),
				WSUTIVOSDTYPE_TEXT_VALUE			= (1 << 3),
			};

			public enum WSUTIVOBJFUNC
			{
				WSUTIVOBJFUNC_NULL					= 0,
				WSUTIVOBJFUNC_SELF					= (1 << 0),
				WSUTIVOBJFUNC_SCRIPT				= (1 << 1),
				WSUTIVOBJFUNC_UPDATERECT			= (1 << 2),
				WSUTIVOBJFUNC_UPDATELINE			= (1 << 3),
				WSUTIVOBJFUNC_SPOT					= (1 << 4),
				WSUTIVOBJFUNC_UPDATELINES			= (1 << 5),
				WSUTIVOBJFUNC_BIT_ONDRAWING			= (1 << 15),
				//WSUTIVOBJFUNC_ALL					= (ushort) - 1
			};

			public enum WSUTIVOBJTYPE
			{
				WSUTIVOBJTPYE_NULL					= 0,
				WSUTIVOBJTPYE_AUTO_FOCUS			= 1,
				WSUTIVOBJTPYE_FIXED_FOCUS			= 2,
				WSUTIVOBJTPYE_EDOF					= 3,
				WSUTIVOBJTPYE_MEASURE_LINE			= 4,
				WSUTIVOBJTPYE_MEASURE_RECT			= 5,
				WSUTIVOBJTPYE_MEASURE_ELLIPSE		= 6,
			};

			public enum WSUTIVIMAGEMODE
			{
				WSUTIVIMAGEMODE_NORMAL				= 0, // default value
				WSUTIVIMAGEMODE_STATICPAINT			= 1,
			};

			public enum WSUTIVFLIPBITS
			{   // If the FLIP attributes and the ROTATE attributes are given at the same time, The ROTATE is perfromed first.
				WSUTIVFLIPBITS_NORMAL				= 0,  // default value
				WSUTIVFLIPBITS_VERT					= (1 << 0),
				WSUTIVFLIPBITS_HORZ					= (1 << 1),
				WSUTIVFLIPBITS_ROTATE_R180			= WSUTIVFLIPBITS_VERT | WSUTIVFLIPBITS_HORZ,
				WSUTIVFLIPBITS_ROTATE_R90			= (1 << 2),
				WSUTIVFLIPBITS_ROTATE_R270			= (1 << 3),
			};

			//----------------------------------------------------------------------------
			// WSIO WINDOW EVENT CODE
			// : WPARAM(LOWORD:child-id, HIWORD:WSUTWEVTCODE_Enum), LPARAM(data)
			//----------------------------------------------------------------------------

			public enum WSUTWEVTCODE
			{
				WSUTWEVTCODE_BTNCLK					= 0x0011,
				WSUTWEVTCODE_LBTNDN					= 0x0021,
				WSUTWEVTCODE_LBTNUP					= 0x0022,
				WSUTWEVTCODE_LBTNDCLK				= 0x0023,
				WSUTWEVTCODE_RBTNDN					= 0x0031,
				WSUTWEVTCODE_RBTNUP					= 0x0032,
				WSUTWEVTCODE_RBTNDCLK				= 0x0033,

				WSUTWEVTCODE_REQLAYOUT				= 0x4101,
				WSUTWEVTCODE_VALUEUPDATED			= 0x4201,

				WSUTWEVTCODE_TITLECLK				= 0x8111,
				WSUTWEVTCODE_VMCAMCLK				= 0x8211,	// bool(Button state) , The view camera button click
				WSUTWEVTCODE_VMCAMLBTNDBLCLK		= 0x8223,	// bool(Button state) , The view camera button double-click
				WSUTWEVTCODE_VMCAPCLK				= 0x8311,	// bool(Button state) , The view captured button click
				WSUTWEVTCODE_VMCAPLBTNDBLCLK		= 0x8323,	// bool(Button state) , The view captured button double-click
				WSUTWEVTCODE_VMRESCLK				= 0x8411,	// bool(Button state) , The view result button click
				WSUTWEVTCODE_VMRESLBTNDBLCLK		= 0x8423,	// bool(Button state) , The view result button double-click
				WSUTWEVTCODE_LIVECLK				= 0x8511,	// bool(Button state) , The live button click
				WSUTWEVTCODE_LIVELBTNDBLCLK			= 0x8523,	// bool(Button state) , The live button double-click
				WSUTWEVTCODE_OFFREVLBTNDBLCLK		= 0x8623,	// The off-review button double-click
			};

			//====================================================================================================================================
			// WSIO UTILities API
			//------------------------------------------------------------------------------------------------------------------------------------

			//============================================================================
			// IMAGE VIEWER
			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_CreateImageViewer(string profile_str, IntPtr parent_hwnd, out IntPtr ptr_viewer_hwnd, uint viewer_control_id, WSUTIVOPMODE operation_mode);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_DestroyImageViewer(IntPtr viewer_hwnd);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_SetEventMessage(IntPtr viewer_hwnd, uint event_message_id);

			//----------------------------------------------------------------------------
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate void WSUTFUNC_IV_ONPAINT(uint viewer_control_id, IntPtr mem_cdc_ptr, int position_left, int position_top, int position_right, int position_bottom);

			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_RegiCbf_OnPaintPost(IntPtr viewer_hwnd, WSUTFUNC_IV_ONPAINT cbf);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_ShowWindow(IntPtr viewer_hwnd, int flag_show, int position_left, int position_top, int position_right, int position_bottom);

			//----------------------------------------------------------------------------
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate void WSUTFUNC_IV_STRING(IntPtr viewer_hwnd, ushort obj_func, ushort cb_data, string script_str);

			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern ulong WSUT_IV_EncodeWparam(ushort obj_func, ushort cb_data, uint data32);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_DecodeWparam(ulong wparam, out ushort p_obj_func, out ushort p_cb_data, out uint p_data32);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern ulong WSUT_IV_EncodeRect(ushort left, ushort top, ushort right, ushort bottom);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_DecodeRect(ulong u64, out ushort p_left, out ushort p_top, out ushort p_right, out ushort p_bottom);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern ulong WSUT_IV_EncodePoint(uint point_x, uint point_y);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_DecodePoint(ulong u64, out uint p_point_x, out uint p_point_y);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern ulong WSUT_IV_EncodePoint2(ushort point1_x, ushort point1_y, ushort point2_x, ushort point2_y);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_DecodePoint2(ulong u64, out ushort p_point1_x, out ushort p_point1_y, out ushort p_point2_x, out ushort p_point2_y);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_ActivateFunction(IntPtr viewer_hwnd, WSUTIVRESOURCE resource_id, ushort obj_func, ushort cb_data, WSUTFUNC_IV_STRING cbf, IntPtr receiver_hwnd, uint window_msg_id);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_SetFunctionScript(IntPtr viewer_hwnd, WSUTIVRESOURCE resource_id, string script_str);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_SetFunctionRectData(IntPtr viewer_hwnd, WSUTIVRESOURCE resource_id, int position_left, int position_top, int position_right, int position_bottom);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_SetFunctionLineData(IntPtr viewer_hwnd, WSUTIVRESOURCE resource_id, int begin_point_x, int begin_point_y, int end_point_x, int end_point_y);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_UpdateManualFocus(IntPtr viewer_hwnd, uint manual_focus);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_Decoding8BytesToRect(ulong lParam, out int ptr_left, out int ptr_top, out int ptr_right, out int ptr_bottom);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_SetResource(IntPtr viewer_hwnd, WSUTIVRESOURCE resource_id, uint value);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_SetColor(IntPtr viewer_hwnd, WSUTIVRESOURCE resource_id, uint rgb_color);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_SetImageMode(IntPtr viewer_hwnd, WSUTIVIMAGEMODE image_mode);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_SetFlipMode(IntPtr viewer_hwnd, WSUTIVFLIPBITS flip_bits);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_AttachFile(IntPtr viewer_hwnd, string file_name_str);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			unsafe public static extern WSIORV WSUT_IV_AttachRawImgData(IntPtr viewer_hwnd, uint width, uint height, uint line_bytes, uint pixel_bytes, byte[] data, uint data_size);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			unsafe public static extern WSIORV WSUT_IV_AttachRawImgData_V2(IntPtr viewer_hwnd, uint width, uint height, uint line_bytes, uint pixel_bytes, byte[] data, uint data_size, string path_name_str);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			unsafe public static extern WSIORV WSUT_IV_AttachRawImgData_V3(IntPtr viewer_hwnd, uint width, uint height, uint line_bytes, uint pixel_bytes, void* data_surface, uint data_surface_size, string path_name_str, void* data_f3, uint data_f3_size);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IV_SetPixelWidth(IntPtr viewer_hwnd, double pixel_width_mm);

			//============================================================================
			// COMMAND LINE PARSING
			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IpFromCmdLine(string command_line_str, string token_str, StringBuilder ip_buffer, uint size_of_ip_buffer, out int ptr_port);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IntFromCmdLine(string command_line_str, string token_str, out int ptr_int);

			//============================================================================
			// LINE SCRIPT
			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_StringFromLineScript(string script_str, string token_str, StringBuilder value_buffer, uint size_of_value_buffer);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_IntFromLineScript(string script_str, string token_str, out int ptr_int);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_Uint64FromLineScript(string script_str, string token_str, out ulong ptr_uint64);

			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_AddressFromLineScript(string script_str, string token_str, IntPtr ptr_address);

			//============================================================================
			// SHARED MEMORY
			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_CopyFromSharedMemory(string shared_memory_name_str, IntPtr ptr_to_memory, int offset_of_shared_memory, int size_to_copy);

			//============================================================================
			// IMAGE FILE
			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_WriteImageFile(string file_str, IntPtr ptr_data, int width, int height, int pixel_bytes, int data_size); // If 'data_size' is NULL, the actual data size is used as a calculated value.

			//============================================================================
			// LICENSE
			//----------------------------------------------------------------------------
			[DllImport(WSIO_DLL, CallingConvention = CallingConvention.Cdecl)]
			public static extern WSIORV WSUT_CheckLicense_TypeA();

			//====================================================================================================================================
		}
	}
}
