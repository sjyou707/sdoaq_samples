using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using SDOAQ;
using SDOAQCSharp.Tool;

namespace SDOAQCSharp
{
    partial class MySdoaq
    {
        public bool AcquisitionStop()
        {
            if (IsRunPlayer == false)
            {
                return true;
            }

            switch (PlayerMode)
            {
                case emPlayerMode.Edof:
                    AcquisitionStop_Edof();
                    break;
                case emPlayerMode.Af:
                    AcquisitionStop_Af();
                    break;
                case emPlayerMode.FocusStack:
                    AcquisitionStop_FocusStack();
                    break;
            }
            return true;
        }

        public bool Acquisition_Sanp(string directoryPath)
        {
            if (IsInitialize == false || IsRunPlayer == false)
            {
                WriteLog(Logger.emLogLevel.Warning, $"Acquisition_Sanp(), Run False. IsInitialize = {IsInitialize}, PlayerMode = {PlayerMode}");
                return false;
            }

            var focusList = SnapFocusList.GetStepList();
            
            var snapParams = new SDOAQ_API.SnapParameters[1]
            {
                new SDOAQ_API.SnapParameters()
                {
                    version = (IntPtr)2,
                    v2 = new SDOAQ_API.SnapParameters.Ver2()
                    {
                        sSnapPath = directoryPath,
                        sConfigFilename = null,
                        sConfigData = null,
                    }
                }
            };

            SelectMultWS(CamIndex);

            var rv = SDOAQ_API.SDOAQ_PlaySnap(CallBack_SDOAQ_Snap, focusList, focusList.Length, snapParams);

            return rv != SDOAQ_API.eErrorCode.ecNoError;
        }

        #region Focus Stack
        public async Task<bool> Acquisition_FocusStackAsync()
        {
            if (IsInitialize == false || IsRunPlayer)
            {
                WriteLog(Logger.emLogLevel.Warning, $"Acquisition_FocusStackAsync(), Run False. IsInitialize = {IsInitialize}, PlayerMode = {PlayerMode}");
                return false;
            }

            var acqParamList = new SDOAQ_API.AcquisitionFixedParameters[]
            {
                CamInfo.AcqParam
            };

            var focusList = FocusList.GetStepList();

            var rv = await Task.Run(() => Acq_FocusStack(acqParamList, focusList, true));
            
            return rv;
        }

        private bool Acq_FocusStack(SDOAQ_API.AcquisitionFixedParameters[] acqParamList, int[] focusList, bool bShowLog)
        {
            byte[][] imageBuffer = null;
            imageBuffer = new byte[focusList.Length][];

            var focusImagePointerList = new IntPtr[focusList.Length];
            var focusImageBufferSizeList = new ulong[focusList.Length];

            ref var acqParam = ref acqParamList[0];

            int sizeOfImage = acqParam.cameraRoiHeight * acqParam.cameraRoiWidth * CamInfo.ColorByte;

            for (int focus = 0; focus < focusList.Length; focus++)
            {
                imageBuffer[focus] = new byte[sizeOfImage];
                focusImageBufferSizeList[focus] = (ulong)sizeOfImage;
                unsafe
                {
                    fixed (byte* pointerToFirst = imageBuffer[focus])
                    {
                        focusImagePointerList[focus] = new IntPtr(pointerToFirst);
                    }
                }
            }

            long tickStart = Environment.TickCount;

            SelectMultWS(CamIndex);
            var rvSdoaq = SDOAQ_API.SDOAQ_SingleShotFocusStack(
                acqParamList,
                focusList, focusList.Length,
                focusImagePointerList, focusImageBufferSizeList);

            long elapsedTime = Environment.TickCount - tickStart;

            if (bShowLog)
            {
                WriteLog(Logger.emLogLevel.Info, $"[Info] SingleShotFocusStack(), rv = {rvSdoaq}, Acquisition Time = {elapsedTime} ms ({((double)focusList.Length / elapsedTime * 1000):F3} fps)");
            }
            else if (rvSdoaq != SDOAQ_API.eErrorCode.ecNoError)
            {
                WriteLog(Logger.emLogLevel.Info, $"[Info] SingleShotFocusStack(), rv = {rvSdoaq}");
            }


            if (rvSdoaq != SDOAQ_API.eErrorCode.ecNoError)
            {
                return false;
            }

            var imgInfoList = new List<SdoaqImageInfo>();
            for (int i = 0; i < imageBuffer.Length; i++)
            {
                string name = $"F-{focusList[i]}";

                imgInfoList.Add(new SdoaqImageInfo(name, acqParam.cameraRoiWidth, acqParam.cameraRoiHeight, CamInfo.ColorByte, imageBuffer[i]));
            }

            CallBackMsgLoop.Invoke((emCallBackMessage.FocusStack, new object[] { imgInfoList }));
            return true;
        }

        public bool AcquisitionContinuous_FocusStack()
        {
            if (IsInitialize == false || IsRunPlayer)
            {
                WriteLog(Logger.emLogLevel.Warning, $"AcquisitionContinuous_FocusStack(), Run False. IsInitialize = {IsInitialize}, PlayerMode = {PlayerMode}");
                return false;
            }

            if (PlayerMethod == emPlayerMethod.Thread)
            {
                PlayerMode = emPlayerMode.FocusStack;
                IsRunPlayer = true;
                _evtContinuosAcq_FocusStack.Set(true);
                return true;
            }

            var acqParamList = new SDOAQ_API.AcquisitionFixedParameters[]
            {
                CamInfo.AcqParam
            };

            var focusList = FocusList.GetStepList();

            var acqParam = acqParamList[0];

            var ringBufferSize = PlyerRingBufferSize;
            var imageBufferSize = (uint)CamInfo.ImgSize;
            var resultImageSizes = new List<ulong>();

            var imgCount = ringBufferSize * focusList.Count();
            for (int i = 0; i < imgCount; i++)
            {
                resultImageSizes.Add(imageBufferSize);
            }

            _ringBuffer.Set_Buffer(resultImageSizes.ToArray());
            _playerFoucsStepCount = focusList.Length;

            SelectMultWS(CamIndex);
            var rv = SDOAQ_API.SDOAQ_PlayFocusStack(acqParamList, CallBack_SDOAQ_PlayFocusStack,
                focusList, focusList.Length,
                ringBufferSize,
                _ringBuffer.Buffer,
                _ringBuffer.Sizes);

            if (rv == SDOAQ_API.eErrorCode.ecNoError)
            {
                PlayerMode = emPlayerMode.FocusStack;
                IsRunPlayer = true;
                return true;
            }
            WriteLog(Logger.emLogLevel.API, $"SDOAQ_PlayFocusStack(), Error = {rv}");
            return false;
        }

        public void AcquisitionStop_FocusStack()
        {
            if (IsRunPlayer && PlayerMode == emPlayerMode.FocusStack)
            {
                if (PlayerMethod == emPlayerMethod.CallBackFunc)
                {
                    SDOAQ_API.SDOAQ_StopFocusStack();
                }
                else
                {
                    _evtContinuosAcq_FocusStack.Reset();
                }               
                IsRunPlayer = false;
                PlayerMode = emPlayerMode.None;
            }
        }
        #endregion

        #region AF
        public async Task<bool> Acquisition_AfAsync()
        {
            if (IsInitialize == false || IsRunPlayer)
            {
                WriteLog(Logger.emLogLevel.Warning, $"Acquisition_AfAsync(), Run False. IsInitialize = {IsInitialize}, PlayerMode = {PlayerMode}");
                return false;
            }

            var acqParamList = new SDOAQ_API.AcquisitionFixedParameters[]
            {
                CamInfo.AcqParam
            };

            var focusList = FocusList.GetStepList();

            var rv = await Task.Run(() => Acq_Af(acqParamList, focusList, true));

            return rv;
        }
        
        private bool Acq_Af(SDOAQ_API.AcquisitionFixedParameters[] acqParamList, int[] focusList, bool bShowLog)
        {
            var acqParam = acqParamList[0];

            var imageBufferSize = (uint)CamInfo.ImgSize;
            var imageBuffer = new byte[imageBufferSize];
            var bestFocusStep = new double[1] { 0 };
            var score = new double[1] { 0 };

            long tickStart = Environment.TickCount;

            SelectMultWS(CamIndex);
            var rvSdoaq = SDOAQ_API.SDOAQ_SingleShotAF(
                acqParamList,
                focusList, focusList.Length,
                imageBuffer, imageBufferSize,
                bestFocusStep, score);

            long elapsedTime = Environment.TickCount - tickStart;

            if (bShowLog)
            {
                WriteLog(Logger.emLogLevel.Info, $"[Info] SingleShotAF(), rv = {rvSdoaq}, Acquisition Time = {elapsedTime} ms ({((double)focusList.Length / elapsedTime * 1000):F3} fps)");
            }
            else if (rvSdoaq != SDOAQ_API.eErrorCode.ecNoError)
            {
                WriteLog(Logger.emLogLevel.Info, $"[Info] SingleShotAF(), rv = {rvSdoaq}");
            }
                

            if (rvSdoaq != SDOAQ_API.eErrorCode.ecNoError)
            {
                return false;
            }

            var imgInfoList = new List<SdoaqImageInfo>
                {
                    new SdoaqImageInfo($"AF", acqParam.cameraRoiWidth, acqParam.cameraRoiHeight, CamInfo.ColorByte, imageBuffer)
                };

            CallBackMsgLoop.Invoke((emCallBackMessage.Af, new object[] { imgInfoList }));
            return true;
        }

        public bool AcquisitionContinuous_Af()
        {
            if (IsInitialize == false || IsRunPlayer)
            {
                WriteLog(Logger.emLogLevel.Warning, $"AcquisitionContinuous_Af(), Run False. IsInitialize = {IsInitialize}, PlayerMode = {PlayerMode}");
                return false;
            }

            if (PlayerMethod == emPlayerMethod.Thread)
            {
                PlayerMode = emPlayerMode.Af;
                IsRunPlayer = true;
                _evtContinuosAcq_Af.Set(true);
                return true;
            }

            var acqParamList = new SDOAQ_API.AcquisitionFixedParameters[]
            {
                CamInfo.AcqParam
            };

            var focusList = FocusList.GetStepList();

            var acqParam = acqParamList[0];

            var ringBufferSize = PlyerRingBufferSize;
            var imageBufferSize = (uint)CamInfo.ImgSize;
            var resultImageSizes = new List<ulong>();

            var imgCount = ringBufferSize * focusList.Count();
            for (int i = 0; i < imgCount; i++)
            {
                resultImageSizes.Add(imageBufferSize);
            }
            
            _ringBuffer.Set_Buffer(resultImageSizes.ToArray());
            _playerFoucsStepCount = focusList.Length;

            SelectMultWS(CamIndex);
            var rv = SDOAQ_API.SDOAQ_PlayAF(acqParamList, CallBack_SDOAQ_PlayAf, 
                focusList, focusList.Length,
                ringBufferSize, 
                _ringBuffer.Buffer, 
                _ringBuffer.Sizes);

            if (rv == SDOAQ_API.eErrorCode.ecNoError)
            {
                PlayerMode = emPlayerMode.Af;
                IsRunPlayer = true;
                return true;
            }
            WriteLog(Logger.emLogLevel.API, $"SDOAQ_PlayAF(), Error = {rv}");
            return false;
        }

        public void AcquisitionStop_Af()
        {
            if (IsRunPlayer && PlayerMode == emPlayerMode.Af)
            {
                if (PlayerMethod == emPlayerMethod.CallBackFunc)
                {
                    SDOAQ_API.SDOAQ_StopAF();
                }
                else
                {
                    _evtContinuosAcq_Af.Reset();
                }
                IsRunPlayer = false;
                PlayerMode = emPlayerMode.None;
            }
        }
        #endregion

        #region EDof
        public async Task<bool> Acquisition_EdofAsync(EdofImageList edofImageList)
        {
            if (IsInitialize == false || IsRunPlayer)
            {
                WriteLog(Logger.emLogLevel.Warning, $"Acquisition_EdofAsync(), Run False. IsInitialize = {IsInitialize}, PlayerMode = {PlayerMode}");
                return false;
            }

            var acqParamList = new SDOAQ_API.AcquisitionFixedParameters[]
            {
                CamInfo.AcqParam
            };

            var focusList = FocusList.GetStepList();

            var rv = await Task.Run(() => Acq_Edof(acqParamList, focusList, edofImageList, true));

            return rv;
        }

        private bool Acq_Edof(SDOAQ_API.AcquisitionFixedParameters[] acqParamList, int[] focusList, EdofImageList edofImageList, bool bShowLog)
        {
            var acqParam = acqParamList[0];

            var pixelSize = CamInfo.PixelSize;
            var imgSize = CamInfo.ImgSize;
            var dataSize = CamInfo.DataSize;

            var bufferEdofImage = new byte[imgSize];         // all-in-focus image
            var bufferStepMapImage = new float[pixelSize];   // 각 pixel 별 focus step number (0~319)                
            var bufferQualityMap = new float[pixelSize];     // 높이맵의 각 픽셀높이에 대한 점수(얼마나 신뢰할만한지)
            var bufferHeightMap = new float[pixelSize];      // 각 pixel 별 높이 정보
            var bufferPointCloud = new float[pixelSize * 3]; // 각 pixel의 (x,y,z) 좌표데이타

            var sizeEdofImageBuffer = (uint)(edofImageList.EnableEdofImg ? imgSize : 0);
            var sizeStepMapBuffer = (uint)(edofImageList.EnableStepMapImg ? dataSize : 0);
            var sizeQualityMapBuffer = (uint)(edofImageList.EnableQualityMap ? dataSize : 0);
            var sizeHeightMapBuffer = (uint)(edofImageList.EnableHeightMap ? dataSize : 0);
            var sizePointCloudBuffer = (uint)(edofImageList.EnablePointCloud ? dataSize * 3 : 0);

            long tickStart = Environment.TickCount;

            SelectMultWS(CamIndex);

            var rvSdoaq = SDOAQ_API.SDOAQ_SingleShotEdof(
                acqParamList,
                focusList, focusList.Length,
                bufferStepMapImage, sizeStepMapBuffer,
                bufferEdofImage, sizeEdofImageBuffer,
                bufferQualityMap, sizeQualityMapBuffer,
                bufferHeightMap, sizeHeightMapBuffer,
                bufferPointCloud, sizePointCloudBuffer);

            long elapsedTime = Environment.TickCount - tickStart;

            if (bShowLog)
            {
                WriteLog(Logger.emLogLevel.Info, $"[Info] SingleShotEdof(), rv = {rvSdoaq} Acquisition Time = {elapsedTime} ms ({((double)focusList.Length / elapsedTime * 1000):F3} fps)");
            }
            else if (rvSdoaq != SDOAQ_API.eErrorCode.ecNoError)
            {
                WriteLog(Logger.emLogLevel.Info, $"[Info] SingleShotEdof(), rv = {rvSdoaq}");
            }
                

            if (rvSdoaq != SDOAQ_API.eErrorCode.ecNoError)
            {
                return false;
            }

            var imgInfoList = new List<SdoaqImageInfo>();
            SdoaqPointCloudInfo pointCloudInfo = null;

            if (sizeEdofImageBuffer > 0)
            {
                imgInfoList.Add(new SdoaqImageInfo("Edof",
                    acqParam.cameraRoiWidth, acqParam.cameraRoiHeight, CamInfo.ColorByte,
                    bufferEdofImage));
            }

            if (sizeStepMapBuffer > 0)
            {
                imgInfoList.Add(new SdoaqImageInfo("StepMap",
                    acqParam.cameraRoiWidth, acqParam.cameraRoiHeight, 1,
                    ConvertFloatBufferToByteBuffer(pixelSize, bufferStepMapImage)));
            }

            if (sizeQualityMapBuffer > 0)
            {
                imgInfoList.Add(new SdoaqImageInfo("QualityMap",
                    acqParam.cameraRoiWidth, acqParam.cameraRoiHeight, 1,
                    ConvertFloatBufferToByteBuffer(pixelSize, bufferQualityMap)));
            }

            if (sizeHeightMapBuffer > 0)
            {
                imgInfoList.Add(new SdoaqImageInfo("HeightMap",
                    acqParam.cameraRoiWidth, acqParam.cameraRoiHeight, 1,
                    ConvertFloatBufferToByteBuffer(pixelSize, bufferHeightMap)));
            }

            if (sizePointCloudBuffer > 0 && sizeEdofImageBuffer > 0)
            {
                pointCloudInfo = new SdoaqPointCloudInfo("PointCloud",
                    acqParam.cameraRoiWidth, acqParam.cameraRoiHeight, focusList.Length,
                    bufferPointCloud, sizePointCloudBuffer,
                    bufferEdofImage, sizeEdofImageBuffer);
            }

            CallBackMsgLoop.Invoke((emCallBackMessage.Edof, new object[] { imgInfoList, pointCloudInfo }));
            return true;
        }

        public bool AcquisitionContinuous_Edof(EdofImageList edofImageList)
        {
            if (IsInitialize == false || IsRunPlayer)
            {
                WriteLog(Logger.emLogLevel.Warning, $"AcquisitionContinuous_Edof(), Run False. IsInitialize = {IsInitialize}, PlayerMode = {PlayerMode}");
                return false;
            }

            if (PlayerMethod == emPlayerMethod.Thread)
            {
                _edofImageList = edofImageList;
                PlayerMode = emPlayerMode.Edof;
                IsRunPlayer = true;
                _evtContinuosAcq_Edof.Set(true);
                return true;
            }

            var acqParamList = new SDOAQ_API.AcquisitionFixedParameters[]
            {
                CamInfo.AcqParam
            };

            var focusList = FocusList.GetStepList();

            var acqParam = acqParamList[0];

            var pixelSize = CamInfo.PixelSize;
            var imgSize = CamInfo.ImgSize;
            var dataSize = CamInfo.DataSize;

            var ringBufferSize = PlyerRingBufferSize;

            int sizeEdofImageBuffer = edofImageList.EnableEdofImg ? imgSize : 0;
            int sizeStepMapBuffer = edofImageList.EnableStepMapImg ? dataSize : 0;            
            int sizeQualityMapBuffer = edofImageList.EnableQualityMap ? dataSize : 0;
            int sizeHeightMapBuffer = edofImageList.EnableHeightMap ? dataSize : 0;
            int sizePointCloudBuffer = edofImageList.EnablePointCloud ? dataSize * 3 : 0;

            var resultImageSizes = new List<ulong>(); 
            
            for( int i = 0; i < ringBufferSize; i++)
            {
                resultImageSizes.AddRange(new ulong[]
                {
                    (uint)sizeEdofImageBuffer,    // EDOF: all-in-focus image
                    (uint)sizeStepMapBuffer,      // StepMap: 각 pixel 별 focus step number (0~319)
                    (uint)sizeQualityMapBuffer,   // QualityMap: 높이맵의 각 픽셀높이에 대한 점수(얼마나 신뢰할만한지)
                    (uint)sizeHeightMapBuffer,    // HeightMap: 각 pixel 별 높이 정보
                    (uint)sizePointCloudBuffer,   // PointCloud: 각 pixel의 (x,y,z) 좌표데이타
                });
            }

            _ringBuffer.Set_Buffer(resultImageSizes.ToArray());
            _playerFoucsStepCount = focusList.Length;

            SelectMultWS(CamIndex);

            var rv = SDOAQ_API.SDOAQ_PlayEdof(acqParamList, CallBack_SDOAQ_PlayEdof,
                focusList, focusList.Length,
                ringBufferSize,
                _ringBuffer.Buffer,
                _ringBuffer.Sizes);

            if (rv == SDOAQ_API.eErrorCode.ecNoError)
            {
                PlayerMode = emPlayerMode.Edof;
                IsRunPlayer = true;               
                return true;
            }
            WriteLog(Logger.emLogLevel.API, $"SDOAQ_PlayEdof(), Error = {rv}");
            return rv == SDOAQ_API.eErrorCode.ecNoError;
        }
        
        public void AcquisitionStop_Edof()
        {
            if (IsRunPlayer && PlayerMode == emPlayerMode.Edof)
            {
                if (PlayerMethod == emPlayerMethod.CallBackFunc)
                {
                    SDOAQ_API.SDOAQ_StopEdof();
                }
                else
                {
                    _evtContinuosAcq_Edof.Reset();
                }
                IsRunPlayer = false;
                PlayerMode = emPlayerMode.None;
            }
        }
        #endregion

    }
}
