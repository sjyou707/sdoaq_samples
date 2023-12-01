namespace SdoaqCameraFrameCallback
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.gr_Image = new System.Windows.Forms.GroupBox();
            this.txt_Gain = new System.Windows.Forms.TextBox();
            this.txt_ExposureTime = new System.Windows.Forms.TextBox();
            this.btn_Gain_Set = new System.Windows.Forms.Button();
            this.btn_ExposureTime_Set = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.gr_FOV = new System.Windows.Forms.GroupBox();
            this.txt_FOV_Height = new System.Windows.Forms.TextBox();
            this.txt_FOV_Width = new System.Windows.Forms.TextBox();
            this.btn_FOV_Set = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.gr_Init = new System.Windows.Forms.GroupBox();
            this.btnInit = new System.Windows.Forms.Button();
            this.btnFinal = new System.Windows.Forms.Button();
            this.gr_GrabState = new System.Windows.Forms.GroupBox();
            this.btn_GrabStatus = new System.Windows.Forms.Button();
            this.rdo_Grab_OFF = new System.Windows.Forms.RadioButton();
            this.rdo_Grab_ON = new System.Windows.Forms.RadioButton();
            this.gr_TriggerMode = new System.Windows.Forms.GroupBox();
            this.rdo_TrigeerMode_External = new System.Windows.Forms.RadioButton();
            this.rdo_TrigeerMode_Software = new System.Windows.Forms.RadioButton();
            this.rdo_TrigeerMode_FreeRun = new System.Windows.Forms.RadioButton();
            this.gr_AcquisitionControl = new System.Windows.Forms.GroupBox();
            this.cb_EnableCameraFrameCallBack = new System.Windows.Forms.CheckBox();
            this.btn_SwTrigger = new System.Windows.Forms.Button();
            this.txt_Log = new System.Windows.Forms.RichTextBox();
            this.pb_CamImage = new System.Windows.Forms.PictureBox();
            this.lbl_ImageStatus = new System.Windows.Forms.Label();
            this.tmr_LogUpdate = new System.Windows.Forms.Timer(this.components);
            this.tmr_GrabStatus = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.gr_Image.SuspendLayout();
            this.gr_FOV.SuspendLayout();
            this.gr_Init.SuspendLayout();
            this.gr_GrabState.SuspendLayout();
            this.gr_TriggerMode.SuspendLayout();
            this.gr_AcquisitionControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_CamImage)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.gr_Image);
            this.splitContainer.Panel1.Controls.Add(this.gr_FOV);
            this.splitContainer.Panel1.Controls.Add(this.gr_Init);
            this.splitContainer.Panel1.Controls.Add(this.gr_GrabState);
            this.splitContainer.Panel1.Controls.Add(this.gr_TriggerMode);
            this.splitContainer.Panel1.Controls.Add(this.gr_AcquisitionControl);
            this.splitContainer.Panel1.Controls.Add(this.txt_Log);
            this.splitContainer.Panel1MinSize = 446;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.pb_CamImage);
            this.splitContainer.Panel2.Controls.Add(this.lbl_ImageStatus);
            this.splitContainer.Size = new System.Drawing.Size(1264, 681);
            this.splitContainer.SplitterDistance = 610;
            this.splitContainer.SplitterWidth = 3;
            this.splitContainer.TabIndex = 0;
            this.splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
            // 
            // gr_Image
            // 
            this.gr_Image.Controls.Add(this.txt_Gain);
            this.gr_Image.Controls.Add(this.txt_ExposureTime);
            this.gr_Image.Controls.Add(this.btn_Gain_Set);
            this.gr_Image.Controls.Add(this.btn_ExposureTime_Set);
            this.gr_Image.Controls.Add(this.label4);
            this.gr_Image.Controls.Add(this.label3);
            this.gr_Image.Location = new System.Drawing.Point(222, 196);
            this.gr_Image.Name = "gr_Image";
            this.gr_Image.Size = new System.Drawing.Size(286, 100);
            this.gr_Image.TabIndex = 4;
            this.gr_Image.TabStop = false;
            this.gr_Image.Text = "Image";
            // 
            // txt_Gain
            // 
            this.txt_Gain.Location = new System.Drawing.Point(145, 61);
            this.txt_Gain.Name = "txt_Gain";
            this.txt_Gain.Size = new System.Drawing.Size(65, 23);
            this.txt_Gain.TabIndex = 2;
            // 
            // txt_ExposureTime
            // 
            this.txt_ExposureTime.Location = new System.Drawing.Point(145, 28);
            this.txt_ExposureTime.Name = "txt_ExposureTime";
            this.txt_ExposureTime.Size = new System.Drawing.Size(65, 23);
            this.txt_ExposureTime.TabIndex = 2;
            // 
            // btn_Gain_Set
            // 
            this.btn_Gain_Set.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Gain_Set.Location = new System.Drawing.Point(216, 61);
            this.btn_Gain_Set.Name = "btn_Gain_Set";
            this.btn_Gain_Set.Size = new System.Drawing.Size(64, 23);
            this.btn_Gain_Set.TabIndex = 0;
            this.btn_Gain_Set.Text = "Set";
            this.btn_Gain_Set.UseVisualStyleBackColor = true;
            this.btn_Gain_Set.Click += new System.EventHandler(this.btn_Gain_Set_Click);
            // 
            // btn_ExposureTime_Set
            // 
            this.btn_ExposureTime_Set.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ExposureTime_Set.Location = new System.Drawing.Point(216, 28);
            this.btn_ExposureTime_Set.Name = "btn_ExposureTime_Set";
            this.btn_ExposureTime_Set.Size = new System.Drawing.Size(64, 23);
            this.btn_ExposureTime_Set.TabIndex = 0;
            this.btn_ExposureTime_Set.Text = "Set";
            this.btn_ExposureTime_Set.UseVisualStyleBackColor = true;
            this.btn_ExposureTime_Set.Click += new System.EventHandler(this.btn_ExposureTime_Set_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(104, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "Gain";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Exposure Time (us)";
            // 
            // gr_FOV
            // 
            this.gr_FOV.Controls.Add(this.txt_FOV_Height);
            this.gr_FOV.Controls.Add(this.txt_FOV_Width);
            this.gr_FOV.Controls.Add(this.btn_FOV_Set);
            this.gr_FOV.Controls.Add(this.label2);
            this.gr_FOV.Controls.Add(this.label1);
            this.gr_FOV.Location = new System.Drawing.Point(16, 196);
            this.gr_FOV.Name = "gr_FOV";
            this.gr_FOV.Size = new System.Drawing.Size(200, 100);
            this.gr_FOV.TabIndex = 4;
            this.gr_FOV.TabStop = false;
            this.gr_FOV.Text = "FOV";
            // 
            // txt_FOV_Height
            // 
            this.txt_FOV_Height.Location = new System.Drawing.Point(54, 61);
            this.txt_FOV_Height.Name = "txt_FOV_Height";
            this.txt_FOV_Height.Size = new System.Drawing.Size(69, 23);
            this.txt_FOV_Height.TabIndex = 2;
            // 
            // txt_FOV_Width
            // 
            this.txt_FOV_Width.Location = new System.Drawing.Point(54, 28);
            this.txt_FOV_Width.Name = "txt_FOV_Width";
            this.txt_FOV_Width.Size = new System.Drawing.Size(69, 23);
            this.txt_FOV_Width.TabIndex = 2;
            // 
            // btn_FOV_Set
            // 
            this.btn_FOV_Set.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_FOV_Set.Location = new System.Drawing.Point(130, 28);
            this.btn_FOV_Set.Name = "btn_FOV_Set";
            this.btn_FOV_Set.Size = new System.Drawing.Size(64, 56);
            this.btn_FOV_Set.TabIndex = 0;
            this.btn_FOV_Set.Text = "Set";
            this.btn_FOV_Set.UseVisualStyleBackColor = true;
            this.btn_FOV_Set.Click += new System.EventHandler(this.btn_FOV_Set_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Height";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Width";
            // 
            // gr_Init
            // 
            this.gr_Init.Controls.Add(this.btnInit);
            this.gr_Init.Controls.Add(this.btnFinal);
            this.gr_Init.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gr_Init.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.gr_Init.Location = new System.Drawing.Point(16, 5);
            this.gr_Init.Name = "gr_Init";
            this.gr_Init.Size = new System.Drawing.Size(371, 63);
            this.gr_Init.TabIndex = 3;
            this.gr_Init.TabStop = false;
            // 
            // btnInit
            // 
            this.btnInit.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.ControlLight;
            this.btnInit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInit.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInit.Location = new System.Drawing.Point(7, 22);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(165, 29);
            this.btnInit.TabIndex = 1;
            this.btnInit.Text = "Initialize";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // btnFinal
            // 
            this.btnFinal.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.ControlLight;
            this.btnFinal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFinal.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.btnFinal.Location = new System.Drawing.Point(190, 22);
            this.btnFinal.Name = "btnFinal";
            this.btnFinal.Size = new System.Drawing.Size(165, 29);
            this.btnFinal.TabIndex = 1;
            this.btnFinal.Text = "Finalize";
            this.btnFinal.UseVisualStyleBackColor = true;
            this.btnFinal.Click += new System.EventHandler(this.btnFinal_Click);
            // 
            // gr_GrabState
            // 
            this.gr_GrabState.Controls.Add(this.btn_GrabStatus);
            this.gr_GrabState.Controls.Add(this.rdo_Grab_OFF);
            this.gr_GrabState.Controls.Add(this.rdo_Grab_ON);
            this.gr_GrabState.Location = new System.Drawing.Point(391, 11);
            this.gr_GrabState.Margin = new System.Windows.Forms.Padding(2);
            this.gr_GrabState.Name = "gr_GrabState";
            this.gr_GrabState.Padding = new System.Windows.Forms.Padding(2);
            this.gr_GrabState.Size = new System.Drawing.Size(122, 180);
            this.gr_GrabState.TabIndex = 1;
            this.gr_GrabState.TabStop = false;
            this.gr_GrabState.Text = "Grab Status";
            // 
            // btn_GrabStatus
            // 
            this.btn_GrabStatus.Enabled = false;
            this.btn_GrabStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_GrabStatus.Location = new System.Drawing.Point(5, 23);
            this.btn_GrabStatus.Name = "btn_GrabStatus";
            this.btn_GrabStatus.Size = new System.Drawing.Size(106, 60);
            this.btn_GrabStatus.TabIndex = 0;
            this.btn_GrabStatus.Text = "IDLE";
            this.btn_GrabStatus.UseVisualStyleBackColor = true;
            this.btn_GrabStatus.Click += new System.EventHandler(this.btn_ExposureTime_Set_Click);
            // 
            // rdo_Grab_OFF
            // 
            this.rdo_Grab_OFF.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdo_Grab_OFF.Location = new System.Drawing.Point(5, 132);
            this.rdo_Grab_OFF.Margin = new System.Windows.Forms.Padding(2);
            this.rdo_Grab_OFF.Name = "rdo_Grab_OFF";
            this.rdo_Grab_OFF.Size = new System.Drawing.Size(106, 37);
            this.rdo_Grab_OFF.TabIndex = 0;
            this.rdo_Grab_OFF.TabStop = true;
            this.rdo_Grab_OFF.Text = "OFF";
            this.rdo_Grab_OFF.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdo_Grab_OFF.UseVisualStyleBackColor = true;
            this.rdo_Grab_OFF.Click += new System.EventHandler(this.rdo_Grab_Click);
            // 
            // rdo_Grab_ON
            // 
            this.rdo_Grab_ON.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdo_Grab_ON.Location = new System.Drawing.Point(5, 88);
            this.rdo_Grab_ON.Margin = new System.Windows.Forms.Padding(2);
            this.rdo_Grab_ON.Name = "rdo_Grab_ON";
            this.rdo_Grab_ON.Size = new System.Drawing.Size(106, 37);
            this.rdo_Grab_ON.TabIndex = 0;
            this.rdo_Grab_ON.TabStop = true;
            this.rdo_Grab_ON.Text = "ON";
            this.rdo_Grab_ON.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdo_Grab_ON.UseVisualStyleBackColor = true;
            this.rdo_Grab_ON.Click += new System.EventHandler(this.rdo_Grab_Click);
            // 
            // gr_TriggerMode
            // 
            this.gr_TriggerMode.Controls.Add(this.rdo_TrigeerMode_External);
            this.gr_TriggerMode.Controls.Add(this.rdo_TrigeerMode_Software);
            this.gr_TriggerMode.Controls.Add(this.rdo_TrigeerMode_FreeRun);
            this.gr_TriggerMode.Location = new System.Drawing.Point(265, 73);
            this.gr_TriggerMode.Margin = new System.Windows.Forms.Padding(2);
            this.gr_TriggerMode.Name = "gr_TriggerMode";
            this.gr_TriggerMode.Padding = new System.Windows.Forms.Padding(2);
            this.gr_TriggerMode.Size = new System.Drawing.Size(122, 118);
            this.gr_TriggerMode.TabIndex = 1;
            this.gr_TriggerMode.TabStop = false;
            this.gr_TriggerMode.Text = "Trigger mode";
            // 
            // rdo_TrigeerMode_External
            // 
            this.rdo_TrigeerMode_External.AutoSize = true;
            this.rdo_TrigeerMode_External.Location = new System.Drawing.Point(20, 79);
            this.rdo_TrigeerMode_External.Margin = new System.Windows.Forms.Padding(2);
            this.rdo_TrigeerMode_External.Name = "rdo_TrigeerMode_External";
            this.rdo_TrigeerMode_External.Size = new System.Drawing.Size(81, 19);
            this.rdo_TrigeerMode_External.TabIndex = 0;
            this.rdo_TrigeerMode_External.TabStop = true;
            this.rdo_TrigeerMode_External.Text = "External";
            this.rdo_TrigeerMode_External.UseVisualStyleBackColor = true;
            this.rdo_TrigeerMode_External.CheckedChanged += new System.EventHandler(this.rdo_TrigeerMode_CheckedChanged);
            // 
            // rdo_TrigeerMode_Software
            // 
            this.rdo_TrigeerMode_Software.AutoSize = true;
            this.rdo_TrigeerMode_Software.Location = new System.Drawing.Point(20, 51);
            this.rdo_TrigeerMode_Software.Margin = new System.Windows.Forms.Padding(2);
            this.rdo_TrigeerMode_Software.Name = "rdo_TrigeerMode_Software";
            this.rdo_TrigeerMode_Software.Size = new System.Drawing.Size(81, 19);
            this.rdo_TrigeerMode_Software.TabIndex = 0;
            this.rdo_TrigeerMode_Software.TabStop = true;
            this.rdo_TrigeerMode_Software.Text = "Software";
            this.rdo_TrigeerMode_Software.UseVisualStyleBackColor = true;
            this.rdo_TrigeerMode_Software.CheckedChanged += new System.EventHandler(this.rdo_TrigeerMode_CheckedChanged);
            // 
            // rdo_TrigeerMode_FreeRun
            // 
            this.rdo_TrigeerMode_FreeRun.AutoSize = true;
            this.rdo_TrigeerMode_FreeRun.Location = new System.Drawing.Point(20, 23);
            this.rdo_TrigeerMode_FreeRun.Margin = new System.Windows.Forms.Padding(2);
            this.rdo_TrigeerMode_FreeRun.Name = "rdo_TrigeerMode_FreeRun";
            this.rdo_TrigeerMode_FreeRun.Size = new System.Drawing.Size(81, 19);
            this.rdo_TrigeerMode_FreeRun.TabIndex = 0;
            this.rdo_TrigeerMode_FreeRun.TabStop = true;
            this.rdo_TrigeerMode_FreeRun.Text = "Free-run";
            this.rdo_TrigeerMode_FreeRun.UseVisualStyleBackColor = true;
            this.rdo_TrigeerMode_FreeRun.CheckedChanged += new System.EventHandler(this.rdo_TrigeerMode_CheckedChanged);
            // 
            // gr_AcquisitionControl
            // 
            this.gr_AcquisitionControl.Controls.Add(this.cb_EnableCameraFrameCallBack);
            this.gr_AcquisitionControl.Controls.Add(this.btn_SwTrigger);
            this.gr_AcquisitionControl.Location = new System.Drawing.Point(16, 71);
            this.gr_AcquisitionControl.Margin = new System.Windows.Forms.Padding(2);
            this.gr_AcquisitionControl.Name = "gr_AcquisitionControl";
            this.gr_AcquisitionControl.Padding = new System.Windows.Forms.Padding(2);
            this.gr_AcquisitionControl.Size = new System.Drawing.Size(239, 120);
            this.gr_AcquisitionControl.TabIndex = 1;
            this.gr_AcquisitionControl.TabStop = false;
            this.gr_AcquisitionControl.Text = "Acquisition Contorol";
            // 
            // cb_EnableCameraFrameCallBack
            // 
            this.cb_EnableCameraFrameCallBack.AutoSize = true;
            this.cb_EnableCameraFrameCallBack.Location = new System.Drawing.Point(12, 25);
            this.cb_EnableCameraFrameCallBack.Name = "cb_EnableCameraFrameCallBack";
            this.cb_EnableCameraFrameCallBack.Size = new System.Drawing.Size(222, 19);
            this.cb_EnableCameraFrameCallBack.TabIndex = 1;
            this.cb_EnableCameraFrameCallBack.Text = "Enable camera frame callback";
            this.cb_EnableCameraFrameCallBack.UseVisualStyleBackColor = true;
            this.cb_EnableCameraFrameCallBack.CheckedChanged += new System.EventHandler(this.cb_EnableCameraFrameCallBack_CheckedChanged);
            // 
            // btn_SwTrigger
            // 
            this.btn_SwTrigger.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_SwTrigger.Location = new System.Drawing.Point(12, 59);
            this.btn_SwTrigger.Name = "btn_SwTrigger";
            this.btn_SwTrigger.Size = new System.Drawing.Size(213, 43);
            this.btn_SwTrigger.TabIndex = 0;
            this.btn_SwTrigger.Text = "SW Trigger";
            this.btn_SwTrigger.UseVisualStyleBackColor = true;
            this.btn_SwTrigger.Click += new System.EventHandler(this.btn_SwTrigger_Click);
            // 
            // txt_Log
            // 
            this.txt_Log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_Log.Location = new System.Drawing.Point(16, 317);
            this.txt_Log.Margin = new System.Windows.Forms.Padding(2);
            this.txt_Log.Name = "txt_Log";
            this.txt_Log.Size = new System.Drawing.Size(579, 353);
            this.txt_Log.TabIndex = 0;
            this.txt_Log.Text = "";
            // 
            // pb_CamImage
            // 
            this.pb_CamImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_CamImage.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pb_CamImage.Location = new System.Drawing.Point(3, 34);
            this.pb_CamImage.Margin = new System.Windows.Forms.Padding(2);
            this.pb_CamImage.Name = "pb_CamImage";
            this.pb_CamImage.Size = new System.Drawing.Size(650, 647);
            this.pb_CamImage.TabIndex = 1;
            this.pb_CamImage.TabStop = false;
            // 
            // lbl_ImageStatus
            // 
            this.lbl_ImageStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_ImageStatus.Location = new System.Drawing.Point(3, 0);
            this.lbl_ImageStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_ImageStatus.Name = "lbl_ImageStatus";
            this.lbl_ImageStatus.Size = new System.Drawing.Size(650, 32);
            this.lbl_ImageStatus.TabIndex = 0;
            this.lbl_ImageStatus.Text = "Text";
            this.lbl_ImageStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmr_LogUpdate
            // 
            this.tmr_LogUpdate.Tick += new System.EventHandler(this.tmr_LogUpdate_Tick);
            // 
            // tmr_GrabStatus
            // 
            this.tmr_GrabStatus.Tick += new System.EventHandler(this.tmr_GrabStatus_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.splitContainer);
            this.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.Text = "SDOAQ Camera Frame Callback Sample";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.gr_Image.ResumeLayout(false);
            this.gr_Image.PerformLayout();
            this.gr_FOV.ResumeLayout(false);
            this.gr_FOV.PerformLayout();
            this.gr_Init.ResumeLayout(false);
            this.gr_GrabState.ResumeLayout(false);
            this.gr_TriggerMode.ResumeLayout(false);
            this.gr_TriggerMode.PerformLayout();
            this.gr_AcquisitionControl.ResumeLayout(false);
            this.gr_AcquisitionControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_CamImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Label lbl_ImageStatus;
        private System.Windows.Forms.PictureBox pb_CamImage;
        private System.Windows.Forms.GroupBox gr_AcquisitionControl;
        private System.Windows.Forms.RichTextBox txt_Log;
        private System.Windows.Forms.GroupBox gr_TriggerMode;
        private System.Windows.Forms.RadioButton rdo_TrigeerMode_FreeRun;
        private System.Windows.Forms.CheckBox cb_EnableCameraFrameCallBack;
        private System.Windows.Forms.Button btn_SwTrigger;
        private System.Windows.Forms.RadioButton rdo_TrigeerMode_External;
        private System.Windows.Forms.RadioButton rdo_TrigeerMode_Software;
        private System.Windows.Forms.GroupBox gr_Init;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.Button btnFinal;
        private System.Windows.Forms.Timer tmr_LogUpdate;
        private System.Windows.Forms.GroupBox gr_FOV;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_FOV_Height;
        private System.Windows.Forms.TextBox txt_FOV_Width;
        private System.Windows.Forms.Button btn_FOV_Set;
        private System.Windows.Forms.GroupBox gr_Image;
        private System.Windows.Forms.TextBox txt_ExposureTime;
        private System.Windows.Forms.Button btn_ExposureTime_Set;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_Gain;
        private System.Windows.Forms.Button btn_Gain_Set;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gr_GrabState;
        private System.Windows.Forms.Button btn_GrabStatus;
        private System.Windows.Forms.RadioButton rdo_Grab_OFF;
        private System.Windows.Forms.RadioButton rdo_Grab_ON;
        private System.Windows.Forms.Timer tmr_GrabStatus;
    }
}

