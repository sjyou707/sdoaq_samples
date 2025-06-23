/* SDOAQ_EDOF.h

	Comments : This file exports all types and functions required to directly access the SDO EDoF algorithm.
	Date     : 2025/06/09
	Author   : YoungJu Lee
	Copyright (c) 2019 SD Optics,Inc. All rights reserved.
*/

#pragma once

#ifdef __cplusplus
extern "C"
{
#endif
	
	enum SDOAQ_EDOF_ErrorCode
	{
		SUCCESS = 1,
		ERROR_NOT_INITIALIZED = -1,
		ERROR_CALIB_DATA = -2,
		ERROR_PROCESS_STOPPED = -3,
		ERROR_INPUT_PARAMETERS = -4,
		ERROR_OUTPUT_PARAMETERS = -5,
		ERROR_INVALID_FOCUS = -6,
		ERROR_NOT_LICENSED = -7,
	};

	/*
	* refer to Shree K. Nayar, "Shape form focus", 1989
	* Modified Laplacian method is recommended for default focus measure
	*/
	enum class SDOAQ_EDOF_FocusMeasure
	{
		MODIFIED_LAPLACIAN,
		GRAYLEVEL_LOCAL_VARIANCE,
		TENENGRAD_GRADIENT,
		CUSTOMIZED1,
		CUSTOMIZED2
	};

	struct SDOAQ_EDOF_FocalStackParams
	{
		SDOAQ_EDOF_FocusMeasure focus_measure; // default = SDOAQ_EDOF_FocusMeasure::MODIFIED_LAPLACIAN

		// input image parameters
		// memory size for one image = image_width * image_height * num_channel * byte_per_channel (bytes)
		int image_num;
		int image_width;
		int image_height;

		// image_offset_x and image_offset_y represent an origin of current image against image sensor origin with full resolution
		// x_fullframe = binning_x * x_local + image_offset_x
		// y_fullframe = binning_y * y_local + image_offset_y
		int image_offset_x;
		int image_offset_y;

		int binning_x = 1;		// 1, 2, 4 (default = 1)
		int binning_y = 1;		// 1, 2, 4 (default = 1)

		int num_channel;		// 1, 3, 4
		int byte_per_channel;	// 1, 2,...
		int num_padding_bit;	// less than 8

		// ROI bounds (not supported yet)
		int roi_top;
		int roi_left;
		int roi_width;
		int roi_height;

		/*
		range = {0.25f, 0.5f, 1.0f}
		default = 0.5f
		*/
		double resize_ratio;

		/*
		range = {3 ~ 15}
		recommendation
		default = 3 (when resize_ratio = 0.25f)
		default = 5 (when resize_ratio = 0.5f)
		default = 9 (when resize_ratio = 1.0f)
		*/
		int pixelwise_kernel_size;

		/*
		range = {0 ~ 16}
		default = 4
		*/
		int pixelwise_iteration;

		/*
		range = {-1 ~ 20},
		default = -1
		if depthwise_kernel_size == -1, this parameter is automatically adjusted.
		*/
		int depthwise_kernel_size; // larger kernel size -> less noise

		/*
		range = {0.0f ~ 9.9f}
		default = 2.0f
		*/
		double depth_quality_th;

		/*
		range = {0 ~ 9}
		default = 4
		if bilateral_sigma_color <= 0, bilateral filtering is replaced with Gaussian filtering
		*/
		double bilateral_sigma_color;

		/*
		range = {0 ~ 15}
		default = 3 (when resize_ratio = 0.25f)
		default = 5 (when resize_ratio = 0.5f)
		default = 9 (when resize_ratio = 1.0f)
		if bilateral_sigma_space <= 0, bilateral filtering is disabled.
		*/
		double bilateral_sigma_space;

		int num_thread; // -1 for max. num. of thread

		/*
		* this flag enables image scale correction
		* more computation needed
		* edof image has a constant pixel pitch which refer to scale_correction_dst_step
		*/
		bool is_scale_correction_enabled; // default = false;

		/*
		* reference MALS step for image scale correction
		* range = {MALS_MIN_STEP ~ MALS_MAX_STEP}
		*/
		int scale_correction_dst_step;
	};

	struct SDOAQ_EDOF_ImageParams
	{
		bool is_allocated; //For unnecessary output image, set this false, this parameter will be deprecated
		int image_width;
		int image_height;

		// image_offset_x and image_offset_y represent an origin of current image against image sensor origin with full resolution
		// x_fullframe = binning_x * x_local + image_offset_x
		// y_fullframe = binning_y * y_local + image_offset_y
		int image_offset_x;
		int image_offset_y;

		int binning_x = 1;		// 1, 2, 4 (default = 1)
		int binning_y = 1;		// 1, 2, 4 (default = 1)

		int num_channel;		// 1, 3, 4
		int byte_per_channel;	// 1, 2,...
		int num_padding_bit;	// less than 8
		bool is_floating_point;	// for floating-point depth

		// if SDEDOF_FocalStackParams.is_scale_correction_enabled == true, set this flag true, too
		bool is_scale_correction_enabled; // default = false;

		/*
		* must be same with SDEDOF_FocalStackParams.scale_correction_dst_step
		* reference MALS step for image scale correction
		* range = {MALS_MIN_STEP ~ MALS_MAX_STEP}
		*/
		int scale_correction_dst_step;
	};

	__declspec(dllexport) int SDOAQ_EDOF_InitializeFromCalibFile(const char* calib_file_name);

	__declspec(dllexport) int SDOAQ_EDOF_Finalize();

	__declspec(dllexport) int SDOAQ_EDOF_GetVersion();

	// return negative number when algorithm failed
	__declspec(dllexport) int SDOAQ_EDOF_Run(
		SDOAQ_EDOF_FocalStackParams* in_params, unsigned char** in_images, unsigned int* in_steps,
		SDOAQ_EDOF_ImageParams* out_edof_params, unsigned char* out_edof_image
	);

#ifdef __cplusplus
}
#endif
