using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using SDOAQ;
using SDOAQCSharp.Tool;

namespace SDOAQCSharp
{
    partial class MyCamera
    {
        public static bool IsInitialize { get; private set; } = false;
        public static event EventHandler<SdoaqEventArgs> Initialized;

        private static readonly Logger s_logger = new Logger();
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

        private static SDOAQ_API.SDOAQ_FrameCallback CallBack_Frame;
        private static SDOAQ_API.SDOAQ_LogCallback CallBack_SDOAQ_Log;
        private static SDOAQ_API.SDOAQ_ErrorCallback CallBack_SDOAQ_Error;
        private static SDOAQ_API.SDOAQ_InitDoneCallback CallBack_InitDone;

        private static readonly Dictionary<int, MyCamera> s_CamObjList = new Dictionary<int, MyCamera>();

        private static MyManualResetEvent<(SDOAQ_API.eErrorCode errorCode, StringBuilder pErrorMessage)> s_evtInitDone = new MyManualResetEvent<(SDOAQ_API.eErrorCode errorCode, StringBuilder pErrorMessage)>(false);
        
        private static bool s_isFirstInitialize = true;

        private const string SCRIPT_FILE_NAME = "wisescope.script.txt";
        private const string SCRIPT_LINE_NUM_OF_WS = "Number of WSM";

        public static Dictionary<int, MyCamera> LoadScript(string scriptFilePath = "")
        {
            string path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), SCRIPT_FILE_NAME);

            //string currentPath = System.IO.Directory.GetCurrentDirectory();
            //string upperPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(currentPath));
            //string path = System.IO.Path.Combine(upperPath, SCRIPT_FILE_NAME);

            SdoaqScriptReader.GetIntFromLineScript(path, SCRIPT_LINE_NUM_OF_WS, 1, out int numOfWiseScope);

            numOfWiseScope = Math.Max(1, numOfWiseScope);
            
            foreach (var item in s_CamObjList)
            {
                item.Value?.Dispose();
            }

            s_CamObjList.Clear();

            for (int i = 0; i < numOfWiseScope; i++)
            {
                s_CamObjList.Add(i, new MyCamera(i));
            }

            return s_CamObjList;
        }

        public static string GetVersion()
        {
            return $"{SDOAQ_API.SDOAQ_GetMajorVersion()}.{SDOAQ_API.SDOAQ_GetMinorVersion()}.{SDOAQ_API.SDOAQ_GetPatchVersion()}";
        }

        public static bool SDOAQ_Initialize(bool bSync = false)
        {
            SDOAQ_Finalize();

            if (s_CamObjList.Count > 1)
            {
                SDOAQ_API.SDOAQ_RegisterMultiWsApi();
            }

            // specify the script file name including the full file path.
            string scriptFile = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wisescope.script.txt");
            SDOAQ_API.SDOAQ_SetSystemScriptFilename(scriptFile);

            // set the path to the cam files folder
            string camFilesPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "CamFiles");
            SDOAQ_API.SDOAQ_SetCamfilePath(camFilesPath);

            if (bSync)
            {
                s_evtInitDone.Reset();
            }

            var rv = SDOAQ_API.SDOAQ_Initialize(CallBack_SDOAQ_Log, CallBack_SDOAQ_Error, CallBack_InitDone);

            if (bSync)
            {
                const int MAX_WAIT_TIME = 30 * 1000;
                if (s_evtInitDone.WaitOne(MAX_WAIT_TIME, true))
                {
                    var (errorCode, pErrorMessage) = s_evtInitDone.ReturnValue;
                    return errorCode == SDOAQ_API.eErrorCode.ecNoError;
                }
                else
                {
                    WriteLog(Logger.emLogLevel.API, $"SDOAQ_Initialize(), Init Done Timeover...");
                    return false;
                }
            }
            return rv == SDOAQ_API.eErrorCode.ecNoError;
        }

        public static bool SDOAQ_Finalize()
        {
            IsInitialize = false;

            return SDOAQ_API.SDOAQ_Finalize() == SDOAQ_API.eErrorCode.ecNoError;
        }

        public static void DisposeStaticResouce()
        {
            if (s_CamObjList != null)
            {
                foreach (var key in s_CamObjList.Keys.ToList())
                {
                    s_CamObjList[key].Dispose();
                }
            }
            
            s_logger?.Dispose();
        }

        public static void EanbleCameraFrameCallBack(bool bEnable)
        {
            SDOAQ_API.SDOAQ_SetFrameCallback(bEnable ? CallBack_Frame : null);
        }

        public static bool SelectMultiWS(int idxCam)
        {
            if (s_CamObjList.Count <= 1)
            {
                return true;
            }

            int idxWS = idxCam + 1;
            var rv = SDOAQ_API.SDOAQ_SelectMultiWs(idxWS);

            if (rv != SDOAQ_API.eErrorCode.ecNoError)
            {
                WriteLog(Logger.emLogLevel.API, $"SelectMultiWS(), WS Index = {idxWS}, rv = {rv}");
                return false;
            }

            return true;
        }

        public static void WriteLog(Logger.emLogLevel logLevel, string format, params object[] args)
        {
            if (s_logger == null)
            {
                return;

            }

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

        private static void Add_CallbackFunction()
        {
            CallBack_SDOAQ_Log = OnSdoaq_Log;
            CallBack_SDOAQ_Error = OnSdoaq_Error;
            CallBack_InitDone = OnSdoaq_InitDone;
            CallBack_Frame = OnSdoaq_Frame;
        }

        private static int GetCallBackMultWs()
        {
            return s_CamObjList.Count > 1 
                ? Math.Max(0, SDOAQ_API.SDOAQ_GetCallbackMultiWs() - 1)
                : 0;
        }

        private static MyCamera GetCamObj()
        {
            int idx = GetCallBackMultWs();

            if (s_CamObjList.TryGetValue(idx, out var cam))
            {
                return cam;
            }

            return null;
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
            if (s_evtInitDone.IsWaitSet)
            {
                s_evtInitDone.Set((errorCode, pErrorMessage));
            }

            bool bInitDone = errorCode == SDOAQ_API.eErrorCode.ecNoError;

            IsInitialize = bInitDone;

            if (bInitDone)
            {
                SDOAQ_API.SDOAQ_RegisterLowLevelAuthorization();

                s_logger.WriteLog($"[INIT]Initialize Done");

                foreach (var camObj in s_CamObjList.Values)
                {
                    camObj.SetTriggerMode(SDOAQ_API.eCameraTriggerMode.ctmCameraSoftware);
                }

                SelectMultiWS(0);
            }
            else
            {
                s_logger.WriteLog($"[INIT]Initialize Error, Error Code = {errorCode}, {pErrorMessage.ToString()}");
            }
            
            Initialized?.Invoke(null, new SdoaqEventArgs(errorCode, pErrorMessage.ToString()));
        }
        
        private static void OnSdoaq_Frame(SDOAQ_API.eErrorCode errorCode, IntPtr pBuffer, int bufferSize, ref SDOAQ_API.FrameDescriptor frameDescriptor)
        {
            if (errorCode != SDOAQ_API.eErrorCode.ecNoError)
            {
                WriteLog(Logger.emLogLevel.Error, $"OnSdoaq_Frame(), Error = {errorCode}, Cam Index : {GetCallBackMultWs()}");
                return;
            }

            if (pBuffer == null || bufferSize == 0)
            {
                return;
            }

            byte[] data = new byte[bufferSize];

            Marshal.Copy(pBuffer, data, 0, bufferSize);

            var myCam = GetCamObj();

            if (myCam == null)
            {
                WriteLog(Logger.emLogLevel.Error, $"OnSdoaq_Frame(), CamObject Null... Cam Index : {GetCallBackMultWs()}");
                return;
            }

            myCam.RecivedFrameCount++;

            var imgInfo = new SdoaqImageInfo(frameDescriptor.pixelsWidth, frameDescriptor.pixelsHeight, 
                frameDescriptor.bytesLine, frameDescriptor.bytesPixel, 
                data);

            myCam.CallBackMsgLoop?.Invoke((CallBackMessage.Frame, new object[] { imgInfo }));
        }
    }
}
