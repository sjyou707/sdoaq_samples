using System;
using System.Runtime.InteropServices;
using System.Text;

/* SDOAQ_EDOF.cs

	Comments : This file exports all types and functions required to directly access the SDO EDoF algorithm.
	Date     : 2025/06/09
	Author   : YoungJu Lee
	Copyright (c) 2019 SD Optics,Inc. All rights reserved.
*/

namespace SDOAQ_EDOF
{
	public static class SDOAQ_EDOF_API
	{
		private const string SDOAQ_DLL = "SDOAQ.dll";

		public enum SDOAQ_EDOF_ErrorCode
		{
			SUCCESS = 1,
			ERROR_NOT_INITIALIZED = -1,
			ERROR_CALIB_DATA = -2,
			ERROR_PROCESS_STOPPED = -3,
			ERROR_INPUT_PARAMETERS = -4,
			ERROR_OUTPUT_PARAMETERS = -5,
			ERROR_INVALID_FOCUS = -6,
			ERROR_NOT_LICENSED = -7,
		}

		/*
		* refer to Shree K. Nayar, "Shape form focus", 1989
		* Modified Laplacian method is recommended for default focus measure
		*/
		public enum SDOAQ_EDOF_FocusMeasure
		{
			MODIFIED_LAPLACIAN,
			GRAYLEVEL_LOCAL_VARIANCE,
			TENENGRAD_GRADIENT,
			CUSTOMIZED1,
			CUSTOMIZED2
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct SDOAQ_EDOF_FocalStackParams
		{
			public SDOAQ_EDOF_FocusMeasure focus_measure; // default = SDOAQ_EDOF_FocusMeasure::MODIFIED_LAPLACIAN

			// input image parameters
			// memory size for one image = image_width * image_height * num_channel * byte_per_channel (bytes)
			public int image_num;
			public int image_width;
			public int image_height;

			// image_offset_x and image_offset_y represent an origin of current image against image sensor origin with full resolution
			// x_fullframe = binning_x * x_local + image_offset_x
			// y_fullframe = binning_y * y_local + image_offset_y
			public int image_offset_x;
			public int image_offset_y;

			public int binning_x;			// 1, 2, 4 (default = 1)
			public int binning_y;			// 1, 2, 4 (default = 1)

			public int num_channel;			// 1, 3, 4
			public int byte_per_channel;	// multi-byte not supported. use only 1
			public int num_padding_bit;     // less than 8

			// ROI bounds (not supported yet)
			public int roi_top;
			public int roi_left;
			public int roi_width;
			public int roi_height;

			// algorithm parameters
			public double resize_ratio; // 0.5, 0.25, 0.125...
			public int pixelwise_kernel_size; // larger kernel size -> smoother depthmap
			public int pixelwise_iteration;
			public int depthwise_kernel_size; // larger kernel size -> less noise, if depthwise_kernel_size == -1, this parameter is automatically adjusted.
			public double depth_quality_th;
			public double bilateral_sigma_color;
			public double bilateral_sigma_space;
			public int num_thread; // -1 for max. num. of thread

			[MarshalAs(UnmanagedType.I1)]
			public bool is_scale_correction_enabled;
			public int scale_correction_dst_step;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct SDOAQ_EDOF_ImageParams
		{
			[MarshalAs(UnmanagedType.I1)]
			public bool is_allocated; //For unnecessary output image, set this false, this parameter will be deprecated
			public int image_width;
			public int image_height;

			// image_offset_x and image_offset_y represent an origin of current image against image sensor origin with full resolution
			// x_fullframe = binning_x * x_local + image_offset_x
			// y_fullframe = binning_y * y_local + image_offset_y
			public int image_offset_x;
			public int image_offset_y;

			public int binning_x;			// 1, 2, 4 (default = 1)
			public int binning_y;			// 1, 2, 4 (default = 1)

			public int num_channel;			// 1, 3, 4
			public int byte_per_channel;	// 1 for unsigned char, 4 for single-precision floating point number
			public int num_padding_bit;     // less than 8

			[MarshalAs(UnmanagedType.I1)]
			public bool is_floating_point; // for floating-point depth
			[MarshalAs(UnmanagedType.I1)]
			public bool is_scale_correction_enabled;
			public int scale_correction_dst_step;
		}

		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int SDOAQ_EDOF_InitializeFromCalibFile(string calibFileName);

        [DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int SDOAQ_EDOF_Finalize();

		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.StdCall)]
		public static extern int SDOAQ_EDOF_GetVersion();

		// return negative number when algorithm failed
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int SDOAQ_EDOF_Run(
            ref SDOAQ_EDOF_FocalStackParams in_params, 
            IntPtr[] in_images, 
            int[] in_steps, 
            ref SDOAQ_EDOF_ImageParams out_edof_params, 
            byte[] out_edof_image
		);
    }
}
