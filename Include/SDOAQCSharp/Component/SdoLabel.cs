using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SDOAQCSharp.Tool;

namespace SDOAQCSharp.Component
{
    public partial class SdoLabel : Label
    {
        [Category(""), Description("Border Color")]
        private Color _borderColor = Color.Transparent;
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                if (_borderColor == value) return;

                _borderColor = value;
                Invalidate();
            }
        }

        [Category(""), Description("Border Width")]
        private int _borderWidth = 0;
        public int BorderWidth
        {
            get => _borderWidth;
            set
            {
                if (_borderWidth == value) return;

                _borderWidth = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category(""), Description("Auto Font Size Adjust")]
        private bool _autoFontSizeAdjust = false;
        public bool AutoFontSizeAdjust
        {
            get => _autoFontSizeAdjust;
            set
            {
                if (_autoFontSizeAdjust == value) return;
                _autoFontSizeAdjust = value;
                Invalidate();
            }
        }

        private const int AUTO_FONT_SIZE_MAX = 1000;
        private const int AUTO_FONT_SIZE_MIN = 8;
        private const int AUTO_FONT_SIZE_MARGIN = 5;
        public SdoLabel()
        {
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);

            this.Paint += Control_Paint;
        }

        private float GetFontSize(string text, int margin, float minimumSize, float maximumSize)
        {
            if (text.Length == 0)
            {
                return minimumSize;
            }

            int width = this.DisplayRectangle.Width - margin;
            int height = this.DisplayRectangle.Height - margin;

            using (Graphics graphics = this.CreateGraphics())
            {
                while (maximumSize - minimumSize > 0.1f)
                {
                    float halfSize = (minimumSize + maximumSize) / 2f;

                    using (Font font = new Font(this.Font.FontFamily, halfSize))
                    {
                        SizeF textSize = graphics.MeasureString(text, font);

                        if ((textSize.Width > width) || (textSize.Height > height))
                        {
                            maximumSize = halfSize;
                        }
                        else
                        {
                            minimumSize = halfSize;
                        }
                    }
                }
                return minimumSize;
            }
        }

        private void ApplyAutoFontSize()
        {
            float fontSize = GetFontSize(this.Text, 
                AUTO_FONT_SIZE_MARGIN, 
                AUTO_FONT_SIZE_MIN, 
                AUTO_FONT_SIZE_MAX);

            if (!this.Font.Size.Equal(fontSize))
            {
                var newFont = new Font(this.Font.FontFamily, fontSize);
                Font?.Dispose();
                Font = newFont;
            }
        }

        private void Control_Paint(object sender, PaintEventArgs e)
        {
            if (DesignMode == false && AutoFontSizeAdjust)
            {
                ApplyAutoFontSize();
            }

            if (BorderStyle == BorderStyle.None)
            {
                ControlPaint.DrawBorder(e.Graphics, new Rectangle(0, 0, this.Width, this.Height)
                                    , BorderColor, BorderWidth, ButtonBorderStyle.Solid
                                    , BorderColor, BorderWidth, ButtonBorderStyle.Solid
                                    , BorderColor, BorderWidth, ButtonBorderStyle.Solid
                                    , BorderColor, BorderWidth, ButtonBorderStyle.Solid);
            }
        }
    }
}
