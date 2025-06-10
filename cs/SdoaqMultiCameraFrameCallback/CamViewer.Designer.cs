namespace SdoaqMultiCameraFrameCallback
{
    partial class CamViewer
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnl_View = new System.Windows.Forms.Panel();
            this.lbl_ImageStatus = new System.Windows.Forms.Label();
            this.pb_CamImage = new System.Windows.Forms.PictureBox();
            this.gr_FOV = new System.Windows.Forms.GroupBox();
            this.txt_FOV_Offset_Y = new System.Windows.Forms.TextBox();
            this.txt_FOV_Height = new System.Windows.Forms.TextBox();
            this.txt_FOV_Offset_X = new System.Windows.Forms.TextBox();
            this.txt_FOV_Width = new System.Windows.Forms.TextBox();
            this.btn_FOV_Set = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_Gain = new System.Windows.Forms.TextBox();
            this.txt_ExposureTime = new System.Windows.Forms.TextBox();
            this.btn_Gain_Set = new System.Windows.Forms.Button();
            this.btn_ExposureTime_Set = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_SwTrigger = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.pnl_View.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_CamImage)).BeginInit();
            this.gr_FOV.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txt_Gain);
            this.groupBox1.Controls.Add(this.txt_ExposureTime);
            this.groupBox1.Controls.Add(this.btn_Gain_Set);
            this.groupBox1.Controls.Add(this.btn_ExposureTime_Set);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.gr_FOV);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(371, 199);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameter";
            // 
            // pnl_View
            // 
            this.pnl_View.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnl_View.Controls.Add(this.pb_CamImage);
            this.pnl_View.Controls.Add(this.lbl_ImageStatus);
            this.pnl_View.Location = new System.Drawing.Point(380, 3);
            this.pnl_View.Name = "pnl_View";
            this.pnl_View.Size = new System.Drawing.Size(308, 254);
            this.pnl_View.TabIndex = 1;
            this.pnl_View.Resize += new System.EventHandler(this.pnl_View_Resize);
            // 
            // lbl_ImageStatus
            // 
            this.lbl_ImageStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_ImageStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_ImageStatus.Location = new System.Drawing.Point(0, 0);
            this.lbl_ImageStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_ImageStatus.Name = "lbl_ImageStatus";
            this.lbl_ImageStatus.Size = new System.Drawing.Size(308, 32);
            this.lbl_ImageStatus.TabIndex = 1;
            this.lbl_ImageStatus.Text = "Text";
            this.lbl_ImageStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pb_CamImage
            // 
            this.pb_CamImage.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pb_CamImage.Location = new System.Drawing.Point(2, 34);
            this.pb_CamImage.Margin = new System.Windows.Forms.Padding(2);
            this.pb_CamImage.Name = "pb_CamImage";
            this.pb_CamImage.Size = new System.Drawing.Size(121, 73);
            this.pb_CamImage.TabIndex = 2;
            this.pb_CamImage.TabStop = false;
            // 
            // gr_FOV
            // 
            this.gr_FOV.Controls.Add(this.txt_FOV_Offset_Y);
            this.gr_FOV.Controls.Add(this.txt_FOV_Height);
            this.gr_FOV.Controls.Add(this.txt_FOV_Offset_X);
            this.gr_FOV.Controls.Add(this.txt_FOV_Width);
            this.gr_FOV.Controls.Add(this.btn_FOV_Set);
            this.gr_FOV.Controls.Add(this.label6);
            this.gr_FOV.Controls.Add(this.label2);
            this.gr_FOV.Controls.Add(this.label5);
            this.gr_FOV.Controls.Add(this.label1);
            this.gr_FOV.Location = new System.Drawing.Point(6, 22);
            this.gr_FOV.Name = "gr_FOV";
            this.gr_FOV.Size = new System.Drawing.Size(355, 100);
            this.gr_FOV.TabIndex = 5;
            this.gr_FOV.TabStop = false;
            this.gr_FOV.Text = "FOV";
            // 
            // txt_FOV_Offset_Y
            // 
            this.txt_FOV_Offset_Y.Location = new System.Drawing.Point(206, 62);
            this.txt_FOV_Offset_Y.Name = "txt_FOV_Offset_Y";
            this.txt_FOV_Offset_Y.Size = new System.Drawing.Size(69, 23);
            this.txt_FOV_Offset_Y.TabIndex = 2;
            // 
            // txt_FOV_Height
            // 
            this.txt_FOV_Height.Location = new System.Drawing.Point(54, 61);
            this.txt_FOV_Height.Name = "txt_FOV_Height";
            this.txt_FOV_Height.Size = new System.Drawing.Size(69, 23);
            this.txt_FOV_Height.TabIndex = 2;
            // 
            // txt_FOV_Offset_X
            // 
            this.txt_FOV_Offset_X.Location = new System.Drawing.Point(206, 28);
            this.txt_FOV_Offset_X.Name = "txt_FOV_Offset_X";
            this.txt_FOV_Offset_X.Size = new System.Drawing.Size(69, 23);
            this.txt_FOV_Offset_X.TabIndex = 2;
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
            this.btn_FOV_Set.Location = new System.Drawing.Point(285, 29);
            this.btn_FOV_Set.Name = "btn_FOV_Set";
            this.btn_FOV_Set.Size = new System.Drawing.Size(64, 56);
            this.btn_FOV_Set.TabIndex = 0;
            this.btn_FOV_Set.Text = "Set";
            this.btn_FOV_Set.UseVisualStyleBackColor = true;
            this.btn_FOV_Set.Click += new System.EventHandler(this.btn_FOV_Set_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(135, 62);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "Offset Y";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Height";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(137, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "Offset X";
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
            // txt_Gain
            // 
            this.txt_Gain.Location = new System.Drawing.Point(212, 167);
            this.txt_Gain.Name = "txt_Gain";
            this.txt_Gain.Size = new System.Drawing.Size(65, 23);
            this.txt_Gain.TabIndex = 10;
            // 
            // txt_ExposureTime
            // 
            this.txt_ExposureTime.Location = new System.Drawing.Point(212, 134);
            this.txt_ExposureTime.Name = "txt_ExposureTime";
            this.txt_ExposureTime.Size = new System.Drawing.Size(69, 23);
            this.txt_ExposureTime.TabIndex = 11;
            // 
            // btn_Gain_Set
            // 
            this.btn_Gain_Set.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Gain_Set.Location = new System.Drawing.Point(291, 167);
            this.btn_Gain_Set.Name = "btn_Gain_Set";
            this.btn_Gain_Set.Size = new System.Drawing.Size(64, 23);
            this.btn_Gain_Set.TabIndex = 6;
            this.btn_Gain_Set.Text = "Set";
            this.btn_Gain_Set.UseVisualStyleBackColor = true;
            this.btn_Gain_Set.Click += new System.EventHandler(this.btn_Gain_Set_Click);
            // 
            // btn_ExposureTime_Set
            // 
            this.btn_ExposureTime_Set.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ExposureTime_Set.Location = new System.Drawing.Point(291, 129);
            this.btn_ExposureTime_Set.Name = "btn_ExposureTime_Set";
            this.btn_ExposureTime_Set.Size = new System.Drawing.Size(64, 23);
            this.btn_ExposureTime_Set.TabIndex = 7;
            this.btn_ExposureTime_Set.Text = "Set";
            this.btn_ExposureTime_Set.UseVisualStyleBackColor = true;
            this.btn_ExposureTime_Set.Click += new System.EventHandler(this.btn_ExposureTime_Set_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(169, 170);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Gain";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(71, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "Exposure Time (us)";
            // 
            // btn_SwTrigger
            // 
            this.btn_SwTrigger.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_SwTrigger.Location = new System.Drawing.Point(3, 208);
            this.btn_SwTrigger.Name = "btn_SwTrigger";
            this.btn_SwTrigger.Size = new System.Drawing.Size(371, 49);
            this.btn_SwTrigger.TabIndex = 0;
            this.btn_SwTrigger.Text = "Camera SW trigger";
            this.btn_SwTrigger.UseVisualStyleBackColor = true;
            this.btn_SwTrigger.Click += new System.EventHandler(this.btn_SwTrigger_Click);
            // 
            // CamViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnl_View);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_SwTrigger);
            this.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "CamViewer";
            this.Size = new System.Drawing.Size(692, 261);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnl_View.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pb_CamImage)).EndInit();
            this.gr_FOV.ResumeLayout(false);
            this.gr_FOV.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel pnl_View;
        private System.Windows.Forms.Label lbl_ImageStatus;
        private System.Windows.Forms.PictureBox pb_CamImage;
        private System.Windows.Forms.GroupBox gr_FOV;
        private System.Windows.Forms.TextBox txt_FOV_Offset_Y;
        private System.Windows.Forms.TextBox txt_FOV_Height;
        private System.Windows.Forms.TextBox txt_FOV_Offset_X;
        private System.Windows.Forms.TextBox txt_FOV_Width;
        private System.Windows.Forms.Button btn_FOV_Set;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_Gain;
        private System.Windows.Forms.TextBox txt_ExposureTime;
        private System.Windows.Forms.Button btn_Gain_Set;
        private System.Windows.Forms.Button btn_ExposureTime_Set;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_SwTrigger;
    }
}
