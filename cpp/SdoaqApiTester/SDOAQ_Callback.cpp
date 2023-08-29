#include "stdafx.h"
#include "SDOAQ_Callback.h"
#include "SDOAQ_App.h"
#include "SDOAQ_AppDlg.h"

//----------------------------------------------------------------------------
void __stdcall g_LogCallback(eLogSeverity severity, char* pMessage)
{
	if (theApp.m_pMainWnd)
	{
		theApp.m_pMainWnd->PostMessageW(EUM_LOG, (WPARAM)severity, (LPARAM)NewWString(pMessage));
	}
}

//----------------------------------------------------------------------------
void __stdcall g_ErrorCallback(eErrorCode errorCode, char* pErrorMessage)
{
	if (theApp.m_pMainWnd)
	{
		theApp.m_pMainWnd->PostMessageW(EUM_ERROR, (WPARAM)errorCode, (LPARAM)NewWString(pErrorMessage));
	}
}

//----------------------------------------------------------------------------
void __stdcall g_ObjectiveChanged(eObjectiveId newObjectiveId)
{
	if (theApp.m_pMainWnd)
	{
		const char* pName;
		switch (newObjectiveId)
		{
		case oi00_35x:
			pName = ">> 0.35x objective found.";
			break;
		case oi01_30x:
			pName = ">> 1.30x objective found.";
			break;
		case oi02_50x:
			pName = ">> 2.50x objective found.";
			break;
		case oi00_75x:
			pName = ">> 0.75x objective found.";
			break;
		case oi01_20x:
			pName = ">> 1.20x objective found.";
			break;
		case oi01_80x:
			pName = ">> 1.80x objective found.";
			break;
		case oi00_50x:
			pName = ">> 0.50x objective found.";
			break;
		case oi01_00x:
			pName = ">> 1.00x objective found.";
			break;
		}
		theApp.m_pMainWnd->PostMessageW(EUM_LOG, (WPARAM)lsInfo, (LPARAM)NewWString(pName));
	}
}

//----------------------------------------------------------------------------
void __stdcall g_InitDoneCallback(eErrorCode errorCode, char* pErrorMessage)
{
	if (theApp.m_pMainWnd)
	{
		theApp.m_pMainWnd->PostMessageW(EUM_INITDONE, (WPARAM)errorCode, (LPARAM)NewWString(pErrorMessage));
	}
}

//----------------------------------------------------------------------------
void __stdcall g_PlayFocusStackCallback(eErrorCode errorCode, int lastFilledRingBufferEntry)
{
	if (theApp.m_pMainWnd)
	{
		theApp.m_pMainWnd->PostMessageW(EUM_RECEIVE_ZSTACK, (WPARAM)errorCode, (LPARAM)lastFilledRingBufferEntry);
	}
}

//----------------------------------------------------------------------------
void __stdcall g_PlayEdofCallback(eErrorCode errorCode, int lastFilledRingBufferEntry)
{
	if (theApp.m_pMainWnd)
	{
		theApp.m_pMainWnd->PostMessageW(EUM_RECEIVE_EDOF, (WPARAM)errorCode, (LPARAM)lastFilledRingBufferEntry);
	}
}

//----------------------------------------------------------------------------
void __stdcall g_PlayAFCallback(eErrorCode errorCode, int lastFilledRingBufferEntry, double dbFocusStep, double dbScore)
{
	if (theApp.m_pMainWnd)
	{
		auto pcPara = new tMsgParaReceiveAf;
		pcPara->lastFilledRingBufferEntry = lastFilledRingBufferEntry;
		pcPara->dbFocusStep = dbFocusStep;
		pcPara->dbScore = dbScore;
		theApp.m_pMainWnd->PostMessageW(EUM_RECEIVE_AF, (WPARAM)errorCode, (LPARAM)pcPara);
	}
}

//----------------------------------------------------------------------------
void __stdcall g_SnapCallback(eErrorCode errorCode, int lastFilledRingBufferEntry)
{
	if (theApp.m_pMainWnd)
	{
		theApp.m_pMainWnd->PostMessageW(EUM_RECEIVE_SNAP, (WPARAM)errorCode, (LPARAM)lastFilledRingBufferEntry);
	}
}
