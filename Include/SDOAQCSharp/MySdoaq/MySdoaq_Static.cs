using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using SDOAQ;
using SDOAQCSharp.Tool;

namespace SDOAQ_App_CS
{
    partial class MySdoaq
    {
        private const string SCRIPT_FILE_NAME = "wisescope.script.txt";
        private const string SCRIPT_LINE_NUM_OF_WS = "Number of WSM";

        public static event EventHandler<SdoaqEventArgs> Initialized;
        public static event EventHandler<LoggerEventArgs> LogReceived
        {
            add
            {
                s_logger.DataReceived += value;
            }

            remove
            {
                s_logger.DataReceived -= value;
            }
        }
        
        public static bool IsInitialize { get; private set; } = false;

        private static SDOAQ.SDOAQ_API.SDOAQ_LogCallback CallBack_SDOAQ_Log;
        private static SDOAQ.SDOAQ_API.SDOAQ_ErrorCallback CallBack_SDOAQ_Error;
        private static SDOAQ.SDOAQ_API.SDOAQ_InitDoneCallback CallBack_InitDone;
        private static SDOAQ.SDOAQ_API.SDOAQ_PlayCallback CallBack_SDOAQ_PlayFocusStack;
        private static SDOAQ.SDOAQ_API.SDOAQ_PlayCallback CallBack_SDOAQ_PlayEdof;
        private static SDOAQ.SDOAQ_API.SDOAQ_PlayAfCallback CallBack_SDOAQ_PlayAf;
        private static SDOAQ.SDOAQ_API.SDOAQ_SnapCallback CallBack_SDOAQ_Snap;

        private static bool s_isFirstInitialize = true;
        private static Dictionary<int, MySdoaq> s_sdoaqObjList = new Dictionary<int, MySdoaq>();
        private static Logger s_logger = new Logger();
        
        public static Dictionary<int, MySdoaq> LoadScript(string scriptFilePath = "")
        {
            string path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), SCRIPT_FILE_NAME);

            int numOfWiseScope = 1;

            var rv = SDOWSIO.WSIO.UTIL.WSUT_IntFromLineScript(path, SCRIPT_LINE_NUM_OF_WS, out int multiWiseScopeCount);
            if (rv == SDOWSIO.WSIO.WSIORV.WSIORV_SUCCESS)
            {
                numOfWiseScope = Math.Max(1, multiWiseScopeCount);
            }

            Finalize();

            s_sdoaqObjList.Clear();
            
            for (int i = 0; i< numOfWiseScope; i++)
            {
                s_sdoaqObjList.Add(i, new MySdoaq());
            }

            return s_sdoaqObjList;
        }

        public static bool Initialize()
        {
            Finalize();
            
            var rv = SDOAQ_API.SDOAQ_Initialize(CallBack_SDOAQ_Log, CallBack_SDOAQ_Error, CallBack_InitDone);

            return rv == SDOAQ_API.eErrorCode.ecNoError;
        }

        public static bool Finalize()
        {
            foreach (var sdoaqObj in s_sdoaqObjList.Values)
            {
                sdoaqObj.AcquisitionStop();
            }

            IsInitialize = false;

            return SDOAQ_API.SDOAQ_Finalize() == SDOAQ_API.eErrorCode.ecNoError;
        }

        public static string GetVersion()
        {
            return $"{SDOAQ_API.SDOAQ_GetMajorVersion()}.{SDOAQ_API.SDOAQ_GetMinorVersion()}.{SDOAQ_API.SDOAQ_GetPatchVersion()}";
        }

        public static string GetVersion_SdEdofAlgorithm()
        {
            int algoVersion = SDOAQ_API.SDOAQ_GetAlgorithmVersion();
            return $"{algoVersion / 1000}.{algoVersion % 1000}";
        }

        private static void Add_CallbackFunction()
        {
            CallBack_SDOAQ_Log = OnSdoaq_Log;
            CallBack_SDOAQ_Error = OnSdoaq_Error;
            CallBack_InitDone = OnSdoaq_InitDone;
            CallBack_SDOAQ_PlayFocusStack = OnSdoaq_PlayFocusStack;
            CallBack_SDOAQ_PlayEdof = OnSdoaq_PlayEdof;
            CallBack_SDOAQ_PlayAf = OnSdoaq_PlayAf;
            CallBack_SDOAQ_Snap = OnSdoaq_Snap;
        }

        private static void WriteLog(Logger.emLogLevel logLevel, string format, params object[] args)
        {
            if (string.IsNullOrEmpty(format))
            {
                return;
            }

            if (args == null || args.Length == 0)
            {
                s_logger.WriteLog($"[{logLevel}] {format}");
            }
            else
            {
                s_logger.WriteLog($"[{logLevel}] {string.Format(format, args)}");
            }
        }

        private static int GetCallBackMultWs()
        {
            return Math.Max(0, SDOAQ_API.SDOAQ_GetCallbackMultiWs() - 1);
        }

        private static MySdoaq GetSdoaqObj()
        {
            int idx = GetCallBackMultWs();

            if (s_sdoaqObjList.ContainsKey(idx) == false)
            {
                return null;
            }

            return s_sdoaqObjList[idx];
        }

        private static void OnSdoaq_Log(SDOAQ_API.eLogSeverity severity, StringBuilder pMessage)
        {
            WriteLog(Logger.emLogLevel.API, $"[{severity}]{pMessage.ToString()}");
        }

        private static void OnSdoaq_Error(SDOAQ_API.eErrorCode errorCode, StringBuilder pErrorMessage)
        {
            WriteLog(Logger.emLogLevel.API, $"[Error]Error Code = {errorCode}, {pErrorMessage.ToString()}");
        }

        private static void OnSdoaq_InitDone(SDOAQ_API.eErrorCode errorCode, StringBuilder pErrorMessage)
        {
            bool bInitDone = errorCode == SDOAQ_API.eErrorCode.ecNoError;

            if (bInitDone)
            {
                WriteLog(Logger.emLogLevel.API, $"[INIT]Initialize Done");

                foreach (var sdoaqObj in s_sdoaqObjList.Values)
                {
                    bool isWriteable = false;
                    string paramValue = string.Empty;

                    bool bCamColor = false;
                    int fullFrameSizeX = 0;
                    int fullFrameSizeY = 0;
                    int camBinning = 0;
                    if (sdoaqObj.GetParam(SDOAQ_API.eParameterId.piCameraColor, out isWriteable, out paramValue))
                    {
                        bCamColor = int.Parse(paramValue) == 0; 
                    }

                    if (sdoaqObj.GetParam(SDOAQ_API.eParameterId.piCameraFullFrameSizeX, out isWriteable, out paramValue))
                    {
                        fullFrameSizeX = int.Parse(paramValue);
                    }

                    if (sdoaqObj.GetParam(SDOAQ_API.eParameterId.piCameraFullFrameSizeY, out isWriteable, out paramValue))
                    {
                        fullFrameSizeY = int.Parse(paramValue);
                    }

                    if (sdoaqObj.GetParam(SDOAQ_API.eParameterId.piCameraBinning, out isWriteable, out paramValue))
                    {
                        camBinning = int.Parse(paramValue);
                    }

                    var camInfo = new SdoaqCamInfo()
                    {
                        ColorByte = bCamColor ? 3 : 1,
                        AcqParam = new SDOAQ_API.AcquisitionFixedParameters()
                        {
                            cameraRoiLeft = 0,
                            cameraRoiTop = 0,
                            cameraRoiWidth = fullFrameSizeX,
                            cameraRoiHeight = fullFrameSizeY,
                            cameraBinning = camBinning,
                        }
                    };

                    sdoaqObj.CamInfo = camInfo;

                    sdoaqObj.SetRoi_AF(DFLT_AF_ROI);
                    sdoaqObj.SetFocus(DFLT_FOCUS_LIST);
                    sdoaqObj.SetSnapFocus(DFLT_FOCUS_LIST);
                }
            }
            else
            {
                WriteLog(Logger.emLogLevel.API, $"[INIT]Initialize Error, Error Code = {errorCode}, {pErrorMessage.ToString()}");
            }

            Initialized?.Invoke(null, new SdoaqEventArgs(errorCode, pErrorMessage.ToString()));
        }

        private static void OnSdoaq_PlayFocusStack(SDOAQ_API.eErrorCode errorCode, int lastFilledRingBufferEntry)
        {
            var sdoaqObj = GetSdoaqObj();

            if (sdoaqObj == null || sdoaqObj.IsRunPlayer == false)
            {
                return;
            }

            if (errorCode != SDOAQ_API.eErrorCode.ecNoError)
            {
                WriteLog(Logger.emLogLevel.API, $"OnSdoaq_PlayFocusStack(), Error = {errorCode}");
                return;
            }

            var foucsList = sdoaqObj.FocusList.GetStepList();

            int idxRingBuffer = lastFilledRingBufferEntry * foucsList.Length;
            var camInfo = sdoaqObj.CamInfo;
            var acqParam = camInfo.AcqParam;

            byte[][] resultImgList = new byte[sdoaqObj._playerFoucsStepCount][];
            unsafe
            {
                for (int i = 0; i < resultImgList.Length; i++)
                {
                    int idx = idxRingBuffer + i;
                    int size = (int)sdoaqObj._ringBuffer.Sizes[idx];
                    byte[] buffer = new byte[size];

                    if (size > 0)
                    {
                        var ptrSrc = (float*)sdoaqObj._ringBuffer.Buffer[idx];

                        fixed (byte* ptrDst = buffer)
                        {
                            Buffer.MemoryCopy(ptrSrc, ptrDst, size, size);
                        }
                    }
                    resultImgList[i] = buffer;
                }
            }

            var imgInfoList = new List<SdoaqImageInfo>();

            for (int i = 0; i < resultImgList.Length; i++)
            {
                imgInfoList.Add(new SdoaqImageInfo($"F-{foucsList[i]}", camInfo.PixelWidth, camInfo.PixelHeight, camInfo.ColorByte, resultImgList[i]));
            }

            sdoaqObj.CallBackMsgLoop.Invoke((CallBackMessage.FocusStack, new object[] { imgInfoList }));
        }

        private static void OnSdoaq_PlayEdof(SDOAQ.SDOAQ_API.eErrorCode errorCode, int lastFilledRingBufferEntry)
        {
            var sdoaqObj = GetSdoaqObj();

            if (sdoaqObj == null || sdoaqObj.IsRunPlayer == false)
            {
                return;
            }

            if (errorCode != SDOAQ_API.eErrorCode.ecNoError)
            {
                WriteLog(Logger.emLogLevel.API, $"OnSdoaq_PlayEdof(), Error = {errorCode}");
                return;
            }

            int idxRingBuffer = lastFilledRingBufferEntry * EDOF_RESULT_IMG_COUNT;
            var camInfo = sdoaqObj.CamInfo;
            var acqParam = camInfo.AcqParam;
            int pixelSize = camInfo.PixelSize;

            byte[] resultImgEdof = new byte[sdoaqObj._ringBuffer.Sizes[idxRingBuffer]];

            Marshal.Copy(sdoaqObj._ringBuffer.Buffer[idxRingBuffer], resultImgEdof, 0, resultImgEdof.Length);

            float[][] resultImgList = new float[EDOF_RESULT_IMG_COUNT - 1][];
            unsafe
            {
                for (int i = 0; i < resultImgList.Length; i++)
                {
                    int idx = idxRingBuffer + i + 1;
                    int size = (int)sdoaqObj._ringBuffer.Sizes[idx];
                    float[] buffer = new float[size];

                    if (size > 0)
                    {
                        var ptrSrc = (float*)sdoaqObj._ringBuffer.Buffer[idx];

                        fixed (float* ptrDst = buffer)
                        {
                            Buffer.MemoryCopy(ptrSrc, ptrDst, size, size);
                        }
                    }
                    resultImgList[i] = buffer;
                }
            }

            var imgInfoList = new List<SdoaqImageInfo>();
            SdoaqPointCloudInfo pointCloudInfo = null;

            if (resultImgEdof.Length > 0)
            {

                imgInfoList.Add(new SdoaqImageInfo("Edof",
                    acqParam.cameraRoiWidth, acqParam.cameraRoiHeight, camInfo.ColorByte,
                    resultImgEdof));
            }

            if (resultImgList[0].Length > 0)
            {
                imgInfoList.Add(new SdoaqImageInfo("StepMap",
                    acqParam.cameraRoiWidth, acqParam.cameraRoiHeight, 1,
                    ConvertFloatBufferToByteBuffer(pixelSize, resultImgList[0])));
            }

            if (resultImgList[1].Length > 0)
            {
                imgInfoList.Add(new SdoaqImageInfo("QualityMap",
                    acqParam.cameraRoiWidth, acqParam.cameraRoiHeight, 1,
                    ConvertFloatBufferToByteBuffer(pixelSize, resultImgList[1])));
            }

            if (resultImgList[2].Length > 0)
            {
                imgInfoList.Add(new SdoaqImageInfo("HeightMap",
                    acqParam.cameraRoiWidth, acqParam.cameraRoiHeight, 1,
                    ConvertFloatBufferToByteBuffer(pixelSize, resultImgList[2])));
            }
            
            if (resultImgList[3].Length > 0 && resultImgEdof.Length > 0)
            {
                pointCloudInfo = new SdoaqPointCloudInfo("PointCloud",
                    acqParam.cameraRoiWidth, acqParam.cameraRoiHeight, sdoaqObj._playerFoucsStepCount,
                    resultImgList[3], (uint)resultImgList[3].Length,
                    resultImgEdof, (uint)resultImgEdof.Length);
            }

            sdoaqObj.CallBackMsgLoop.Invoke((CallBackMessage.Edof, new object[] { imgInfoList, pointCloudInfo }));

        }

        private static void OnSdoaq_PlayAf(SDOAQ.SDOAQ_API.eErrorCode errorCode, int lastFilledRingBufferEntry, double dbBestFocusStep, double dbBestScore)
        {
            var sdoaqObj = GetSdoaqObj();

            if (sdoaqObj == null || sdoaqObj.IsRunPlayer == false)
            {
                return;
            }

            if (errorCode != SDOAQ_API.eErrorCode.ecNoError)
            {
                WriteLog(Logger.emLogLevel.API, $"OnSdoaq_PlayFocusStack(), Error = {errorCode}");
                return;
            }

            int idxRingBuffer = lastFilledRingBufferEntry;
            var camInfo = sdoaqObj.CamInfo;
            var acqParam = camInfo.AcqParam;

            byte[][] resultImgList = new byte[1][];
            unsafe
            {
                for (int i = 0; i < resultImgList.Length; i++)
                {
                    int idx = idxRingBuffer + i;
                    int size = (int)sdoaqObj._ringBuffer.Sizes[idx];
                    byte[] buffer = new byte[size];

                    if (size > 0)
                    {
                        var ptrSrc = (float*)sdoaqObj._ringBuffer.Buffer[idx];

                        fixed (byte* ptrDst = buffer)
                        {
                            Buffer.MemoryCopy(ptrSrc, ptrDst, size, size);
                        }
                    }
                    resultImgList[i] = buffer;
                }
            }

            var imgInfoList = new List<SdoaqImageInfo>();

            for (int i = 0; i < resultImgList.Length; i++)
            {
                imgInfoList.Add(new SdoaqImageInfo($"AF", camInfo.PixelWidth, camInfo.PixelHeight, camInfo.ColorByte, resultImgList[i]));
            }

            sdoaqObj.CallBackMsgLoop.Invoke((CallBackMessage.Af, new object[] { imgInfoList }));
        }

        private static void OnSdoaq_Snap(SDOAQ.SDOAQ_API.eErrorCode errorCode, int lastFilledRingBufferEntry)
        {
            var sdoaqObj = GetSdoaqObj();

            if (sdoaqObj == null)
            {
                return;
            }

            WriteLog(Logger.emLogLevel.API, $"[Cam{sdoaqObj.CamIndex}]Snap Received, ErrorCode = {errorCode}");
        }
    }
}
