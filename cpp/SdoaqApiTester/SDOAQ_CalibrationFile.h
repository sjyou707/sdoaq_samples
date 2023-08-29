#pragma once

#include <vector>

#define CALIBRATION_TITLE			_T("SD OPTICS WISESCOPE CALIBRATION")

#define CALIB_VERSION				_T("VERSION")
#define CALIB_BASIC_OBJ				_T("OBJECTIVE")
#define CALIB_ADDON_OBJ				_T("ADDON")
#define CALIB_SENSOR_WIDTH			_T("SENSORRESOLUTIONHORIZONTAL")
#define CALIB_SENSOR_HEIGHT			_T("SENSORRESOLUTIONVERTICAL")
#define CALIB_FIELD_CURVATURE		_T("FIELDCURVATURE")
#define CALIB_CALIBTABLE			_T("CALIBTABLE")
#define CALIB_ROWS					_T("NUMOFROWSEXCEPTHEADER")
#define CALIB_COLS					_T("NUMOFCOLS")
#define CALIB_TABLE_HEADER			_T("STEP,OBJ_HEIGHT,PIXEL_PITCH_X,PIXEL_PITCH_Y,SCALE_X,SCALE_Y,SHIFT_X,SHIFT_Y")

#define CURVATURE_POLY22			_T("POLY22")
#define CURVATURE_FIELDS_SIZE		6

#define CALIB_VERSION_NUMBER		_T("2.1")

class SDOAQ_CalibrationFile
{
private:
	/*
	calibration factors for each MALS step
	*/
	struct PerStepCalibDataV2
	{
		// MALS step (0 ~ 319)
		int step;

		// height of object surface
		double obj_height;

		// horizontal pitch of a pixel in objective side
		double pixel_pitch_x;

		// vertical pitch of a pixel in objective side
		double pixel_pitch_y;

		// horizontal scale factor of current MALS step compared with reference MALS step(160)
		double scale_x;

		// vertical scale factor of current MALS step compared with reference MALS step(160)
		double scale_y;

		// amount of horizontal shift related to scale factor
		double shift_x;

		// amount of vertical shift related to scale factor
		double shift_y;

		/*
		Similarity transformation of an image at a certain MALS step against to reference MALS step(160)

		|scale_x     0     shift_x|
		|   0     scale_y  shift_y|
		|   0        0        1   |
		*/
	};

	struct CalibV2
	{
		CString objective;
		int sensor_width;
		int sensor_height;
		double fieldCurvatureCoefs[CURVATURE_FIELDS_SIZE];
		std::vector<PerStepCalibDataV2> calibTable;
	};

public:
	std::vector<CalibV2> calibData;

	bool GetCalibList(const CString& sDirPath, std::vector<CString>& vsFileList);
	bool BuildCalibData(const CString& sFileName, std::vector<CalibV2>& calib);
};