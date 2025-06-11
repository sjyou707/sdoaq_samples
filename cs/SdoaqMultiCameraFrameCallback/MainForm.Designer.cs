namespace SdoaqMultiCameraFrameCallback
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
            this.txt_Log = new System.Windows.Forms.RichTextBox();
            this.tab_CamPage = new System.Windows.Forms.TabControl();
            this.tmr_LogUpdate = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.txt_Log);
            this.splitContainer.Panel1MinSize = 20;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tab_CamPage);
            this.splitContainer.Size = new System.Drawing.Size(924, 681);
            this.splitContainer.SplitterDistance = 167;
            this.splitContainer.SplitterWidth = 3;
            this.splitContainer.TabIndex = 0;
            this.splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
            // 
            // txt_Log
            // 
            this.txt_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_Log.Location = new System.Drawing.Point(0, 0);
            this.txt_Log.Margin = new System.Windows.Forms.Padding(2);
            this.txt_Log.Name = "txt_Log";
            this.txt_Log.Size = new System.Drawing.Size(924, 167);
            this.txt_Log.TabIndex = 0;
            this.txt_Log.Text = "";
            // 
            // tab_CamPage
            // 
            this.tab_CamPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tab_CamPage.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tab_CamPage.ItemSize = new System.Drawing.Size(200, 40);
            this.tab_CamPage.Location = new System.Drawing.Point(0, 0);
            this.tab_CamPage.Name = "tab_CamPage";
            this.tab_CamPage.SelectedIndex = 0;
            this.tab_CamPage.Size = new System.Drawing.Size(924, 511);
            this.tab_CamPage.TabIndex = 0;
            this.tab_CamPage.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tab_CamPage_DrawItem);
            // 
            // tmr_LogUpdate
            // 
            this.tmr_LogUpdate.Tick += new System.EventHandler(this.tmr_LogUpdate_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 681);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.RichTextBox txt_Log;
        private System.Windows.Forms.Timer tmr_LogUpdate;
        private System.Windows.Forms.TabControl tab_CamPage;
    }
}

