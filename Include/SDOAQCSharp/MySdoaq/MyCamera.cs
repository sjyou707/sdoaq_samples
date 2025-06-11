using System;
using System.Runtime.InteropServices;
using System.Text;

using SDOAQ;
using SDOAQCSharp.Tool;

namespace SDOAQCSharp
{
    public partial class MyCamera : IDisposable
    {
        public enum RequestMsg
        {
            Initialize,
        }

        public enum CallBackMessage
        {
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

        private MyQueue<(CallBackMessage msg, object[] objs)> _callBackMsgQueue = new MyQueue<(CallBackMessage msg, object[] objs)>();

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
       
        public uint RecivedFrameCount { get; private set; } = 0;

        public readonly int CamIndex;

        private MyManualResetEvent<(SDOAQ_API.eErrorCode errorCode, StringBuilder pErrorMessage)> _evtInitDone = new MyManualResetEvent<(SDOAQ_API.eErrorCode errorCode, StringBuilder pErrorMessage)>(false);

        private bool _disposedValue = false;

        
        public MyCamera(int index)
        {
            CamIndex = index;
            if (s_isFirstInitialize)
            {
                Add_CallbackFunction();
                s_isFirstInitialize = false;
            }
        }

        ~MyCamera()
        {
            Dispose();
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    ;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Set/Get Parameter
        public SDOAQ_API.eErrorCode SetTriggerMode(SDOAQ_API.eCameraTriggerMode triggerMode)
        {
            if (IsInitialize == false)
            {
                return SDOAQ_API.eErrorCode.ecNotInitialized;
            }

            SelectMultiWS(CamIndex);

            return SDOAQ_API.SDOAQ_SetCameraTriggerMode(triggerMode);
        }

        public SDOAQ_API.eErrorCode SetExeSoftwareTrigger()
        {
            if (IsInitialize == false)
            {
                return SDOAQ_API.eErrorCode.ecNotInitialized;
            }

            SelectMultiWS(CamIndex);

            return SDOAQ_API.SDOAQ_ExecCameraSoftwareTrigger();
        }

        public SDOAQ_API.eErrorCode SetFOV(int width, int height, int offset_X = 0, int offset_Y = 0, int bining = 1)
        {
            if (IsInitialize == false)
            {
                return SDOAQ_API.eErrorCode.ecNotInitialized;
            }

            SelectMultiWS(CamIndex);

            return SDOAQ_API.SDOAQ_SetCameraRoiParameter(width, height, offset_X, offset_Y, bining);
		}

        public SDOAQ_API.eErrorCode GetFOV(out int width, out int height, out int offset_X, out int offset_Y, out int bining)
        {
            width = 0;
            height = 0;
            offset_X = 0;
            offset_Y = 0;
            bining = 1;

            if (IsInitialize == false)
            {
                return SDOAQ_API.eErrorCode.ecNotInitialized;
            }

            int cam_width = 0;
            int cam_height = 0;
            int cam_offset_X = 0;
            int cam_offset_Y = 0;
            int cam_binning = 0;

            SelectMultiWS(CamIndex);

            var rv = SDOAQ_API.SDOAQ_GetCameraRoiParameter(ref cam_width, ref cam_height, ref cam_offset_X, ref cam_offset_Y, ref cam_binning);

            width = cam_width;
            height = cam_height;
            offset_X = cam_offset_X;
            offset_Y = cam_offset_Y;
            bining = cam_binning;

            return rv;
        }

        public SDOAQ_API.eErrorCode SetExposureTime(int exposureTime)
        {
            SelectMultiWS(CamIndex);

            return SDOAQ_API.SDOAQ_SetIntParameterValue(SDOAQ_API.eParameterId.piCameraExposureTime, exposureTime);
        }

        public SDOAQ_API.eErrorCode GetExposureTime(out int exposureTime)
        {
            SelectMultiWS(CamIndex);

            exposureTime = 0;

            if (IsInitialize == false)
            {
                return SDOAQ_API.eErrorCode.ecNotInitialized;
            }
            
            var rv = SDOAQ_API.SDOAQ_GetIntParameterValue(SDOAQ_API.eParameterId.piCameraExposureTime, ref exposureTime);
            
            return rv;
        }

        #region Features supported only by select cameras
        public SDOAQ_API.eErrorCode SetGrabState(SDOAQ_API.eCameraGrabbingStatus grabState)
        {
            if (IsInitialize == false)
            {
                return SDOAQ_API.eErrorCode.ecNotInitialized;
            }
            SelectMultiWS(CamIndex);
            return SDOAQ_API.SDOAQ_SetCameraGrabbingStatus(grabState);
        }

        unsafe public SDOAQ_API.eErrorCode GetGrabState(out SDOAQ_API.eCameraGrabbingStatus grabState)
        {
            grabState = SDOAQ_API.eCameraGrabbingStatus.cgsOffGrabbing;
            if (IsInitialize == false)
            {
                return SDOAQ_API.eErrorCode.ecNotInitialized;
            }
            SelectMultiWS(CamIndex);
            return SDOAQ_API.SDOAQ_GetCameraGrabbingStatus(out grabState);
        }

        public SDOAQ_API.eErrorCode SetGain(double gain)
        {
            SelectMultiWS(CamIndex);
            return SDOAQ_API.SDOAQ_SetCameraParameterDouble(ParameterName.Gain, gain);
        }

        public SDOAQ_API.eErrorCode GetGain(out double gain)
        {
            gain = 0;
            if (IsInitialize == false)
            {
                return SDOAQ_API.eErrorCode.ecNotInitialized;
            }

            SelectMultiWS(CamIndex);
            var rv = SDOAQ_API.SDOAQ_GetCameraParameterDouble(ParameterName.Gain, out gain, SDOAQ_API.eCameraParameterType.cptValue);
            return rv;
        }

        public SDOAQ_API.eErrorCode SetWhiteBalance(double red, double green, double blue)
        {
            if (IsInitialize == false)
            {
                return SDOAQ_API.eErrorCode.ecNotInitialized;
            }

            SelectMultiWS(CamIndex);

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
            SelectMultiWS(CamIndex);

            return SDOAQ_API.SDOAQ_SetCameraParameterBool(ParameterName.ReverseX, bReverse);
        }

        public SDOAQ_API.eErrorCode SetReverseY(bool bReverse)
        {
            SelectMultiWS(CamIndex);

            return SDOAQ_API.SDOAQ_SetCameraParameterBool(ParameterName.ReverseY, bReverse);
        }
        #endregion

        #endregion
        
        public void AppendLog(Logger.emLogLevel logLevel, string format, params object[] args)
        {
            MyCamera.WriteLog(logLevel, $"[Cam{CamIndex + 1}] {string.Format(format, args)}");
        }
    }
}
