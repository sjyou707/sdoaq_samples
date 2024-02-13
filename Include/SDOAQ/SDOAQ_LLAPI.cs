using System;
using System.Runtime.InteropServices;
using System.Text;

/* SDOAQ_LOWLEVEL.cs

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

namespace SDOAQ
{
	public static partial class SDOAQ_API
	{

        // Register the low level API permission.
        [DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SDOAQ_RegisterLowLevelAuthorization();


        // Set the camera to the content of acquisitionParams
        [DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetAcquisitionFixedParameters(AcquisitionFixedParameters[] acquisitionParams);


        // ['FrameDescriptor' structure]
        //		'typeThis': 'FrameDescriptor' Type. This value is always 1
        //		'bytesPixel': Number of data bytes that make up a pixel of the frame.
        //		'pixelsWidth': Number of width pixels in the frame
        //		'pixelsHeight': Number of height pixels in the frame
        //		'bytesLine': bytes for the With line in the frame. This may include padding bytes. ( padding bytes = 'bytesLine' - 'bytesPixel' * 'pixelsWidth').
        [StructLayout(LayoutKind.Sequential)]
		public struct FrameDescriptor
		{
			public int typeThis;
			public int bytesPixel;
			public int pixelsWidth;
			public int pixelsHeight;
			public int bytesLine;
		};

        // 'SDOAQ_FrameCallback' Call Back Function
        //		'errorCode': ecNoError
        //		'buffer': Frame buffer. When the callback function is returned, the buffer is deleted, requiring a copy of the data.
        //		'bufferSize': buffer size
        //		'frameDescriptor': buffer frame Info 
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void SDOAQ_FrameCallback(eErrorCode errorCode, IntPtr buffer, int bufferSize, ref FrameDescriptor frameDescriptor);

        // Register a callback function that receives frame data : Callback is not accepted when 'singleFrameCb' is NULL.
        [DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetFrameCallback(SDOAQ_FrameCallback singleFrameCb);


		public enum eCameraTriggerMode
		{
			ctmFreerun = 1,
			ctmSoftware = 2,
			ctmExternal = 3
		};
        // Set the camera trigger mode.
        [DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetCameraTriggerMode(eCameraTriggerMode ctm);


		public enum eCameraGrabbingStatus
		{
			cgsOffGrabbing = 0,
			cgsOnGrabbing = 1,
		};
        // Set the camera grab status.
        [DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetCameraGrabbingStatus(eCameraGrabbingStatus cgs);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_GetCameraGrabbingStatus(out eCameraGrabbingStatus cgs);


        // The API below apply only to some cameras.
        public enum eCameraParameterType
		{
			cptValue = 0,
			cptMin = 1,
			cptMax = 2,
			cptInc = 3,
		};

		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_ExecCameraSoftwareTrigger();

		// for CoaXPress, Basler USB, Basler GigE
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetCameraParameterString([MarshalAs(UnmanagedType.LPStr)] string register, [MarshalAs(UnmanagedType.LPStr)] string value);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_GetCameraParameterString([MarshalAs(UnmanagedType.LPStr)] string register, StringBuilder buffer, int bufferSize);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetCameraParameterInteger([MarshalAs(UnmanagedType.LPStr)] string register, long value);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_GetCameraParameterInteger([MarshalAs(UnmanagedType.LPStr)] string register, out long value, eCameraParameterType cpt);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetCameraParameterDouble([MarshalAs(UnmanagedType.LPStr)] string register, double value);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_GetCameraParameterDouble([MarshalAs(UnmanagedType.LPStr)] string register, out double value, eCameraParameterType cpt);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetCameraParameterBool([MarshalAs(UnmanagedType.LPStr)] string register, bool value);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_GetCameraParameterBool([MarshalAs(UnmanagedType.LPStr)] string register, out bool value);

		// for Sentech CameraLink
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetCameraRegisterInteger(IntPtr id_register, long value);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_GetCameraRegisterInteger(IntPtr id_register, out long value_ptr);

		// for Euresys MultiCam grabber
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetGrabberRegisterString(IntPtr id_register, [MarshalAs(UnmanagedType.LPStr)] string sz_value);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_GetGrabberRegisterString(IntPtr id_register, StringBuilder buffer_ptr, int buffer_size);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetGrabberRegisterInteger(IntPtr id_register, long value);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_GetGrabberRegisterInteger(IntPtr id_register, out long value_ptr);
	}
}
