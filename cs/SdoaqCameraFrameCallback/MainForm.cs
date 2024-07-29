using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SDOWSIO;
using SDOAQCSharp.Tool;

namespace SdoaqCameraFrameCallback
{
    public partial class MainForm : Form
    {
        private MyCamera _myCam;


        private StringBuilder _logBuffer = new StringBuilder();
        private object _lockLog = new object();

        private List<Control> _controlList = new List<Control>();

        private int _tickFrameUpdate = 0;
        private int _skipFrame = 0;

        private SDOAQ.SDOAQ_API.eCameraTriggerMode _seletedTriggerMode = SDOAQ.SDOAQ_API.eCameraTriggerMode.ctmSoftware;
        private bool _checkedCallBackFrame = false;

        private const int FRAME_UPDATE_TIME_INTERVAL = 100;
        private const string STR_CAM_GRABBING = "Grabbing...";
        private const string STR_CAM_IDLE = "IDLE...";

        public MainForm()
        {
            InitializeComponent();

            _controlList.AddRange(new Control[]
            {
                btnInit,
                btnFinal,

                gr_TriggerMode,

                gr_FOV,
                gr_Image,
                gr_GrabState,

                gr_AcquisitionControl,
            });

            CreateViewPanel(pb_CamImage, "Main viewer");

            _myCam = new MyCamera();
            _myCam.MyLogger.DataReceived += Logger_DataReceived;
            _myCam.CallBackMsgLoop += Cam_CallBackMsgLoop;

            rdo_TrigeerMode_FreeRun.Tag = SDOAQ.SDOAQ_API.eCameraTriggerMode.ctmFreerun;
            rdo_TrigeerMode_Software.Tag = SDOAQ.SDOAQ_API.eCameraTriggerMode.ctmSoftware;
            rdo_TrigeerMode_External.Tag = SDOAQ.SDOAQ_API.eCameraTriggerMode.ctmExternal;

            rdo_Grab_ON.Tag = SDOAQ.SDOAQ_API.eCameraGrabbingStatus.cgsOnGrabbing;
            rdo_Grab_OFF.Tag = SDOAQ.SDOAQ_API.eCameraGrabbingStatus.cgsOffGrabbing;

            EnableCameraControl(false, btnInit);

            rdo_TrigeerMode_Software.Checked = true;

            tmr_LogUpdate.Start();
            tmr_GrabStatus.Start();
        }

        private void CreateViewPanel(Control ctrl, string viewrName)
        {
            WSIO.UTIL.WSUT_IV_CreateImageViewer(viewrName
                    , ctrl.Handle, out var hwnd, 0
                    , WSIO.UTIL.WSUTIVOPMODE.WSUTIVOPMODE_VISION
                    | WSIO.UTIL.WSUTIVOPMODE.WSUTIVOPMODE_INFOOSD);

            WSIO.UTIL.WSUT_IV_SetColor(hwnd, WSIO.UTIL.WSUTIVRESOURCE.WSUTIVRESOURCE_OUTERFRAME, Utils.RGB(70, 130, 180));

            ctrl.Tag = hwnd;
        }
        private void LayoutUpdate()
        {
            if (pb_CamImage.Tag != null)
            {
                WSIO.UTIL.WSUT_IV_ShowWindow((IntPtr)pb_CamImage.Tag, 1, pb_CamImage.Left, pb_CamImage.Top - 35, pb_CamImage.Right, pb_CamImage.Bottom - 35);
            }
        }

        private void Logger_DataReceived(object sender, LoggerEventArgs e)
        {
            lock (_lockLog)
            {
                _logBuffer.Append(e.Data);
            }
        }

        private void Cam_CallBackMsgLoop((MyCamera.CallBackMessage msg, object[] objs) item)
        {
            switch (item.msg)
            {
                case MyCamera.CallBackMessage.Initialize:
                    {
                        this.BeginInvoke(new MethodInvoker(() =>
                        {
                            _skipFrame = 0;

                            _myCam.GetFOV(out var width, out var height);

                            txt_FOV_Width.Text = width.ToString();
                            txt_FOV_Height.Text = height.ToString();

                            _myCam.GetExposureTime(out var exposureTime);

                            txt_ExposureTime.Text = exposureTime.ToString();

                            _myCam.GetGain(out var gain);
                            txt_Gain.Text = gain.ToString();

                            _myCam.EanbleCameraFrameCallBack(_checkedCallBackFrame);
                            _myCam.SetTriggerMode(_seletedTriggerMode);

                            _myCam.GetGrabState(out var grabState);
                            if (grabState == SDOAQ.SDOAQ_API.eCameraGrabbingStatus.cgsOnGrabbing)
                            {
                                rdo_Grab_ON.Checked = true;
                            }
                            else
                            {
                                rdo_Grab_OFF.Checked = true;
                            }
                            EnableCameraControl(true);
                        }));
                    }
                    break;
                case MyCamera.CallBackMessage.Frame:
                    {
                        //Frame is too fast to prevent UI lock
                        if (_tickFrameUpdate < Environment.TickCount)
                        {
                            var imgInfo = (SdoaqImageInfo)item.objs[0];

                            WSIO.UTIL.WSUT_IV_AttachRawImgData((IntPtr)pb_CamImage.Tag, (uint)imgInfo.Width, (uint)imgInfo.Height,
                                (uint)imgInfo.Line,
                                (uint)imgInfo.PixelBytes,
                                imgInfo.Data, (uint)imgInfo.Data.Length);

                            this.BeginInvoke(new MethodInvoker(() =>
                            {
                                var frameCount = _myCam.RecivedFrameCount;
                                lbl_ImageStatus.Text = $"Received Frame = { frameCount}  (Displayed Frame = { frameCount - _skipFrame})";
                            }));

                            _tickFrameUpdate = Environment.TickCount + FRAME_UPDATE_TIME_INTERVAL;
                        }
                        else
                        {
                            ++_skipFrame;
                        }
                    }
                    break;
            }
        }

        private void EnableCameraControl(bool bEnable, params Control[] alwaysEnableControls)
        {
            foreach (var ctrl in _controlList)
            {
                if (bEnable)
                {
                    ctrl.Enabled = true;
                }
                else if (alwaysEnableControls != null && alwaysEnableControls.Contains(ctrl))
                {
                    ctrl.Enabled = true;
                }
                else
                {
                    ctrl.Enabled = false;
                }
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            LayoutUpdate();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _myCam.MyLogger.WriteLog($"SDOAQ Version : {_myCam.GetVersion()}");
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _myCam.Dispose();
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            _myCam.MyLogger.WriteLog($"Initialize Start");
            EnableCameraControl(false);
            _myCam.Initialize(false);
        }

        private void btnFinal_Click(object sender, EventArgs e)
        {
            _myCam.MyLogger.WriteLog($"Finalize");
            _myCam.Finalize();
            EnableCameraControl(false, btnInit);
        }

        private void cb_EnableCameraFrameCallBack_CheckedChanged(object sender, EventArgs e)
        {
            _checkedCallBackFrame = (sender as CheckBox).Checked;
            _myCam.EanbleCameraFrameCallBack(_checkedCallBackFrame);
        }

        private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            LayoutUpdate();
        }

        private void rdo_TrigeerMode_CheckedChanged(object sender, EventArgs e)
        {
            var rdo = sender as RadioButton;

            if (rdo.Checked == false)
            {
                return;
            }
            _myCam.MyLogger.WriteLog($"Selected TriggerMode {rdo.Text}");

            _seletedTriggerMode = (SDOAQ.SDOAQ_API.eCameraTriggerMode)rdo.Tag;
            _myCam.SetGrabState(SDOAQ.SDOAQ_API.eCameraGrabbingStatus.cgsOffGrabbing);
            _myCam.SetTriggerMode(_seletedTriggerMode);
            _myCam.SetGrabState(SDOAQ.SDOAQ_API.eCameraGrabbingStatus.cgsOnGrabbing);
        }

        private void btn_SwTrigger_Click(object sender, EventArgs e)
        {
            _myCam.MyLogger.WriteLog($"Set Exe Software Trigger");
            _myCam.SetExeSoftwareTrigger();
        }

        private void tmr_LogUpdate_Tick(object sender, EventArgs e)
        {
            if (_logBuffer.Length == 0)
            {
                return;
            }

            lock (_lockLog)
            {
                txt_Log.AppendText(_logBuffer.ToString());
                txt_Log.ScrollToCaret();
                _logBuffer.Clear();
            }
        }

        private void btn_FOV_Set_Click(object sender, EventArgs e)
        {
            _myCam.SetGrabState(SDOAQ.SDOAQ_API.eCameraGrabbingStatus.cgsOffGrabbing);

            var width = int.Parse(txt_FOV_Width.Text);
            var height = int.Parse(txt_FOV_Height.Text);

            _myCam.SetFOV(width, height);

            _myCam.GetFOV(out width, out height);

            txt_FOV_Width.Text = width.ToString();
            txt_FOV_Height.Text = height.ToString();
            _myCam.SetGrabState(SDOAQ.SDOAQ_API.eCameraGrabbingStatus.cgsOnGrabbing);

            _myCam.MyLogger.WriteLog($"SetFOV : {txt_FOV_Width.Text} x {txt_FOV_Height.Text}");
        }

        private void btn_ExposureTime_Set_Click(object sender, EventArgs e)
        {
            var exposureTime = int.Parse(txt_ExposureTime.Text);
            _myCam.SetExposureTime(exposureTime);

            _myCam.GetExposureTime(out exposureTime);

            txt_ExposureTime.Text = exposureTime.ToString();

            _myCam.MyLogger.WriteLog($"Set ExposureTime : {txt_ExposureTime.Text}");
        }

        private void btn_Gain_Set_Click(object sender, EventArgs e)
        {
            var gain = double.Parse(txt_Gain.Text);
            _myCam.SetGain(gain);

            _myCam.GetGain(out gain);

            txt_Gain.Text = gain.ToString();

            _myCam.MyLogger.WriteLog($"Set Gain : {txt_Gain.Text}");
        }



        private void rdo_Grab_Click(object sender, EventArgs e)
        {
            _myCam.SetGrabState((SDOAQ.SDOAQ_API.eCameraGrabbingStatus)(sender as Control).Tag);
        }

        private void tmr_GrabStatus_Tick(object sender, EventArgs e)
        {
            if (_myCam.IsInitialize == false)
            {
                btn_GrabStatus.BackColor = System.Drawing.Color.Transparent;
                btn_GrabStatus.Text = string.Empty;
                return;
            }

            _myCam.GetGrabState(out var grabState);

            if (grabState == SDOAQ.SDOAQ_API.eCameraGrabbingStatus.cgsOnGrabbing)
            {
                btn_GrabStatus.BackColor = System.Drawing.Color.LawnGreen;
                btn_GrabStatus.Text = STR_CAM_GRABBING;
            }
            else
            {
                btn_GrabStatus.BackColor = System.Drawing.Color.Yellow;
                btn_GrabStatus.Text = STR_CAM_IDLE;
            }
        }
    }
}
