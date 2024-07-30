using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using SDOAQCSharp.Tool;
using SDOWSIO;

namespace SDOAQCSharp.Component
{
    public partial class SdoaqImageViewr : UserControl
    {
        private readonly bool _visiblePointCloud;
        public bool VisiblePointCloud => _visiblePointCloud;

        private MySdoaq _sdoaqObj;

        private List<SdoaqImageInfo> _imageList = new List<SdoaqImageInfo>();
        private SdoaqPointCloudInfo _pointCloudInfoInfo = null;

        private int _lastSelectImageIndex = 0;

        private int _stackImgCount = 0;
        private int _afImgCount = 0;
        private int _edofImgCount = 0;
 
        public SdoaqImageViewr(bool visiblePointCloud)
        {
            InitializeComponent();

            _visiblePointCloud = visiblePointCloud;

            this.Resize += UserControl_Resize;
            this.Disposed += UserControl_Disposed;
            this.listbox_ImageList.SelectedIndexChanged += listbox_SelectedIndexChanged;
            this.Load += UserControl_Load;
        }

        public void Set_SdoaqObj(MySdoaq sdoaqObj)
        {
            if (_sdoaqObj != null)
            {
                _sdoaqObj.CallBackMsgLoop -= SdoaqObj_CallBackMsgLoop;
            }
            _sdoaqObj = sdoaqObj;
            _sdoaqObj.CallBackMsgLoop += SdoaqObj_CallBackMsgLoop;
        }
        
        private void SdoaqObj_CallBackMsgLoop((MySdoaq.CallBackMessage msg, object[] objs) callBackMsg)
        {
            switch (callBackMsg.msg)
            {
                case MySdoaq.CallBackMessage.FocusStack:
                    {
                        _stackImgCount++;
                        _afImgCount = 0;
                        _edofImgCount = 0;

                         _imageList = (List<SdoaqImageInfo>)callBackMsg.objs[0];

                        this.Invoke(() => UpdatgeImageList($"Focus Stack {_stackImgCount}"));
                    }
                    break;
                case MySdoaq.CallBackMessage.Af:
                    {
                        _stackImgCount = 0;
                        _afImgCount++;
                        _edofImgCount = 0;

                        _imageList = (List<SdoaqImageInfo>)callBackMsg.objs[0];

                        this.Invoke(() => UpdatgeImageList($"AF {_afImgCount}"));
                    }
                    break;
                case MySdoaq.CallBackMessage.Edof:
                    {
                        _stackImgCount = 0;
                        _afImgCount = 0;
                        _edofImgCount++;

                        _imageList = (List<SdoaqImageInfo>)callBackMsg.objs[0];
                        _pointCloudInfoInfo = (SdoaqPointCloudInfo)callBackMsg.objs[1];

                        this.Invoke(() =>
                        {
                            UpdatgeImageList($"Edof {_edofImgCount}");
                            UpdatePointCloud();
                        });
                    }
                    break;
            }
        }

        private bool CompareImageList(List<SdoaqImageInfo> imageList, ListBox listBox)
        {
            if (imageList.Count != listBox.Items.Count)
            {
                return false;
            }

            for (int i = 0; i < imageList.Count; i++)
            {
                if (imageList[i].Name != listBox.Items[i].ToString())
                {
                    return false;
                }
            }
            return true;
        }
        private void UpdatgeImageList(string labelText)
        {
            var listBox = listbox_ImageList;
            
            if (CompareImageList(_imageList, listBox) == false)
            {
                listBox.Items.Clear();

                foreach (var imgInfo in _imageList)
                {
                    listBox.Items.Add(imgInfo.Name);
                }

                if (listBox.Items.Count <= _lastSelectImageIndex)
                {
                    if (listBox.Items.Count > 0)
                    {
                        _lastSelectImageIndex = listBox.Items.Count - 1;
                    }
                    else
                    {
                        _lastSelectImageIndex = -1;
                    }
                }
            }
            

            if (_lastSelectImageIndex >= 0)
            {
                if (listBox.SelectedIndex == -1)
                {
                    listBox.SelectedIndex = _lastSelectImageIndex;
                }
                else
                {
                    UpdateImage(_lastSelectImageIndex);
                }
            }
            
            lbl_ImageViewer.Text = labelText;
        }

        private void UpdatePointCloud()
        {
            var hwnd3DViewer = (IntPtr)pb_PointCloudViewer.Tag;

            if (hwnd3DViewer == null)
            {
                return;
            }

            if (_visiblePointCloud == false || _pointCloudInfoInfo == null)
            {
                WSIO.GL.WSGL_Display_BG(hwnd3DViewer);
                return;
            }
            
            var paraDisplay25D = new WSIO.GL.tPara_Display25D[]
            {
                new WSIO.GL.tPara_Display25D()
                {
                     width = (uint)_pointCloudInfoInfo.Width,
                     height= (uint)_pointCloudInfoInfo.Height,
                     z_offset1 = 0,
                     z_offset2 = 0,
                     z_slices = (uint)_pointCloudInfoInfo.SliceCount,
                     scx1 = 0,
                     scx2 = 0,
                     scy1 = 0,
                     scy2 = 0,
                     scz1 = 0,
                     scz2 = 0,
                }
            };

            int displyMode = (int)(WSIO.GL.EDisplayMode.EDM_BGR_BYTE 
                | WSIO.GL.EDisplayMode.EDM_DIMENSION_CALXY_25D 
                | WSIO.GL.EDisplayMode.EDM_NDC_XY_ONLY);

            uint imageSize = (uint)(_pointCloudInfoInfo.Width * _pointCloudInfoInfo.Height);

            WSIO.GL.WSGL_Display_25D(hwnd3DViewer, WSIO.GL.GL_MG_ONSTAGE, "main",
                _pointCloudInfoInfo.VertexDataBuffer, imageSize * 3,
                _pointCloudInfoInfo.ImgDataBuffer, imageSize,
                displyMode, 1.0f, paraDisplay25D);
        }

        private void UpdateImage(int idx)
        {
            if (_imageList.Count > idx)
            {
                var img = _imageList[idx];

                WSIO.UTIL.WSUT_IV_AttachRawImgData((IntPtr)pb_ImageViewer.Tag, (uint)img.Width, (uint)img.Height,
                          (uint)(img.Width * img.ColorByte),
                          (uint)img.ColorByte, img.Data,
                          (uint)img.Data.Length);

            }
        }

        private void LayouyUpdate()
        {
            if (_visiblePointCloud)
            {
                var rectList = this.ClientRectangle.DivideRect_Col(2);

                pnl_ImageViewerGroup.SetBounds(rectList[0]);
                pnl_PointCloudViewerGroup.SetBounds(rectList[1]);
                
                pnl_ImageViewerGroup.Visible = true;
            }
            else
            {
                pnl_ImageViewerGroup.SetBounds(this.ClientRectangle);

                pnl_PointCloudViewerGroup.Visible = false;
            }
            
            var rectImageViewer = new Rectangle(0, 
                lbl_ImageViewer.Bottom,
                pnl_ImageViewerGroup.Width - listbox_ImageList.Width,
                pnl_ImageViewerGroup.Height - lbl_ImageViewer.Height);

            pb_ImageViewer.SetBounds(rectImageViewer);

            var rectPointCloudViewer = new Rectangle(0, 
                lbl_PointCloudViewer.Bottom,
                pnl_PointCloudViewerGroup.Width,
                pnl_PointCloudViewerGroup.Height - lbl_PointCloudViewer.Height);

            pb_PointCloudViewer.SetBounds(rectPointCloudViewer);

            if (pb_ImageViewer.Tag != null)
            {
                var ctrl = pb_ImageViewer;
                var hwnd = (IntPtr)pb_ImageViewer.Tag;

                WSIO.UTIL.WSUT_IV_ShowWindow(hwnd, 1, ctrl.Left, ctrl.Top - 35, ctrl.Right, ctrl.Bottom - 35);
            }

            if (pb_PointCloudViewer.Tag != null)
            {
                var ctrl = pb_PointCloudViewer;
                var hwnd = (IntPtr)pb_PointCloudViewer.Tag;
                
                WSIO.GL.WSGL_ShowWindow(hwnd, _visiblePointCloud, ctrl.Left, ctrl.Top - 35, ctrl.Right, ctrl.Bottom - 35);
            }
        }

        private void UserControl_Load(object sender, EventArgs e)
        {
            if (DesignMode)
            {
                return;
            }

            var attributes_ImageViewer = WSIO.UTIL.WSUTIVOPMODE.WSUTIVOPMODE_VISION
                                       | WSIO.UTIL.WSUTIVOPMODE.WSUTIVOPMODE_INFOOSD;

            var rvWsio = WSIO.UTIL.WSUT_IV_CreateImageViewer("Image Viewer"
                    , pb_ImageViewer.Handle
                    , out IntPtr hwndImageViewr
                    , 0
                    , attributes_ImageViewer);

            MySdoaq.WriteLog(Logger.emLogLevel.API, $"WSUT_IV_CreateImageViewer(), rv = {rvWsio}");

            pb_ImageViewer.Tag = hwndImageViewr;


            if (_visiblePointCloud)
            {
                rvWsio = WSIO.GL.WSGL_Initialize(pb_PointCloudViewer.Handle, out IntPtr hwnd3DViewer);

                MySdoaq.WriteLog(Logger.emLogLevel.API, $"WSGL_Initialize(), rv = {rvWsio}");

                var attributes_3D = WSIO.GL.EDisplayAttributes.EDA_SHOW_GUIDER_OBJECTS
                                  | WSIO.GL.EDisplayAttributes.EDA_SHOW_SCALE_OBJECTS
                                  | WSIO.GL.EDisplayAttributes.EDA_SHOW_COLORMAPBAR_OBJECTS
                                  | WSIO.GL.EDisplayAttributes.EDA_NOHIDE_PICKER;

                WSIO.GL.WSGL_SetDisplayAttributes(hwnd3DViewer, (int)attributes_3D);

                pb_PointCloudViewer.Tag = hwnd3DViewer;

                WSIO.GL.WSGL_ShowWindow(hwnd3DViewer, _visiblePointCloud, pb_PointCloudViewer.Left, pb_PointCloudViewer.Top - 35, pb_PointCloudViewer.Right, pb_PointCloudViewer.Bottom - 35);

                WSIO.GL.WSGL_Display_BG(hwnd3DViewer);
            }
        }

        private void UserControl_Disposed(object sender, EventArgs e)
        {
            if (_sdoaqObj != null)
            {
                _sdoaqObj.CallBackMsgLoop -= SdoaqObj_CallBackMsgLoop;
            }

            if (pb_ImageViewer.Tag != null)
            {
                var hwnd = (IntPtr)pb_ImageViewer.Tag;
                WSIO.UTIL.WSUT_IV_DestroyImageViewer(hwnd);

                pb_ImageViewer.Tag = null;
            }

            if (pb_PointCloudViewer.Tag != null)
            {
                var hwnd = (IntPtr)pb_PointCloudViewer.Tag;

                WSIO.GL.WSGL_Finalize(hwnd);

                pb_PointCloudViewer.Tag = null;
            }
        }
        
        private void UserControl_Resize(object sender, EventArgs e)
        {
            LayouyUpdate();
        }

        private void listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listbox_ImageList.SelectedIndex < 0)
            {
                return;
            }

            int idx = listbox_ImageList.SelectedIndex;
            
            if (_imageList.Count > idx)
            {
                _lastSelectImageIndex = idx;
                UpdateImage(idx);
            }
        }
    }
}
