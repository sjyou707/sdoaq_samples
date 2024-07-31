namespace SdoaqAutoFocus
{
    partial class SdoaqAutoFocus
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
            this.txt_Log = new System.Windows.Forms.RichTextBox();
            this.gpb_Controls = new System.Windows.Forms.GroupBox();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btn_SetROI = new System.Windows.Forms.Button();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tmr_LogUpdate = new System.Windows.Forms.Timer(this.components);
            this.pnl_Viewer = new SDOAQCSharp.Component.SdoPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.gpb_Controls.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.txt_Log);
            this.splitContainer.Panel1.Controls.Add(this.gpb_Controls);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.pnl_Viewer);
            this.splitContainer.Size = new System.Drawing.Size(1202, 522);
            this.splitContainer.SplitterDistance = 642;
            this.splitContainer.TabIndex = 0;
            // 
            // txt_Log
            // 
            this.txt_Log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_Log.Location = new System.Drawing.Point(14, 294);
            this.txt_Log.Margin = new System.Windows.Forms.Padding(2);
            this.txt_Log.Name = "txt_Log";
            this.txt_Log.Size = new System.Drawing.Size(613, 202);
            this.txt_Log.TabIndex = 1;
            this.txt_Log.Text = "";
            // 
            // gpb_Controls
            // 
            this.gpb_Controls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpb_Controls.Controls.Add(this.button8);
            this.gpb_Controls.Controls.Add(this.button7);
            this.gpb_Controls.Controls.Add(this.button6);
            this.gpb_Controls.Controls.Add(this.button5);
            this.gpb_Controls.Controls.Add(this.button4);
            this.gpb_Controls.Controls.Add(this.button3);
            this.gpb_Controls.Controls.Add(this.button2);
            this.gpb_Controls.Controls.Add(this.btn_SetROI);
            this.gpb_Controls.Controls.Add(this.textBox5);
            this.gpb_Controls.Controls.Add(this.textBox4);
            this.gpb_Controls.Controls.Add(this.textBox3);
            this.gpb_Controls.Controls.Add(this.textBox2);
            this.gpb_Controls.Controls.Add(this.textBox1);
            this.gpb_Controls.Controls.Add(this.label5);
            this.gpb_Controls.Controls.Add(this.label4);
            this.gpb_Controls.Controls.Add(this.label3);
            this.gpb_Controls.Controls.Add(this.label2);
            this.gpb_Controls.Controls.Add(this.label1);
            this.gpb_Controls.Location = new System.Drawing.Point(14, 15);
            this.gpb_Controls.Name = "gpb_Controls";
            this.gpb_Controls.Size = new System.Drawing.Size(613, 258);
            this.gpb_Controls.TabIndex = 0;
            this.gpb_Controls.TabStop = false;
            this.gpb_Controls.Text = "Controls";
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(459, 209);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(145, 36);
            this.button8.TabIndex = 18;
            this.button8.Text = "Stop AF";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(234, 209);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(145, 36);
            this.button7.TabIndex = 17;
            this.button7.Text = "Play AF";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(9, 209);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(145, 36);
            this.button6.TabIndex = 16;
            this.button6.Text = "SingleShot AF";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(541, 171);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(52, 25);
            this.button5.TabIndex = 15;
            this.button5.Text = "Set";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(541, 134);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(52, 25);
            this.button4.TabIndex = 14;
            this.button4.Text = "Set";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(541, 97);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(52, 25);
            this.button3.TabIndex = 13;
            this.button3.Text = "Set";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(541, 60);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(52, 25);
            this.button2.TabIndex = 12;
            this.button2.Text = "Set";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // btn_SetROI
            // 
            this.btn_SetROI.Location = new System.Drawing.Point(541, 23);
            this.btn_SetROI.Name = "btn_SetROI";
            this.btn_SetROI.Size = new System.Drawing.Size(52, 25);
            this.btn_SetROI.TabIndex = 11;
            this.btn_SetROI.Text = "Set";
            this.btn_SetROI.UseVisualStyleBackColor = true;
            this.btn_SetROI.Click += new System.EventHandler(this.btn_SetROI_Click);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(261, 172);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(274, 23);
            this.textBox5.TabIndex = 10;
            this.textBox5.Text = "4";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(261, 135);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(274, 23);
            this.textBox4.TabIndex = 9;
            this.textBox4.Text = "1";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(261, 98);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(274, 23);
            this.textBox3.TabIndex = 8;
            this.textBox3.Text = "1";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(261, 61);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(274, 23);
            this.textBox2.TabIndex = 7;
            this.textBox2.Text = "0";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(261, 24);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(274, 23);
            this.textBox1.TabIndex = 6;
            this.textBox1.Text = "1020,543,128,128";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 176);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(217, 15);
            this.label5.TabIndex = 5;
            this.label5.Text = "Stability Debounce Count(0~10)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 139);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(154, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Stability Method(1~4)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(168, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "Resampling Method (0~2)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(210, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Sharpness Measure Method(0~2)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(217, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "AF ROI (left,top,width,height)";
            // 
            // tmr_LogUpdate
            // 
            this.tmr_LogUpdate.Tick += new System.EventHandler(this.tmr_LogUpdate_Tick);
            // 
            // pnl_Viewer
            // 
            this.pnl_Viewer.BorderColor = System.Drawing.Color.Black;
            this.pnl_Viewer.BorderWidth = 1;
            this.pnl_Viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_Viewer.Location = new System.Drawing.Point(0, 0);
            this.pnl_Viewer.Name = "pnl_Viewer";
            this.pnl_Viewer.Size = new System.Drawing.Size(556, 522);
            this.pnl_Viewer.TabIndex = 2;
            // 
            // SdoaqAutoFocus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1202, 522);
            this.Controls.Add(this.splitContainer);
            this.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.Name = "SdoaqAutoFocus";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.SdoaqAutoFocus_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.gpb_Controls.ResumeLayout(false);
            this.gpb_Controls.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.GroupBox gpb_Controls;
        private System.Windows.Forms.RichTextBox txt_Log;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btn_SetROI;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer tmr_LogUpdate;
        private SDOAQCSharp.Component.SdoPanel pnl_Viewer;
    }
}

