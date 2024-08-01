using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SDOAQ;
using SDOAQCSharp.Tool;
using SDOAQCSharp;
using SDOAQCSharp.Component;

namespace SdoaqEdof
{
    public partial class SdoaqEDoF : Form
    {
        private StringBuilder _logBuffer = new StringBuilder();
        private object _lockLog = new object();
        private Dictionary<int, MySdoaq> _sdoaqObjList = null;

        private SdoaqImageViewr _imgViewer;

        public SdoaqEDoF()
        {
            InitializeComponent();

            _imgViewer = new SdoaqImageViewr(false);
            _imgViewer.Dock = DockStyle.Fill;

            pnl_Viewer.Controls.Add(_imgViewer);
            _sdoaqObjList = MySdoaq.LoadScript();
            _imgViewer.Set_SdoaqObj(GetSdoaqObj());

            MySdoaq.LogReceived += Sdoaq_LogDataReceived;
        }

        private void SdoaqEDoF_Load(object sender, EventArgs e)
        {
            OpenFileDialogSet();
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

        #region [Evet]
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

        private void Write_Log(string str)
        {
            Sdoaq_LogDataReceived(null, new LoggerEventArgs(str));
        }


        #endregion

        private void btn_SingleShotEDoF_Click(object sender, EventArgs e)
        {
            var task = GetSdoaqObj()?.Acquisition_EdofAsync();
        }

        private void btn_PlayEDoF_Click(object sender, EventArgs e)
        {
            GetSdoaqObj()?.AcquisitionContinuous_Edof();
        }

        private void btn_StopEDoF_Click(object sender, EventArgs e)
        {
            GetSdoaqObj()?.AcquisitionStop_Edof();
        }

        private void btn_SetROI_Click(object sender, EventArgs e)
        {
            GetSdoaqObj()?.SetRoi(txt_ROI.Text);
        }

        private void btn_SetMALSFocus_Click(object sender, EventArgs e)
        {
            GetSdoaqObj()?.SetFocus(txt_MALSFocus.Text);
        }

        private void btn_SetResizeRatio_Click(object sender, EventArgs e)
        {
            GetSdoaqObj()?.SetParam(SDOAQ_API.eParameterId.pi_edof_calc_resize_ratio, cmb_EdofResizeRatio.SelectedItem.ToString());
        }

        private void btn_SetKernelSize_Click(object sender, EventArgs e)
        {
            bool isSet = false;
            if(Int32.TryParse(txt_KernelSize.Text, out int value))
            {
                if(value >= 3 && value <=5)
                {
                    isSet = true;
                }
            }

            if(isSet)
            {
                var rv = SDOAQ_API.SDOAQ_SetIntParameterValue(SDOAQ_API.eParameterId.pi_edof_calc_pixelwise_kernel_size, value);
            }
            else
            {
                Write_Log("Value is out of range[3 ~ 5]");
            }
        }

        private void btn_SetIteraion_Click(object sender, EventArgs e)
        {
            bool isSet = false;
            if (Int32.TryParse(txt_Iteraion.Text, out int value))
            {
                if (value >= 0 && value <= 16)
                {
                    isSet = true;
                }
            }

            if (isSet)
            {
                var rv = SDOAQ_API.SDOAQ_SetIntParameterValue(SDOAQ_API.eParameterId.pi_edof_calc_pixelwise_iteration, value);
            }
            else
            {
                Write_Log("Value is out of range[0 ~ 16]");
            }
        }

        private void btn_SetThreshold_Click(object sender, EventArgs e)
        {
            bool isSet = false;
            if (Double.TryParse(txt_Threshold.Text, out double value))
            {
                if (value >= 0.0d && value <= 9.9d)
                {
                    isSet = true;
                }
            }

            if (isSet)
            {
                var rv = SDOAQ_API.SDOAQ_SetDblParameterValue(SDOAQ_API.eParameterId.pi_edof_depth_quality_th, value);
            }
            else
            {
                Write_Log("Value is out of range[0.0 ~ 9.9]");
            }            
        }

        private void btn_SetScaleStep_Click(object sender, EventArgs e)
        {
            GetSdoaqObj()?.GetIntParamRange(SDOAQ_API.eParameterId.pi_edof_scale_correction_dst_step, out int min, out int max);

            bool isSet = false;
            if (Int32.TryParse(txt_ScaleStep.Text, out int value))
            {
                if (value >= min && value <= max)
                {
                    isSet = true;
                }
            }

            if (isSet)
            {
                var rv = SDOAQ_API.SDOAQ_SetIntParameterValue(SDOAQ_API.eParameterId.pi_edof_scale_correction_dst_step, value);
            }
            else
            {
                Write_Log($"Value is out of range[{min} ~ {max}]");
            }           
        }

        private void btn_OpenCalibration_Click(object sender, EventArgs e)
        {
            string fullName = "";
            if(openFile.ShowDialog() == DialogResult.OK)
            {
                fullName = openFile.FileName;

                var rv_sdoaq = SDOAQ_API.SDOAQ_SetCalibrationFile(fullName);
            }
        }

        private void OpenFileDialogSet()
        {
            openFile.Title = "Select calibration file for objective";
            openFile.FileName = "";
            openFile.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
        }
    }
}
