/* SDOAQ_LOWLEVEL.h

	Comments : This file exports all types and functions needed to access the SDO acquisition engine.
	Date     : 2023/08/25
	Author   : YoungJu Lee
	Copyright (c) 2019 SD Optics,Inc. All rights reserved.

	========================================================================================================================================================
	Revision history
	========================================================================================================================================================
	Version     date      Author         Descriptions
	--------------------------------------------------------------------------------------------------------------------------------------------------------
	 2.3.0  2022.08.25  YoungJu Lee     - Init (Camera register setting APIs are only valid for Basler USB and Basler GigE)
	--------------------------------------------------------------------------------------------------------------------------------------------------------
	 T2024.02.08		YoungJu Lee		- Supports CoaXPress type, Sentech CameraLink and Euresys MultiCam grabber
	--------------------------------------------------------------------------------------------------------------------------------------------------------
*/

#pragma once
#include "SDOAQ_WS.h"

#ifdef __cplusplus
extern "C"
{
#endif

	// Register the low level API permission.
	__declspec(dllexport) void SDOAQ_RegisterLowLevelAuthorization(void);


	// Set the camera to the content of pAcquisitionParams
	__declspec(dllexport) eErrorCode SDOAQ_SetAcquisitionFixedParameters(sAcquisitionFixedParameters* pAcquisitionParams);


	// ['FrameDescriptor' structure]
	//		'typeThis': 'FrameDescriptor' Type. This value is always 1
	//		'bytesPixel': Number of data bytes that make up a pixel of the frame.
	//		'pixelsWidth': Number of width pixels in the frame
	//		'pixelsHeight': Number of height pixels in the frame
	//		'bytesLine': bytes for the With line in the frame. This may include padding bytes. ( padding bytes = 'bytesLine' - 'bytesPixel' * 'pixelsWidth').
	typedef struct
	{
		int typeThis;
		int bytesPixel;
		int pixelsWidth;
		int pixelsHeight;
		int bytesLine;
	} FrameDescriptor;

	// 'SDOAQ_FrameCallback' Call Back Function
	//		'errorCode': ecNoError
	//		'buffer': Frame buffer. When the callback function is returned, the buffer is deleted, requiring a copy of the data.
	//		'bufferSize': buffer size
	//		'frameDescriptor': buffer frame Info 
	typedef void(__stdcall* SDOAQ_FrameCallback)(eErrorCode errorCode, unsigned char* pBuffer, size_t BufferSize, FrameDescriptor* pFrameDescriptor);
	// Register a callback function that receives frame data : Callback is not accepted when 'singleFrameCb' is NULL.
	__declspec(dllexport) eErrorCode SDOAQ_SetFrameCallback(SDOAQ_FrameCallback singleFrameCb);


	enum eCameraTriggerMode
	{
		ctmFreerun = 1,
		ctmSoftware = 2,
		ctmExternal = 3
	};
	// Set the camera trigger mode.
	__declspec(dllexport) eErrorCode SDOAQ_SetCameraTriggerMode(eCameraTriggerMode ctm);


	enum eCameraGrabbingStatus
	{
		cgsOffGrabbing = 0,
		cgsOnGrabbing = 1,
	};
	// Set the camera grab status.
	__declspec(dllexport) eErrorCode SDOAQ_SetCameraGrabbingStatus(eCameraGrabbingStatus cgs);
	__declspec(dllexport) eErrorCode SDOAQ_GetCameraGrabbingStatus(eCameraGrabbingStatus* cgs_ptr);


	// The API below apply only to some cameras.
	enum eCameraParameterType
	{
		cptValue = 0,
		cptMin = 1,
		cptMax = 2,
		cptInc = 3,
	};

	__declspec(dllexport) eErrorCode SDOAQ_ExecCameraSoftwareTrigger(void);

	// for CoaXPress, Basler USB, Basler GigE
	__declspec(dllexport) eErrorCode SDOAQ_SetCameraParameterString(const char* sz_register, const char* sz_value);
	__declspec(dllexport) eErrorCode SDOAQ_GetCameraParameterString(const char* sz_register, char* buffer_ptr, int buffer_size);
	__declspec(dllexport) eErrorCode SDOAQ_SetCameraParameterInteger(const char* sz_register, long long value);
	__declspec(dllexport) eErrorCode SDOAQ_GetCameraParameterInteger(const char* sz_register, long long* value_ptr, eCameraParameterType cpt);
	__declspec(dllexport) eErrorCode SDOAQ_SetCameraParameterDouble(const char* sz_register, double value);
	__declspec(dllexport) eErrorCode SDOAQ_GetCameraParameterDouble(const char* sz_register, double* value_ptr, eCameraParameterType cpt);
	__declspec(dllexport) eErrorCode SDOAQ_SetCameraParameterBool(const char* sz_register, bool value);
	__declspec(dllexport) eErrorCode SDOAQ_GetCameraParameterBool(const char* sz_register, bool* value_ptr);

	// for Sentech CameraLink
	__declspec(dllexport) eErrorCode SDOAQ_SetCameraRegisterInteger(void* id_register, long long value);
	__declspec(dllexport) eErrorCode SDOAQ_GetCameraRegisterInteger(void* id_register, long long* value_ptr);

	// for Euresys MultiCam grabber
	__declspec(dllexport) eErrorCode SDOAQ_SetGrabberRegisterString(void* id_register, const char* sz_value);
	__declspec(dllexport) eErrorCode SDOAQ_GetGrabberRegisterString(void* id_register, char* buffer_ptr, int buffer_size);
	__declspec(dllexport) eErrorCode SDOAQ_SetGrabberRegisterInteger(void* id_register, long long value);
	__declspec(dllexport) eErrorCode SDOAQ_GetGrabberRegisterInteger(void* id_register, long long* value_ptr);

#ifdef __cplusplus
}
#endif