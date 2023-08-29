#pragma once

#include "WSIO.h"
#if !defined(SDP_WSIO)
#include "WSIO_GL_DEFINE.h"
#endif

//====================================================================================================================================
// WSIO Graphic Library Definition & API
//------------------------------------------------------------------------------------------------------------------------------------
//	이 파일(WSIO_GL.h)에는 서비스로 제공되는 3D 그래픽 라이브러리 기능을 추가한다.
//
//------------------------------------------------------------------------------------------------------------------------------------

//============================================================================
// ENUMERATED DEFINITION
//----------------------------------------------------------------------------
typedef WSIOVOID WSGLHANDLE;
typedef int WSGLKEY;

//====================================================================================================================================
// WSIO Graphic Library API
//------------------------------------------------------------------------------------------------------------------------------------
WSIODLL_API int				WSGL_GetMajorVersion(
			void
			);

WSIODLL_API int				WSGL_GetMinorVersion(
			void
			);

WSIODLL_API int				WSGL_GetPatchVersion(
			void
			);

WSIODLL_API WSIORV			WSGL_Initialize(
			WSIOVOID		parent_hwnd,
			WSGLHANDLE*		p_hd3v
			);

WSIODLL_API WSIORV			WSGL_Finalize(
			WSGLHANDLE		hd3v
			);

WSIODLL_API bool			WSGL_IsOnRun(
			WSGLHANDLE		hd3v
			);

WSIODLL_API WSIORV			WSGL_ShowWindow(
			WSGLHANDLE		hd3v,
			bool			show,
			int				left,
			int				top,
			int				right,
			int				bottom
			);

WSIODLL_API WSIORV			WSGL_ResetDisplay(
			WSGLHANDLE		hd3v
			);

WSIODLL_API WSIORV			WSGL_StopDisplay(
			WSGLHANDLE		hd3v
			);

WSIODLL_API WSIORV			WSGL_Display_Welcome(
			WSGLHANDLE		hd3v
			);

WSIODLL_API WSIORV			WSGL_Display_BG(
			WSGLHANDLE		hd3v
			);

WSIODLL_API WSIORV			WSGL_Display_25D(
			WSGLHANDLE		hd3v,
			const char*		sz_group,
			const char*		sz_name,
			float*			p_vertex,
			unsigned		nums_vertex_data,
			void*			p_image,
			unsigned		nums_image_data,
			int				mode,
			float			unit_microns,
			const tPara_Display25D* p_para
			);

WSIODLL_API WSIORV			WSGL_Display_3D(
			WSGLHANDLE		hd3v,
			const char*		sz_group,
			const char*		sz_name,
			float*			p_vertex,
			unsigned		nums_vertex_data,
			void*			p_image,
			unsigned		nums_image_data,
			unsigned*		p_face,
			unsigned		nums_face,
			int				mode,
			float			unit_microns
			);

WSIODLL_API WSIORV			WSGL_Display_3D_File(
			WSGLHANDLE		hd3v,
			const char*		sz_group,
			const char*		sz_path,
			int				mode,
			float			unit_microns
			);

WSIODLL_API WSIORV			WSGL_Save_3D(
			WSGLHANDLE		hd3v,
			const char*		sz_inpath,
			const char*		sz_outpath
			);

WSIODLL_API WSIORV			WSGL_UpdateUnitMicrons(
			WSGLHANDLE		hd3v,
			const char*		sz_group,
			float			unit_microns
			);

WSIODLL_API WSIORV			WSGL_ClearObject(
			WSGLHANDLE		hd3v,
			const char*		sz_group
			);

WSIODLL_API WSIORV			WSGL_DoRendering(
			WSGLHANDLE		hd3v
			);

WSIODLL_API WSIORV			WSGL_ClearScreen(
			WSGLHANDLE		hd3v
			);

//----------------------------------------------------------------------------
WSIODLL_API WSIORV			WSGL_SetDisplayAttributes(
			WSGLHANDLE		hd3v,
			int				attributes
			);

WSIODLL_API WSIORV			WSGL_GetDisplayAttributes(
			WSGLHANDLE		hd3v,
			int*			p_attributes
			);

WSIODLL_API WSIORV			WSGL_RotationType(
			WSGLHANDLE		hd3v,
			WSGLKEY*		p_rotation_type
			);

WSIODLL_API WSIORV			WSGL_SetFunctionMode(
			WSGLHANDLE		hd3v,
			int				function_mode,
			const int*		p_mode_list,
			int				mode_nums
			);

WSIODLL_API WSIORV			WSGL_RegisterFunctionModeCallback(
			WSGLHANDLE		hd3v,
			WSIOVOID		hwnd,
			unsigned		msg
			);

WSIODLL_API WSIORV			WSGL_SetProjectionMode(
			WSGLHANDLE		hd3v,
			WSGLKEY			projection_mode
			);

WSIODLL_API WSIORV			WSGL_RegisterProjectionModeCallback(
			WSGLHANDLE		hd3v,
			WSIOVOID		hwnd,
			unsigned		msg
			);

WSIODLL_API WSIORV			WSGL_SetSurfaceMode(
			WSGLHANDLE		hd3v,
			WSGLKEY			surface_mode
			);

WSIODLL_API WSIORV			WSGL_RegisterSurfaceModeCallback(
			WSGLHANDLE		hd3v,
			WSIOVOID		hwnd,
			unsigned		msg
			);

WSIODLL_API bool			WSGL_SetImageColorRatio(
			WSGLHANDLE		hd3v,
			float			image_ratio
			);

WSIODLL_API WSIORV			WSGL_GetImageColorRatio(
			WSGLHANDLE		hd3v,
			float*			p_image_ratio
			);

WSIODLL_API WSIORV			WSGL_GetMeasureData(
			WSGLHANDLE		hd3v,
			char*			p_buf,
			int				buf_size,
			const char*		sz_delimiter,
			int*			p_written_size
			);

WSIODLL_API WSIORV			WSGL_ClearAllMeasureData(
			WSGLHANDLE		hd3v,
			bool			flagRendering
			);

WSIODLL_API WSIORV			WSGL_ClearLastMeasureData(
			WSGLHANDLE		hd3v,
			bool			flagRendering
			);

WSIODLL_API WSIORV			WSGL_SetProfileValue(
			WSGLHANDLE		hd3v,
			WSGLKEY			type,
			double			db0to1,
			bool			o_rendering
			);

WSIODLL_API WSIORV			WSGL_GetProfileValue(
			WSGLHANDLE		hd3v,
			WSGLKEY			type,
			double*			p_value
			);

WSIODLL_API WSIORV			WSGL_RegisterProfileValueUpdateCallback(
			WSGLHANDLE		hd3v,
			WSIOVOID		hwnd,
			unsigned		msg
			);

WSIODLL_API WSIORV			WSGL_SetGradient(
			WSGLHANDLE		hd3v,
			double			dx,
			double			dy,
			double			dz
			);

//----------------------------------------------------------------------------
WSIODLL_API WSIORV			WSGL_SetInfoString(
			WSGLHANDLE		hd3v,
			const char*		sz_info
			);

WSIODLL_API const char*		WSGL_GetLastErrorString(
			WSGLHANDLE		hd3v
			);

WSIODLL_API WSIORV			WSGL_SetUserString(
			WSGLKEY			ekey,
			const char*		sz_str
			);

WSIODLL_API WSIORV			WSGL_GetUserString(
			WSGLKEY			ekey,
			char*			p_buf,
			int				size
			);

//----------------------------------------------------------------------------
// USEFUL FUNCTION
//----------------------------------------------------------------------------
// Create triangle indices for all (or jumped by mag) points in the x, y matrix.
// 'indices_nums' must be 'width' x 'height' x NUMOF_TRIANGLE_INDICES_OF_PLANE_XY_PER_PIXEL or bigger.
WSIODLL_API WSIORV			WSGL_GenerateTriangleIndicesOfPlaneXY(
			unsigned*		p_wrtten_nums,
			unsigned*		p_indices,
			unsigned		indices_nums,
			unsigned		width,
			unsigned		height,
			int				mag,
			float*			p_vertex
			);

// 'buf_nums' must be 'width' x 'height' x NUMOF_TEXCOORD_BUF_NUMBER_PER_PIXEL or bigger.
WSIODLL_API WSIORV			WSGL_GenerateFixedXY25D_Texcoord(
			unsigned		width,
			unsigned		height,
			int				dirs,
			float*			buf,
			unsigned		buf_nums
			);


//====================================================================================================================================
