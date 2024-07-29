using System;
using System.Runtime.InteropServices;
using System.Text;

using SDOAQ;
using SDOAQCSharp.Tool;

namespace SdoaqCameraFrameCallback
{
    public class MyCamera : IDisposable
    {
        public enum RequestMsg
        {
            Initialize,
        }

        public enum CallBackMessage
        {
            Initialize,
            Frame,
        }

        /// <summary>
        /// The parameter name varies from camera API to camera API.
        /// In the example, the name is based on the Pylon API
        /// </summary>
        private class ParameterName
        {
            public const string Width = "Width";
            public const string Height = "Height";
            public const string OffsetX = "OffsetX";
            public const string OffsetY = "OffsetY";
            public const string Gain = "Gain";
            public const string ReverseX = "ReverseX";
            public const string ReverseY = "ReverseY";
            public const string BalanceRatioSelector = "BalanceRatioSelector";
            public const string BalanceRatioSelector_Red = "Red";
            public const string BalanceRatioSelector_Green = "Green";
            public const string BalanceRatioSelector_Blue = "Blue";
            public const string BalanceRatioAbs = "BalanceRatioAbs";
            public const string BalanceRatio = "BalanceRatio";
        }

        public MyQueue<(CallBackMessage msg, object[] objs)>.MsgLoopCallBack CallBackMsgLoop
        {
            get
            {
                return _callBackMsgQueue.CallBackMsgLoop;
            }
            set
            {
                _callBackMsgQueue.CallBackMsgLoop = value;
            }
        }
        public Logger MyLogger { get; private set; } = new Logger();
        public bool IsInitialize { get; private set; } = false;
        public uint RecivedFrameCount { get; private set; } = 0;

        private MyQueue<(CallBackMessage msg, object[] objs)> _callBackMsgQueue = new MyQueue<(CallBackMessage msg, object[] objs)>();

        private MyManualResetEvent<(SDOAQ_API.eErrorCode errorCode, StringBuilder pErrorMessage)> _evtInitDone = new MyManualResetEvent<(SDOAQ_API.eErrorCode errorCode, StringBuilder pErrorMessage)>(false);

        private static SDOAQ_API.SDOAQ_FrameCallback SDOAQ_FrameCallback;
        private static SDOAQ_API.SDOAQ_LogCallback SDOAQ_LogCallback;
        private static SDOAQ_API.SDOAQ_ErrorCallback SDOAQ_ErrorCallback;
        private static SDOAQ_API.SDOAQ_InitDoneCallback SDOAQ_InitDoneCallback;

        private static MyCamera _myCamera = null;
        private static bool _flagFirst = true;

        public MyCamera()
        {
            if (_flagFirst)
            {
                _flagFirst = false;
                _myCamera = this;

                SDOAQ_LogCallback = LogCallback;
                SDOAQ_ErrorCallback = ErrorCallback;
                SDOAQ_InitDoneCallback = InitDoneCallback;

                SDOAQ_FrameCallback = FrameCallback;
            }
        }

        public string GetVersion()
        {
            return $"{SDOAQ_API.SDOAQ_GetMajorVersion()}.{SDOAQ_API.SDOAQ_GetMinorVersion()}.{SDOAQ_API.SDOAQ_GetPatchVersion()}";
        }

        public SDOAQ_API.eErrorCode Initialize(bool bSync = false)
        {
            IsInitialize = false;
            RecivedFrameCount = 0;
            if (bSync)
            {
                _evtInitDone.Reset();
            }

            var rv = SDOAQ_API.SDOAQ_Initialize(null, null, SDOAQ_InitDoneCallback);

            if (bSync)
            {
                _evtInitDone.WaitOne();
                rv = _evtInitDone.ReturnValue.errorCode;
            }
            return rv;
        }

        public SDOAQ_API.eErrorCode Finalize()
        {
            IsInitialize = false;
            return SDOAQ_API.SDOAQ_Finalize();
        }

        public void EanbleCameraFrameCallBack(bool bEnable)
        {
            SDOAQ_API.SDOAQ_SetFrameCallback(bEnable ? SDOAQ_FrameCallback : null);
        }

        #region Set/Get Parameter
        public SDOAQ_API.eErrorCode SetTriggerMode(SDOAQ_API.eCameraTriggerMode triggerMode)
        {
            if (IsInitialize == false)
            {
                return SDOAQ_API.eErrorCode.ecNotInitialized;
            }
            return SDOAQ_API.SDOAQ_SetCameraTriggerMode(triggerMode);
        }

        public SDOAQ_API.eErrorCode SetExeSoftwareTrigger()
        {
            if (IsInitialize == false)
            {
                return SDOAQ_API.eErrorCode.ecNotInitialized;
            }

            return SDOAQ_API.SDOAQ_ExecCameraSoftwareTrigger();
        }

        public SDOAQ_API.eErrorCode SetFOV(int width, int height)
        {
            if (IsInitialize == false)
            {
                return SDOAQ_API.eErrorCode.ecNotInitialized;
            }

            return SDOAQ_API.SDOAQ_SetCameraParameter(width, height, 1);
        }

        public SDOAQ_API.eErrorCode GetFOV(out int width, out int height)
        {
            width = 0;
            height = 0;
            if (IsInitialize == false)
            {
                return SDOAQ_API.eErrorCode.ecNotInitialized;
            }

            int[] cam_width = new int[1];
            int[] cam_height = new int[1];
            int[] cam_binning = new int[1];

            var rv = SDOAQ_API.SDOAQ_GetCameraParameter(cam_width, cam_height, cam_binning);

            width = cam_width[0];
            height = cam_height[0];
            return rv;
        }

        public SDOAQ_API.eErrorCode SetExposureTime(int exposureTime)
        {
            return SDOAQ_API.SDOAQ_SetIntParameterValue(SDOAQ_API.eParameterId.piCameraExposureTime, exposureTime);
        }

        public SDOAQ_API.eErrorCode GetExposureTime(out int exposureTime)
        {
            exposureTime = 0;
            int[] tmp = new int[1];
            var rv = SDOAQ_API.SDOAQ_GetIntParameterValue(SDOAQ_API.eParameterId.piCameraExposureTime, tmp);

            if (rv == SDOAQ_API.eErrorCode.ecNoError)
            {
                exposureTime = tmp[0];
            }
            return rv;
        }

        #region Features supported only by select cameras
        public SDOAQ_API.eErrorCode SetGrabState(SDOAQ_API.eCameraGrabbingStatus grabState)
        {
            if (IsInitialize == false)
            {
                return SDOAQ_API.eErrorCode.ecNotInitialized;
            }
            return SDOAQ_API.SDOAQ_SetCameraGrabbingStatus(grabState);
        }

        unsafe public SDOAQ_API.eErrorCode GetGrabState(out SDOAQ_API.eCameraGrabbingStatus grabState)
        {
            grabState = SDOAQ_API.eCameraGrabbingStatus.cgsOffGrabbing;
            if (IsInitialize == false)
            {
                return SDOAQ_API.eErrorCode.ecNotInitialized;
            }
            return SDOAQ_API.SDOAQ_GetCameraGrabbingStatus(out grabState);
        }

        public SDOAQ_API.eErrorCode SetGain(double gain)
        {
            return SDOAQ_API.SDOAQ_SetCameraParameterDouble(ParameterName.Gain, gain);
        }

        public SDOAQ_API.eErrorCode GetGain(out double gain)
        {
            gain = 0;
            var rv = SDOAQ_API.SDOAQ_GetCameraParameterDouble(ParameterName.Gain, out gain, SDOAQ_API.eCameraParameterType.cptValue);
            return rv;
        }

        public SDOAQ_API.eErrorCode SetWhiteBalance(double red, double green, double blue)
        {
            SDOAQ_API.eErrorCode rv = SDOAQ_API.eErrorCode.ecNoError;
            if (SDOAQ_API.eErrorCode.ecNoError != (rv = SDOAQ_API.SDOAQ_SetCameraParameterString(ParameterName.BalanceRatioSelector, ParameterName.BalanceRatioSelector_Red)))
            {
                return rv;
            }

            if (SDOAQ_API.eErrorCode.ecNoError != (SDOAQ_API.SDOAQ_SetCameraParameterDouble(ParameterName.BalanceRatio, red)))
            {
                return rv;
            }

            if (SDOAQ_API.eErrorCode.ecNoError != (SDOAQ_API.SDOAQ_SetCameraParameterString(ParameterName.BalanceRatioSelector, ParameterName.BalanceRatioSelector_Green)))
            {
                return rv;
            }

            if (SDOAQ_API.eErrorCode.ecNoError != (SDOAQ_API.SDOAQ_SetCameraParameterDouble(ParameterName.BalanceRatio, green)))
            {
                return rv;
            }

            if (SDOAQ_API.eErrorCode.ecNoError != (SDOAQ_API.SDOAQ_SetCameraParameterString(ParameterName.BalanceRatioSelector, ParameterName.BalanceRatioSelector_Blue)))
            {
                return rv;
            }

            if (SDOAQ_API.eErrorCode.ecNoError != (SDOAQ_API.SDOAQ_SetCameraParameterDouble(ParameterName.BalanceRatio, blue)))
            {
                return rv;
            }

            return rv;
        }

        public SDOAQ_API.eErrorCode SetReverseX(bool bReverse)
        {
            return SDOAQ_API.SDOAQ_SetCameraParameterBool(ParameterName.ReverseX, bReverse);
        }

        public SDOAQ_API.eErrorCode SetReverseY(bool bReverse)
        {
            return SDOAQ_API.SDOAQ_SetCameraParameterBool(ParameterName.ReverseY, bReverse);
        }
        #endregion
        #endregion

        #region IDispose
        public void Dispose()
        {
            EanbleCameraFrameCallBack(false);
            _callBackMsgQueue.Dispose();
            MyLogger.Dispose();
            Finalize();
        }
        #endregion

        #region callBackFuntion
        private static void LogCallback(SDOAQ_API.eLogSeverity severity, StringBuilder pMessage)
        {
            _myCamera.MyLogger.WriteLog($"[LOG][{severity}]{pMessage.ToString()}");
        }

        private static void ErrorCallback(SDOAQ_API.eErrorCode errorCode, StringBuilder pErrorMessage)
        {
            _myCamera.MyLogger.WriteLog($"[ERROR]Error Code = {errorCode}, {pErrorMessage.ToString()}");
        }


        private static void InitDoneCallback(SDOAQ_API.eErrorCode errorCode, StringBuilder pErrorMessage)
        {
            if (_myCamera._evtInitDone.IsWaitSet)
            {
                _myCamera._evtInitDone.Set((errorCode, pErrorMessage));
            }

            bool bInitDone = errorCode == SDOAQ_API.eErrorCode.ecNoError;
            if (bInitDone)
            {
                SDOAQ_API.SDOAQ_RegisterLowLevelAuthorization();

                _myCamera.IsInitialize = true;
                _myCamera.MyLogger.WriteLog($"[INIT]Initialize Done");

                //_myCamera.SetTriggerMode(eCameraTriggerMode.ctmSoftware);
            }
            else
            {
                _myCamera.MyLogger.WriteLog($"[INIT]Initialize Error, Error Code = {errorCode}, {pErrorMessage.ToString()}");
            }

            _myCamera.CallBackMsgLoop.Invoke((CallBackMessage.Initialize, new object[] { bInitDone }));
        }

        private static void FrameCallback(SDOAQ_API.eErrorCode errorCode, IntPtr pBuffer, int bufferSize, ref SDOAQ_API.FrameDescriptor frameDescriptor)
        {
            if (errorCode != SDOAQ_API.eErrorCode.ecNoError)
            {
                return;
            }

            if (pBuffer == null || bufferSize == 0)
            {
                return;
            }

            byte[] data = new byte[bufferSize];

            Marshal.Copy(pBuffer, data, 0, bufferSize);

            ++_myCamera.RecivedFrameCount;
            var imgInfo = new SdoaqImageInfo(frameDescriptor.pixelsWidth, frameDescriptor.pixelsHeight, frameDescriptor.bytesLine, frameDescriptor.bytesPixel, data);

            _myCamera.CallBackMsgLoop?.Invoke((CallBackMessage.Frame, new object[] { imgInfo }));
        }
        #endregion

    }
}
