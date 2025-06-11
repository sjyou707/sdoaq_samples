using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SDOWSIO;
using SDOAQCSharp.Tool;
using SDOAQCSharp;
using System.Threading.Tasks;
using System.Drawing;

namespace SdoaqMultiCameraFrameCallback
{
    public partial class MainForm : Form
    {
        private Dictionary<int, MyCamera> _sdoaqCamList = null;
        private Dictionary<int, CamViewer> _camViewerList;

        private StringBuilder _logBuffer = new StringBuilder();
        private object _lockLog = new object();
        
        private readonly static SolidBrush BRUSH_SELECTED_TAB = new SolidBrush(Color.FromArgb(122, 159, 205));

        public MainForm()
        {
            InitializeComponent();
            
            _sdoaqCamList = MyCamera.LoadScript();
            _camViewerList = new Dictionary<int, CamViewer>();

            foreach (var item in _sdoaqCamList)
            {
                int idxCam = item.Value.CamIndex + 1;
                var camViewer = new CamViewer();
                camViewer.Name = $"CamViewr_Cam{idxCam}";

                camViewer.Dock = DockStyle.Fill;
                camViewer.Set_CamObject(item.Value);

                var tabPage = new TabPage();
                tabPage.Name = $"CamParamPage_{idxCam}";
                tabPage.Text = $"Cam {idxCam}";
                tabPage.Controls.Add(camViewer);

                tab_CamPage.Controls.Add(tabPage);


                _camViewerList.Add(item.Key, camViewer);
            }

            MyCamera.LogReceived += Logger_DataReceived;
            MyCamera.Initialized += MyCamera_Initialized;
            
            tmr_LogUpdate.Start();
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

        private MyCamera GetCamObj()
        {
            return _sdoaqCamList[0];
        }
        

        private void Logger_DataReceived(object sender, LoggerEventArgs e)
        {
            lock (_lockLog)
            {
                _logBuffer.Append(e.Data);
            }
        }

        private void MyCamera_Initialized(object sender, SdoaqEventArgs e)
        {
            MyCamera.EanbleCameraFrameCallBack(true);

            foreach (var item in _camViewerList)
            {
                item.Value.Update_CamParam();
            }
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            MyCamera.WriteLog(Logger.emLogLevel.Info, $"SDOAQ Version : {MyCamera.GetVersion()}");

            MyCamera.WriteLog(Logger.emLogLevel.User, $"Initialize Start");

            MyCamera.SDOAQ_Initialize(false);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyCamera.LogReceived -= Logger_DataReceived;
            MyCamera.Initialized -= MyCamera_Initialized;

            MyCamera.EanbleCameraFrameCallBack(false);

            foreach (var cam in _sdoaqCamList.Values)
            {
                cam.SetTriggerMode(SDOAQ.SDOAQ_API.eCameraTriggerMode.ctmSoftware);
                cam.SetGrabState(SDOAQ.SDOAQ_API.eCameraGrabbingStatus.cgsOffGrabbing);
            }

            MyCamera.DisposeStaticResouce();

            MyCamera.SDOAQ_Finalize();
        }
        
        private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
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
        
        private void tab_CamPage_DrawItem(object sender, DrawItemEventArgs e)
        {

            var tab = sender as TabControl;
            Rectangle paddedBounds = e.Bounds;
            Size textLength = TextRenderer.MeasureText(tab.TabPages[e.Index].Text, e.Font);

            paddedBounds.Inflate(-((e.Bounds.Width - textLength.Width) / 2), -((e.Bounds.Height - textLength.Height) / 2));

            if (tab.SelectedIndex == e.Index)
            {
                e.Graphics.FillRectangle(BRUSH_SELECTED_TAB, e.Bounds);
                e.Graphics.DrawString(tab.TabPages[e.Index].Text, e.Font, SystemBrushes.WindowText, paddedBounds);
            }
            else
            {
                e.Graphics.DrawString(tab.TabPages[e.Index].Text, e.Font, SystemBrushes.GrayText, paddedBounds);
            }
        }
    }
}
