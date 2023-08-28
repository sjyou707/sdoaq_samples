using System;


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
	 2.3.0  2022.08.25  YoungJu Lee     - Init
	--------------------------------------------------------------------------------------------------------------------------------------------------------
*/

namespace SDOAQ
{
	public static partial class SDOAQ_API
	{

		// low level API 사용 권한을 등록한다.
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SDOAQ_RegisterLowLevelAuthorization();


		// 'pAcquisitionParams'의 내용으로 카메라를 설정한다.
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetAcquisitionFixedParameters(AcquisitionFixedParameters[] pAcquisitionParams);


		// ['FrameDescriptor' structure]
		//		'typeThis': 'FrameDescriptor'의 종류. 이 값은 항상 1이다.
		//		'bytesPixel': Frame의 한 픽셀을 이루는 데이터 바이트 수.
		//		'pixelsWidth': Frame의 가로 픽셀 수
		//		'pixelsHeight': Frame의 세로 픽셀 수
		//		'bytesLine': Frame의 가로 줄의 데이터 바이트. 여기에는 패딩 바이트가 포함될 수 있다 ( padding bytes = 'bytesLine' - 'bytesPixel' * 'pixelsWidth').
		[StructLayout(LayoutKind.Sequential)]
		public struct FrameDescriptor
		{
			public int typeThis;
			public int bytesPixel;
			public int pixelsWidth;
			public int pixelsHeight;
			public int bytesLine;
		};

		// 'SDOAQ_FrameCallback' 콜백 함수
		//		'errorCode': ecNoError
		//		'pBuffer': 프레임 버퍼 포인터. 콜백함수를 리턴하면 버퍼는 더 이상 유효하지 않으므로 콜백 함수 안에서 데이터를 취득해야 한다.
		//		'BufferSize': 'pBuffer'의 버퍼 크기
		//		'pFrameDescriptor': 'pBuffer'의 버퍼에 담긴 프레임 정보, 이 값이 NULL이면 정보가 없는 것이임.
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void SDOAQ_FrameCallback(eErrorCode errorCode, byte[] pBuffer, size_t BufferSize, FrameDescriptor* pFrameDescriptor);
		
		// 프레임 데이터를 받는 콜백함수를 등록한다: 'singleFrameCb'이 NULL일 때는 콜백을 받지 않는다.
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetFrameCallback(SDOAQ_FrameCallback singleFrameCb);


		public enum eCameraTriggerMode
		{
			ctmFreerun = 1,
			ctmSoftware = 2,
			ctmExternal = 3
		};
		// 카메라 트리거 모드를 설정한다.
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetCameraTriggerMode(eCameraTriggerMode ctm);


		// 아래 API들은 일부 카메라에서만 적용된다.
		public enum eCameraParameterType
		{
			cptValue = 0,
			cptMin = 1,
			cptMax = 2,
			cptInc = 3,
		};

		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_ExecCameraSoftwareTrigger();
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetCameraParameterString([MarshalAs(UnmanagedType.LPStr)] string sz_register, [MarshalAs(UnmanagedType.LPStr)] string sz_value);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_GetCameraParameterString([MarshalAs(UnmanagedType.LPStr)] string sz_register, StringBuilder buffer_ptr, int buffer_size);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetCameraParameterInteger([MarshalAs(UnmanagedType.LPStr)] string sz_register, long value);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_GetCameraParameterInteger([MarshalAs(UnmanagedType.LPStr)] string sz_register, long[] value_ptr, eCameraParameterType cpt);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetCameraParameterDouble([MarshalAs(UnmanagedType.LPStr)] string sz_register, double value);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_GetCameraParameterDouble([MarshalAs(UnmanagedType.LPStr)] string sz_register, double[] value_ptr, eCameraParameterType cpt);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SetCameraParameterBool([MarshalAs(UnmanagedType.LPStr)] string sz_register, int value);
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_GetCameraParameterBool([MarshalAs(UnmanagedType.LPStr)] string sz_register, int[] value_ptr);
	}
}
