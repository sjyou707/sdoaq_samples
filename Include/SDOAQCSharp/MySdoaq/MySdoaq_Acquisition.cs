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

            switch (_playerMode)
            {
                case PlayerMode.Edof:
                    AcquisitionStop_Edof();
                    break;
                case PlayerMode.Af:
                    AcquisitionStop_Af();
                    break;
                case PlayerMode.FocusStack:
                    AcquisitionStop_FocusStack();
                    break;
            }
            return true;
        }

        public bool Acquisition_Sanp(string directoryPath)
        {
            if (IsRunPlayer == false)
            {
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

            var rv = SDOAQ_API.SDOAQ_PlaySnap(CallBack_SDOAQ_Snap, focusList, focusList.Length, snapParams);

            return rv != SDOAQ_API.eErrorCode.ecNoError;
        }

        #region Focus Stack
        public async Task<bool> Acquisition_FocusStackAsync()
        {
            if (IsRunPlayer)
            {
                return false;
            }

            var acqParamList = new SDOAQ_API.AcquisitionFixedParameters[]
            {
                CamInfo.AcqParam
            };

            var focusList = FocusList.GetStepList();

            var rv = await Task.Run(() =>
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

                var rvSdoaq = SDOAQ_API.SDOAQ_SingleShotFocusStack(
                    acqParamList,
                    focusList, focusList.Length,
                    focusImagePointerList, focusImageBufferSizeList);

                long elapsedTime = Environment.TickCount - tickStart;

                WriteLog(Logger.emLogLevel.Info, $"[Info] StackImage(), rv = {rvSdoaq}, Acquisition Time = {elapsedTime} ms ({((double)focusList.Length / elapsedTime * 1000):F3} fps)");


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

                CallBackMsgLoop.Invoke((CallBackMessage.FocusStack, new object[] { imgInfoList }));
                return true;
            });
            
            return rv;
        }

        public bool AcquisitionContinuous_FocusStack()
        {
            if (IsRunPlayer)
            {
                return false;
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

            var rv = SDOAQ_API.SDOAQ_PlayFocusStack(acqParamList, CallBack_SDOAQ_PlayFocusStack,
                focusList, focusList.Length,
                ringBufferSize,
                _ringBuffer.Buffer,
                _ringBuffer.Sizes);

            if (rv == SDOAQ_API.eErrorCode.ecNoError)
            {
                _playerMode = PlayerMode.FocusStack;
                IsRunPlayer = true;
                return true;
            }
            WriteLog(Logger.emLogLevel.API, $"SDOAQ_PlayFocusStack(), Error = {rv}");
            return false;
        }

        public void AcquisitionStop_FocusStack()
        {
            if (IsRunPlayer && _playerMode == PlayerMode.FocusStack)
            {
                SDOAQ_API.SDOAQ_StopFocusStack();
                IsRunPlayer = false;
                _playerMode = PlayerMode.None;
            }
        }
        #endregion

        #region AF
        public async Task<bool> Acquisition_AfAsync()
        {
            if (IsRunPlayer)
            {
                return false;
            }

            var acqParamList = new SDOAQ_API.AcquisitionFixedParameters[]
            {
                CamInfo.AcqParam
            };

            var focusList = FocusList.GetStepList();

            var rv = await Task.Run(() =>
            {
                var acqParam = acqParamList[0];

                var imageBufferSize = (uint)CamInfo.ImgSize;
                var imageBuffer = new byte[imageBufferSize];
                var bestFocusStep = new double[1] { 0 };
                var score = new double[1] { 0 };

                long tickStart = Environment.TickCount;

                var rvSdoaq = SDOAQ_API.SDOAQ_SingleShotAF(
                    acqParamList,
                    focusList, focusList.Length,
                    imageBuffer, imageBufferSize,
                    bestFocusStep, score);

                long elapsedTime = Environment.TickCount - tickStart;

                WriteLog(Logger.emLogLevel.Info, $"[Info] StackImage(), rv = {rvSdoaq}, Acquisition Time = {elapsedTime} ms ({((double)focusList.Length / elapsedTime * 1000):F3} fps)");

                if (rvSdoaq != SDOAQ_API.eErrorCode.ecNoError)
                {
                    return false;
                }

                var imgInfoList = new List<SdoaqImageInfo>
                {
                    new SdoaqImageInfo($"AF", acqParam.cameraRoiWidth, acqParam.cameraRoiHeight, CamInfo.ColorByte, imageBuffer)
                };

                CallBackMsgLoop.Invoke((CallBackMessage.Af, new object[] { imgInfoList }));
                return true;
            });

            return rv;
        }
        
        public bool AcquisitionContinuous_Af()
        {
            if (IsRunPlayer)
            {
                return false;
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

            var rv = SDOAQ_API.SDOAQ_PlayAF(acqParamList, CallBack_SDOAQ_PlayAf, 
                focusList, focusList.Length,
                ringBufferSize, 
                _ringBuffer.Buffer, 
                _ringBuffer.Sizes);

            if (rv == SDOAQ_API.eErrorCode.ecNoError)
            {
                _playerMode = PlayerMode.Af;
                IsRunPlayer = true;
                return true;
            }
            WriteLog(Logger.emLogLevel.API, $"SDOAQ_PlayAF(), Error = {rv}");
            return false;
        }

        public void AcquisitionStop_Af()
        {
            if (IsRunPlayer && _playerMode == PlayerMode.Af)
            {
                SDOAQ_API.SDOAQ_StopAF();
                IsRunPlayer = false;
                _playerMode = PlayerMode.None;
            }
        }
        #endregion

        #region EDof
        public async Task<bool> Acquisition_EdofAsync(bool enableEdofImg = true,
            bool enableStepMapImg = true, bool enableQualityMap = true, 
            bool enableHeightMap = true, bool enablePointCloud = true)
        {
            if (IsRunPlayer)
            {
                return false;
            }

            var acqParamList = new SDOAQ_API.AcquisitionFixedParameters[]
            {
                CamInfo.AcqParam
            };

            var focusList = FocusList.GetStepList();

            var rv = await Task.Run(() =>
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

                var sizeEdofImageBuffer = (uint)(enableEdofImg ? imgSize : 0);
                var sizeStepMapBuffer =  (uint)(enableStepMapImg ? dataSize : 0);                
                var sizeQualityMapBuffer = (uint)(enableQualityMap ? dataSize : 0);
                var sizeHeightMapBuffer = (uint)(enableHeightMap ? dataSize : 0);
                var sizePointCloudBuffer = (uint)(enablePointCloud ? dataSize * 3 : 0);

                long tickStart = Environment.TickCount;

                var rvSdoaq = SDOAQ_API.SDOAQ_SingleShotEdof(
                    acqParamList,
                    focusList, focusList.Length,
                    bufferStepMapImage, sizeStepMapBuffer,
                    bufferEdofImage, sizeEdofImageBuffer,
                    bufferQualityMap, sizeQualityMapBuffer,
                    bufferHeightMap, sizeHeightMapBuffer,
                    bufferPointCloud, sizePointCloudBuffer);

                long elapsedTime = Environment.TickCount - tickStart;
                WriteLog(Logger.emLogLevel.Info, $"[Info] EdofImage(), rv = {rvSdoaq} Acquisition Time = {elapsedTime} ms ({((double)focusList.Length / elapsedTime * 1000):F3} fps)");

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

                CallBackMsgLoop.Invoke((CallBackMessage.Edof, new object[] { imgInfoList, pointCloudInfo }));
                return true;
            });

            return rv;
        }

        public bool AcquisitionContinuous_Edof(bool enableStepMapImg = true,
            bool enableEdofImg = true, bool enableQualityMap = true,
            bool enableHeightMap = true, bool enablePointCloud = true)
        {
            if (IsRunPlayer)
            {
                return false;
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

            int sizeEdofImageBuffer = enableEdofImg ? imgSize : 0;
            int sizeStepMapBuffer = enableStepMapImg ? dataSize : 0;            
            int sizeQualityMapBuffer = enableQualityMap ? dataSize : 0;
            int sizeHeightMapBuffer = enableHeightMap ? dataSize : 0;
            int sizePointCloudBuffer = enablePointCloud ? dataSize * 3 : 0;

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

            var rv = SDOAQ_API.SDOAQ_PlayEdof(acqParamList, CallBack_SDOAQ_PlayEdof,
                focusList, focusList.Length,
                ringBufferSize,
                _ringBuffer.Buffer,
                _ringBuffer.Sizes);

            if (rv == SDOAQ_API.eErrorCode.ecNoError)
            {
                _playerMode = PlayerMode.Edof;
                IsRunPlayer = true;               
                return true;
            }
            WriteLog(Logger.emLogLevel.API, $"SDOAQ_PlayEdof(), Error = {rv}");
            return rv == SDOAQ_API.eErrorCode.ecNoError;
        }

        public void AcquisitionStop_Edof()
        {
            if (IsRunPlayer && _playerMode == PlayerMode.Edof)
            {
                SDOAQ_API.SDOAQ_StopEdof();
                IsRunPlayer = false;
                _playerMode = PlayerMode.None;
            }
        }
        #endregion

    }
}
