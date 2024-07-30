using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SDOAQ;
using SDOAQCSharp.Tool;

namespace SDOAQCSharp
{
    public partial class MySdoaq
    {
        public enum EofImgViewOption
        {
            StepMap,
            QuaalityMap,
            HeightMap,
            PointClound,
            Edof,
        }

        public enum CallBackMessage
        {
            FocusStack,
            Edof,
            Af,
            Snap,
        }
        
        public enum PlayerMode
        {
            None,
            FocusStack,
            Af,
            Edof,
        }
        public readonly int CamIndex;
        
        public bool IsRunPlayer { get; private set; } = false;
        private PlayerMode _playerMode = PlayerMode.None;
        public PlayerMode CurrentPlayerMode => _playerMode;

        public SdoaqCamInfo CamInfo { get; private set; } = new SdoaqCamInfo();

        public FocusLHU FocusList { get; private set; } = new FocusLHU();
        public FocusLHU SnapFocusList { get; private set; } = new FocusLHU();

        public int PlyerRingBufferSize { get; set; } = DFLT_RING_BUFFER_SIZE;

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

        
        private RingBuffer _ringBuffer = new RingBuffer();
        private int _playerFoucsStepCount = 0;

        public const int DFLT_FOCUS_STEP = 160; //320 Step WiseScope Base (320/2)
        public const int MAX_FOCUS_STEP = 319; //320 Step WiseScope Base
        
        public const double RESIZE_RATIO_ORIGINAL = 1;
        public const double RESIZE_RATIO_HALF = 0.5;
        public const double RESIZE_RATIO_QUARTER = 0.25;

        public const int EDOF_RESULT_IMG_COUNT = 5;

        public const int DFLT_RING_BUFFER_SIZE = 3;
        
        public const string DFLT_FOCUS_LIST = "0-319-32";
        public const string DFLT_AF_ROI = "0,0,100,100";

        public MySdoaq()
        {
            if (s_isFirstInitialize)
            {
                Add_CallbackFunction();
                s_isFirstInitialize = false;
            }

            CamIndex = s_sdoaqObjList.Count;            
        }
    }
}
