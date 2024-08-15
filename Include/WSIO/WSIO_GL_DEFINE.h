/*	WSIO_GL_DEFINE.h

	Copyright (c) SD Optics,Inc. All rights reserved.

	========================================================================================================================================================
	Revision history
	========================================================================================================================================================
	Version     date      Author         Descriptions
	--------------------------------------------------------------------------------------------------------------------------------------------------------
*/

#ifndef _GL_DEFINE
#define _GL_DEFINE

#define GL_MG_ONSTAGE "onstage"
#define GL_MG_GUEST "guest"

typedef unsigned long long HD3V;
typedef unsigned long RGB4;

//----------------------------------------------------------------------------
enum EKey
{
	EKEY_QUERY = -1,
	EKEY_NULL = NULL,

	/////////////////////////////////////
	//DO NOT CHANGE ESFM_xxxx VALUE

	//enum surface mode (surfacer - mouse left button)
	ESFM_RAWDATA = 101,

	ESFM_CMAP_RAINBOW = 111,
	ESFM_CMAP_RGB = 112,
	ESFM_CMAP_GREY = 113,
	ESFM_CMAP_JET = 114,
	ESFM_CMAP_HSV = 115,

	ESFM_140 = 140,

	ESFM_STATIC_151 = 151,
	ESFM_XYZ_RGB_161 = 161,
	/////////////////////////////////////

	//enum function mode (guider - mouse left button)
	EFUN_NAVIGATION = 0x10101,
	EFUN_MEASURE = 0x10102,
	EFUN_PROFILE = 0x10103,
	EFUN_COMPARE = 0x10104,

	//enum rotation type (guider - mouse right button)
	EROT_3D = 0x10201,
	EROT_PLAIN_XY = 0x10202,

	//enum profile type
	EPFT_XY_Z = 0x1030C,
	EPFT_XZ_Y = 0x1030A,
	EPFT_YZ_X = 0x10306,
	EPFT_RZ_XY = 0x10303, // activation key

	//enum projection mode
	EPJM_ORTHOGRAPHIC = 0x10401,
	EPJM_PERSPECTIVE = 0x10402,

	//enum matrix direction
	EDIR_XY = 0x105C0,
	EDIR_XrY = 0x105C4,

	//test function
	EKEY_DEVELOPER_TEST_BEGIN = 0x10600,
	EKEY_DEVELOPER_TEST_LAST = 0x106FF,

	//enum main color
	ECOLOR_MAIN_BACKGROUND = 0x13011,

	//enum osd color
	EDIR_OSD_COLOR_PICKER_TEXT = 0x14011,
	EDIR_OSD_COLOR_PICKER_BACK = 0x14012,
	EDIR_OSD_COLOR_MEASURE_TEXT = 0x14021,
	EDIR_OSD_COLOR_MEASURE_BACK = 0x14022,
	EDIR_OSD_COLOR_MEASURE_LINE = 0x14023,
	EDIR_OSD_COLOR_INFO_TEXT = 0x14031,
	EDIR_OSD_COLOR_INFO_BACK = 0x14032,
	EDIR_OSD_COLOR_CONSOLE_TEXT = 0x14041,
	EDIR_OSD_COLOR_CONSOLE_BACK = 0x14042,

	//enum user string
	ESTR_FUNCTION = 0x20101,
	ESTR_PROJECTION = 0x20102,
	ESTR_ROTATION = 0x20103,
	ESTR_ZSCALE = 0x20104,

	ESTR_X_KEY_TO_REMOVE_LAST_LINE = 0x20201,
	ESTR_X_KEY_TO_REMOVE_ALL_LINES = 0x20202,
	ESTR_X_KEY_TO_SET_DATUM_PLANE = 0x20203,
};

enum EDisplayAttributes
{
	// 마우스 포인터에 픽커를 표시한다.
	EDA_SHOW_PICKER_ON_MOUSE = (1 << 1),

	// 픽커가 가리키는 포인트의 정보를 보여준다.
	EDA_SHOW_PICKED_POINT_INFORMATION = (1 << 2),

	// 픽커가 3D 물체로 가려지지 않고 항상 보이게 한다.
	EDA_NOHIDE_PICKER = (1 << 3),

	// 왼쪽 버튼 클릭으로 측정을 한다.
	// 설정을 해제하더라도, 그 동안 측정한 데이타는 유지한다.
	EDA_MEASURE_PICKERS_ON_LCLCK_BUTTON = (1 << 4),

	// 측정 데이타 리스트와 포인트 번호를 화면에 표시한다.
	EDA_SHOW_LIST_MEASURED_DATA = (1 << 5),

	// small object, xy plane grid 를 화면에 표시한다.
	EDA_SHOW_GUIDER_OBJECTS = (1 << 6),

	// reserved. 오른쪽 버튼 더블 클릭으로 xx 를 한다.
	EDA_XXX_ON_RDBCLCK_BUTTON = (1 << 7),

	// scale object 를 화면에 표시한다.
	EDA_SHOW_SCALE_OBJECTS = (1 << 8),

	// color map 을 화면에 표시한다.
	EDA_SHOW_COLORMAPBAR_OBJECTS = (1 << 9),

	// 선택된 object 의 outline 을 표시한다.
	EDA_SHOW_OUTLINE = (1 << 10),

	// object 의 교차부분을 표시하지 않는다.
	EDA_HIDE_INTERSECTION = (1 << 11),

	// reserved. 오른쪽 버튼 클릭
	EDA_ON_RCLCK_BUTTON = (1 << 12),

	// bounding box 를 표시한다.
	EDA_SHOW_BOUNDING_BOX = (1 << 13),

	// reserved. set datum plane
	EDA_SET_DATAUM_PLANE = (1 << 14),

	// display transparent objects by blending
	EDA_BLEND = (1 << 15),

	// NEXT
	EDA_NEXT = (1 << 16),
};

// 카메라에서 오는 데이터는 EDM_NDC_XY_ONLY를 설정해서 Z값은 normalizing에서 제외한다. 파일에서 읽은 고정적인 데이터는 설정하지 않으면 기본 동작 - XYZ 모두 normalizing 영향을 준다
enum EDisplayMode
{
	EDM_NULL = 0,

	// { COLOR DATA TYPE }
	// If none is set, it is assumed to be RGB-FLOAT(0~1) COLOR TYPE
	// EDM_BGR_BYTE: BGR-BYTE(0~255) COLOR TYPE
	EDM_BGR_BYTE = (1 << 0),

	// { DIMENSION }
	// EDM_DIMENSION_FIXEDXY_25D: It consists of Z (height) data for the XY plane.
	EDM_DIMENSION_FIXEDXY_25D = (1 << 4),
	// EDM_DIMENSION_CALXY_25D: It consists of Z (height) data for a calibrated XY plane.
	EDM_DIMENSION_CALXY_25D = (1 << 5),

	// { NDC Factor }
	// EDM_NDC_XY_ONLY: Only X and Y data are used for normalizing. When normalizing, the Z axis is excluded. (Because there is inaccurate noise on the Z axis.)
	EDM_NDC_XY_ONLY = (1 << 8),
};

struct tPara_Display25D
{
	unsigned width;
	unsigned height;
	double z_offset1;
	double z_offset2;
	unsigned z_slices;
	double scx1;
	double scx2;
	double scy1;
	double scy2;
	double scz1;
	double scz2;
};

enum { NUMOF_TRIANGLE_INDICES_OF_PLANE_XY_PER_PIXEL = 6 };
enum { NUMOF_TEXCOORD_BUF_NUMBER_PER_PIXEL = 2 };

#endif
