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
using SDOAQCSharp.Tool;
using SDOAQCSharp;
using SDOAQCSharp.Component;

namespace SdoaqAutoFocus
{
    public partial class SdoaqAutoFocus : Form
    {
        private StringBuilder _logBuffer = new StringBuilder();
        private object _lockLog = new object();

        public SdoaqAutoFocus()
        {
            InitializeComponent();

            MySdoaq.LogReceived += Sdoaq_LogDataReceived;
            tmr_LogUpdate.Start();
        }
        private void SdoaqAutoFocus_Load(object sender, EventArgs e)
        {
            Frm_Load();
        }

        private void Frm_Load()
        {      
            MySdoaq.LoadScript();
            MySdoaq.Initialize();
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
    }
}
