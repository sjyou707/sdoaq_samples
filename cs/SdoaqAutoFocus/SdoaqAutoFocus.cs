using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SDOWSIO;
using SDOAQ;
using SDOAQCSharp.Tool;
using SDOAQCSharp;
using SDOAQCSharp.Component;

namespace SdoaqAutoFocus
{
    public partial class SdoaqAutoFocus : Form
    {
        private StringBuilder _logBuffer = new StringBuilder();
        private object _lockLog = new object();
        private Dictionary<int, MySdoaq> _sdoaqObjList = null;

        private SdoaqImageViewr _imgViewer;

        public SdoaqAutoFocus()
        {
            InitializeComponent();

            _imgViewer = new SdoaqImageViewr(false);
            _imgViewer.Dock = DockStyle.Fill;

            pnl_Viewer.Controls.Add(_imgViewer);

            _sdoaqObjList = MySdoaq.LoadScript();

            _imgViewer.Set_SdoaqObj(GetSdoaqObj());

            var aaa = GetSdoaqObj();

            MySdoaq.LogReceived += Sdoaq_LogDataReceived;            
        }

        private void SdoaqAutoFocus_Load(object sender, EventArgs e)
        {
            tmr_LogUpdate.Start();
            Frm_Load();
        }

        private void Frm_Load()
        {
            MySdoaq.Initialize();

        }
        private MySdoaq GetSdoaqObj()
        {
            return _sdoaqObjList[0];
        }

        #region [Event]
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

        private void Sdoaq_LogDataReceived(object sender, LoggerEventArgs e)
        {
            lock (_lockLog)
            {
                _logBuffer.Append(e.Data);
            }
        }        
        #endregion

        private void btn_SetROI_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            var task = GetSdoaqObj().Acquisition_AfAsync();
        }
    }
}
