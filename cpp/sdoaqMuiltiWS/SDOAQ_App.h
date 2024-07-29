
// SDOAQ_App.h : main header file for the PROJECT_NAME application
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// CSDOAQ_App:
// See SDOAQ_App.cpp for the implementation of this class
//

class CSDOAQ_App : public CWinApp
{
public:
	CSDOAQ_App();

// Overrides
public:
	virtual BOOL InitInstance();

// Implementation

	DECLARE_MESSAGE_MAP()
};

extern CSDOAQ_App theApp;