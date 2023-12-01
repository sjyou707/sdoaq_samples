using System;
using System.Runtime.InteropServices;
using System.Text;

using SDOAQCSharpTool;

using static SDOAQ.SDOAQ_API;

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

        private MyManualResetEvent<(eErrorCode errorCode, StringBuilder pErrorMessage)> _evtInitDone = new MyManualResetEvent<(eErrorCode errorCode, StringBuilder pErrorMessage)>(false);

        private static SDOAQ_FrameCallback SDOAQ_FrameCallback;
        private static SDOAQ_LogCallback SDOAQ_LogCallback;
        private static SDOAQ_ErrorCallback SDOAQ_ErrorCallback;
        private static SDOAQ_InitDoneCallback SDOAQ_InitDoneCallback;

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
            return $"{SDOAQ_GetMajorVersion()}.{SDOAQ_GetMinorVersion()}.{SDOAQ_GetPatchVersion()}";
        }

        public eErrorCode Initialize(bool bSync = false)
        {
            IsInitialize = false;
            RecivedFrameCount = 0;
            if (bSync)
            {
                _evtInitDone.Reset();
            }

            var rv = SDOAQ_Initialize(null, null, SDOAQ_InitDoneCallback);

            if (bSync)
            {
                _evtInitDone.WaitOne();
                rv = _evtInitDone.ReturnValue.errorCode;
            }
            return rv;
        }

        public eErrorCode Finalize()
        {
            IsInitialize = false;
            return SDOAQ_Finalize();
        }

        public void EanbleCameraFrameCallBack(bool bEnable)
        {
            SDOAQ_SetFrameCallback(bEnable ? SDOAQ_FrameCallback : null);
        }

        #region Set/Get Parameter
        public eErrorCode SetTriggerMode(eCameraTriggerMode triggerMode)
        {
            if (IsInitialize == false)
            {
                return eErrorCode.ecNotInitialized;
            }
            return SDOAQ_SetCameraTriggerMode(triggerMode);
        }

        public eErrorCode SetExeSoftwareTrigger()
        {
            if (IsInitialize == false)
            {
                return eErrorCode.ecNotInitialized;
            }

            return SDOAQ_ExecCameraSoftwareTrigger();
        }

        public eErrorCode SetFOV(int width, int height)
        {
            if (IsInitialize == false)
            {
                return eErrorCode.ecNotInitialized;
            }

            return SDOAQ_SetCameraParameter(width, height, 1);
        }

        public eErrorCode GetFOV(out int width, out int height)
        {
            width = 0;
            height = 0;
            if (IsInitialize == false)
            {
                return eErrorCode.ecNotInitialized;
            }

            int[] cam_width = new int[1];
            int[] cam_height = new int[1];
            int[] cam_binning = new int[1];

            var rv = SDOAQ_GetCameraParameter(cam_width, cam_height, cam_binning);

            width = cam_width[0];
            height = cam_height[0];
            return rv;
        }

        public eErrorCode SetExposureTime(int exposureTime)
        {
            return SDOAQ_SetIntParameterValue(eParameterId.piCameraExposureTime, exposureTime);
        }

        public eErrorCode GetExposureTime(out int exposureTime)
        {
            exposureTime = 0;
            int[] tmp = new int[1];
            var rv = SDOAQ_GetIntParameterValue(eParameterId.piCameraExposureTime, tmp);

            if (rv == eErrorCode.ecNoError)
            {
                exposureTime = tmp[0];
            }
            return rv;
        }

        #region Features supported only by select cameras
        public eErrorCode SetGrabState(eCameraGrabbingStatus grabState)
        {
            if (IsInitialize == false)
            {
                return eErrorCode.ecNotInitialized;
            }
            return SDOAQ_SetCameraGrabbingStatus(grabState);
        }

        unsafe public eErrorCode GetGrabState(out eCameraGrabbingStatus grabState)
        {
            grabState = eCameraGrabbingStatus.cgsOffGrabbing;
            if (IsInitialize == false)
            {
                return eErrorCode.ecNotInitialized;
            }
            return SDOAQ_GetCameraGrabbingStatus(out grabState);
        }

        public eErrorCode SetGain(double gain)
        {
            return SDOAQ_SetCameraParameterDouble(ParameterName.Gain, gain);
        }

        public eErrorCode GetGain(out double gain)
        {
            gain = 0;
            var rv = SDOAQ_GetCameraParameterDouble(ParameterName.Gain, out gain, eCameraParameterType.cptValue);
            return rv;
        }

        public eErrorCode SetWhiteBalance(double red, double green, double blue)
        {
            eErrorCode rv = eErrorCode.ecNoError;
            if (eErrorCode.ecNoError != (rv = SDOAQ_SetCameraParameterString(ParameterName.BalanceRatioSelector, ParameterName.BalanceRatioSelector_Red)))
            {
                return rv;
            }

            if (eErrorCode.ecNoError != (SDOAQ_SetCameraParameterDouble(ParameterName.BalanceRatio, red)))
            {
                return rv;
            }

            if (eErrorCode.ecNoError != (SDOAQ_SetCameraParameterString(ParameterName.BalanceRatioSelector, ParameterName.BalanceRatioSelector_Green)))
            {
                return rv;
            }

            if (eErrorCode.ecNoError != (SDOAQ_SetCameraParameterDouble(ParameterName.BalanceRatio, green)))
            {
                return rv;
            }

            if (eErrorCode.ecNoError != (SDOAQ_SetCameraParameterString(ParameterName.BalanceRatioSelector, ParameterName.BalanceRatioSelector_Blue)))
            {
                return rv;
            }

            if (eErrorCode.ecNoError != (SDOAQ_SetCameraParameterDouble(ParameterName.BalanceRatio, blue)))
            {
                return rv;
            }

            return rv;
        }

        public eErrorCode SetReverseX(bool bReverse)
        {
            return SDOAQ_SetCameraParameterBool(ParameterName.ReverseX, bReverse);
        }

        public eErrorCode SetReverseY(bool bReverse)
        {
            return SDOAQ_SetCameraParameterBool(ParameterName.ReverseY, bReverse);
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
        private static void LogCallback(eLogSeverity severity, StringBuilder pMessage)
        {
            _myCamera.MyLogger.WriteLog($"[LOG][{severity}]{pMessage.ToString()}");
        }

        private static void ErrorCallback(eErrorCode errorCode, StringBuilder pErrorMessage)
        {
            _myCamera.MyLogger.WriteLog($"[ERROR]Error Code = {errorCode}, {pErrorMessage.ToString()}");
        }


        private static void InitDoneCallback(eErrorCode errorCode, StringBuilder pErrorMessage)
        {
            if (_myCamera._evtInitDone.IsWaitSet)
            {
                _myCamera._evtInitDone.Set((errorCode, pErrorMessage));
            }

            bool bInitDone = errorCode == eErrorCode.ecNoError;
            if (bInitDone)
            {
                SDOAQ_RegisterLowLevelAuthorization();

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

        private static void FrameCallback(eErrorCode errorCode, IntPtr pBuffer, int bufferSize, ref FrameDescriptor frameDescriptor)
        {
            if (errorCode != eErrorCode.ecNoError)
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
