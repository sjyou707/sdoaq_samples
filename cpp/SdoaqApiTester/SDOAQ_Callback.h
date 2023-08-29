#pragma once
#include "..\..\Include\SDOAQ\SDOAQ_WS.h"

void __stdcall g_LogCallback(eLogSeverity severity, char* pMessage);
void __stdcall g_ErrorCallback(eErrorCode errorCode, char* pErrorMessage);
void __stdcall g_ObjectiveChanged(eObjectiveId newObjectiveId);
void __stdcall g_InitDoneCallback(eErrorCode errorCode, char* pErrorMessage);
void __stdcall g_PlayFocusStackCallback(eErrorCode errorCode, int lastFilledRingBufferEntry);
void __stdcall g_PlayEdofCallback(eErrorCode errorCode, int lastFilledRingBufferEntry);
void __stdcall g_PlayAFCallback(eErrorCode errorCode, int lastFilledRingBufferEntry, double dbFocusStep, double dbScore);
void __stdcall g_SnapCallback(eErrorCode errorCode, int lastFilledRingBufferEntry);
