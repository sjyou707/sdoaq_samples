using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SDOAQ;
using SDOAQCSharp.Tool;

namespace SDOAQCSharp
{
    public partial class MySdoaq : IDisposable
    {
        public enum emEofImgViewOption
        {
            StepMap,
            QuaalityMap,
            HeightMap,
            PointClound,
            Edof,
        }

        public enum emCallBackMessage
        {
            FocusStack,
            Edof,
            Af,
            Snap,
        }
        
        public enum emPlayerMode
        {
            None,
            FocusStack,
            Af,
            Edof,
        }

        public enum emPlayerMethod
        {
            CallBackFunc,
            Thread,
        }

        public class EdofImageList
        {
            public bool EnableEdofImg = false;
            public bool EnableStepMapImg = false; 
            public bool EnableQualityMap = false;
            public bool EnableHeightMap = false;
            public bool EnablePointCloud = false;
        }

        public readonly int CamIndex;
        public readonly emPlayerMethod PlayerMethod;
        public bool IsRunPlayer { get; private set; } = false;
        public emPlayerMode PlayerMode { get; private set; } = emPlayerMode.None;
       
        public SdoaqCamInfo CamInfo { get; private set; } = new SdoaqCamInfo();

        public FocusLHU FocusList { get; private set; } = new FocusLHU();
        public FocusLHU SnapFocusList { get; private set; } = new FocusLHU();

        public int PlyerRingBufferSize { get; set; } = DFLT_RING_BUFFER_SIZE;

        private MyQueue<(emCallBackMessage msg, object[] objs)> _callBackMsgQueue = new MyQueue<(emCallBackMessage msg, object[] objs)>();

        public MyQueue<(emCallBackMessage msg, object[] objs)>.MsgLoopCallBack CallBackMsgLoop
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
        
        private RingBuffer _ringBuffer = new RingBuffer();
        private int _playerFoucsStepCount = 0;
        private EdofImageList _edofImageList = new EdofImageList();

        private bool _disposedValue = false;

        public const int DFLT_FOCUS_STEP = 160; //320 Step WiseScope Base (320/2)
        public const int MAX_FOCUS_STEP = 319; //320 Step WiseScope Base
        
        public const double RESIZE_RATIO_ORIGINAL = 1;
        public const double RESIZE_RATIO_HALF = 0.5;
        public const double RESIZE_RATIO_QUARTER = 0.25;

        public const int EDOF_RESULT_IMG_COUNT = 5;

        public const int DFLT_RING_BUFFER_SIZE = 3;
        
        public const string DFLT_FOCUS_LIST = "0-319-32";
        public const string DFLT_AF_ROI = "0,0,100,100";

        public MySdoaq(emPlayerMethod playerMethod)
        {
            if (s_isFirstInitialize)
            {
                Add_CallbackFunction();
                s_isFirstInitialize = false;
            }

            CamIndex = s_sdoaqObjList.Count;
            PlayerMethod = playerMethod;

            if (PlayerMethod == emPlayerMethod.Thread)
            {
                CreateContinuosAcqThread();
            }
        }

        ~MySdoaq()
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
                    if (PlayerMethod == emPlayerMethod.Thread)
                    {
                        DisposeContinuosAcqThread();
                    }
                    
                    _ringBuffer.Dispose();
                    _ringBuffer = null;
                    
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
    }
}
