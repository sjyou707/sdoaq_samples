#include "stdafx.h"
#include "SDOAQ_CalibrationFile.h"
#include "SDOAQ_App.h"
#include "SDOAQ_AppDlg.h"

#include <fstream>
#include <algorithm>
#include <sstream>

//================================================================================================
// CALIBRATION
//------------------------------------------------------------------------------------------------
bool SDOAQ_CalibrationFile::GetCalibList(const CString& sDirPath, std::vector<CString>& vsFileList)
{
	WIN32_FIND_DATA fileData;
	HANDLE			hDir;

	if ((hDir = FindFirstFile((sDirPath + "/*"), &fileData)) == INVALID_HANDLE_VALUE)
	{
		FindClose(hDir);
		return false;
	}

	vsFileList.clear();

	while (FindNextFile(hDir, &fileData))
	{
		if (strcmp((const char *)fileData.cFileName, ".") != 0 && strcmp((const char *)fileData.cFileName, "..") != 0)
		{
			const CString sExt = CString(fileData.cFileName).Right(4);
			if (sExt.CompareNoCase(_T(".csv")))
			{
				continue;
			}

			vsFileList.push_back(FString(sDirPath + "\\" + fileData.cFileName));
		}
	}

	FindClose(hDir);

	return true;
}

//------------------------------------------------------------------------------------------------
bool SDOAQ_CalibrationFile::BuildCalibData(const CString& sFileName, std::vector<CalibV2>& vCalib)
{
	std::ifstream calib_file_stream;
	calib_file_stream.open(sFileName);

	if (calib_file_stream.is_open())
	{
		std::string line;
		CalibV2 calib;

		auto CHECK_ITEM = [&](CString& sItemName)
		{
			getline(calib_file_stream, line);
			return ((CString)CA2T(line.c_str())).MakeUpper().Find(sItemName);
		};

		auto GET_VALUE = [&](CString& sItemName)
		{
			if (-1 == CHECK_ITEM(sItemName))
			{
				return CString();
			}

			auto value = line.substr(line.find("=") + 1);
			value.erase(remove(value.begin(), value.end(), ' '), value.end());
			value.erase(remove(value.begin(), value.end(), ','), value.end());
			return (CString)value.c_str();
		};

		// Title
		if (-1 == CHECK_ITEM((CString)CALIBRATION_TITLE))
		{
			calib_file_stream.close();
			return false;
		}

		// Version
		auto version = _ttof(GET_VALUE((CString)CALIB_VERSION));
		if (_ttof(CALIB_VERSION_NUMBER) != version)
		{
			theApp.m_pMainWnd->PostMessageW(EUM_LOG, (WPARAM)lsWarning, (LPARAM)NewWString(FStringA(">> The version of the %s file does not match. (version:%.1lf) ", std::string(CT2A(sFileName)).c_str(), version)));
			return false;
		}

		/// Objective
		getline(calib_file_stream, line);

		/// SerialNumber
		getline(calib_file_stream, line);

		// AddOn
		calib.objective = GET_VALUE((CString)CALIB_ADDON_OBJ);

		/// AddOnSerialNumber
		getline(calib_file_stream, line);

		/// Date
		getline(calib_file_stream, line);

		// SensorResolutionHorizontal
		calib.sensor_width = _ttoi(GET_VALUE((CString)CALIB_SENSOR_WIDTH));

		// SensorResolutionVertical
		calib.sensor_height = _ttoi(GET_VALUE((CString)CALIB_SENSOR_HEIGHT));

		/// ValidStepMin
		getline(calib_file_stream, line);

		/// ValidStempMax
		getline(calib_file_stream, line);

		/// HeightFit
		getline(calib_file_stream, line);

		/// PixelPitch
		getline(calib_file_stream, line);

		// FieldCurvature
		auto curvatureType = GET_VALUE((CString)CALIB_FIELD_CURVATURE);
		if (0 == curvatureType.CompareNoCase(CURVATURE_POLY22))
		{
			for (int i = 0; i < CURVATURE_FIELDS_SIZE; i++)
			{
				calib.fieldCurvatureCoefs[i] = _ttof(GET_VALUE(CString("")));
			}
		}

		/// CalibTable 
		getline(calib_file_stream, line);

		/// NumOfRowsExceptHeader 
		getline(calib_file_stream, line);

		/// NumOfCols 
		getline(calib_file_stream, line);

		// Table header
		if (0 == CHECK_ITEM((CString)CALIB_TABLE_HEADER))
		{
			// CalibTable Data
			PerStepCalibDataV2 calibTbl;
			while (getline(calib_file_stream, line))
			{
				std::string value;
				std::stringstream linestream(line);

				getline(linestream, value, ',');
				calibTbl.step = stoi(value);

				getline(linestream, value, ',');
				calibTbl.obj_height = stod(value);

				getline(linestream, value, ',');
				calibTbl.pixel_pitch_x = stod(value);

				getline(linestream, value, ',');
				calibTbl.pixel_pitch_y = stod(value);

				getline(linestream, value, ',');
				calibTbl.scale_x = stod(value);

				getline(linestream, value, ',');
				calibTbl.scale_y = stod(value);

				getline(linestream, value, ',');
				calibTbl.shift_x = stod(value);

				getline(linestream, value, ',');
				calibTbl.shift_y = stod(value);

				calib.calibTable.push_back(calibTbl);
			}
		}

		vCalib.push_back(calib);

		calib_file_stream.close();
	}
	else
	{
		return false;
	}

	return true;
}