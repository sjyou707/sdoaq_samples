using System;
using System.Runtime.InteropServices;
using System.Text;

/* SDOAQ_MULTIWS.cs

	Comments : This file exports all types and functions required to access the SDO acquisition engine configured with multiple WiseScopes.
	Date     : 2024/03/19
	Author   : YoungJu Lee
	Copyright (c) 2019 SD Optics,Inc. All rights reserved.

	========================================================================================================================================================
	Revision history
	========================================================================================================================================================
	Version     date      Author         Descriptions
	--------------------------------------------------------------------------------------------------------------------------------------------------------
	 2.6.0  2024.03.19  YoungJu Lee     - Init
	--------------------------------------------------------------------------------------------------------------------------------------------------------
*/

namespace SDOAQ
{
	public static partial class SDOAQ_API
	{
		// Registers multiple wisescopes uses. This function must be called before initializing SDOAQ.
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SDOAQ_RegisterMultiWsApi();

		public enum eWiseScopeID { MULTI_WS_ALL = 0, MULTI_WS_FIRST = 1 };
		// Among multiple wisescopes, select the wisescope to use by ID. This function is not supported unless you register multiple wisescopes uses.
		// 'multi_ws_id'(Multi WS ID) is started from 1(MULTI_WS_FIRST). 0 means all.
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern eErrorCode SDOAQ_SelectMultiWs(int multi_ws_id);

		// Returns the wisescope ID from which the result was acquired.
		// This function can be called within a camera-related callback function to find out the wisescope ID that generated the callback.
		// Return value(Multi WS ID) is started from 1(MULTI_WS_FIRST). 0 means no information.
		[DllImport(SDOAQ_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SDOAQ_GetCallbackMultiWs();
	}
}