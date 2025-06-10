using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdoaqMultiFocus
{
    public enum emMultiFocusFunc
    {
        AUTO_FOCUS = 1,
        FIXED_FOCUS = 2,
    }

    public class MultiFocusItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// unique number
        /// </summary>
        private int _Id;
        public int Id
        {
            get => _Id;
            set
            {
                _Id = value;
                OnPropertyChanged("Id");
            }
        }
        /// <summary>
        /// 1: auto-focus, 2: fixed-focus
        /// </summary>
        private emMultiFocusFunc _Func;
        
        public emMultiFocusFunc Func
        {
            get => _Func;
            set
            {
                _Func = value;
                OnPropertyChanged("Func");
            }
        }
        /// <summary>
        /// focus step for fixed-focus roi
        /// </summary>
        private int _Focus;
        public int Focus
        {
            get => _Focus;
            set
            {
                _Focus = value;
                OnPropertyChanged("Focus");
            }
        }
        /// <summary>
        /// multi-foucs roi
        /// </summary>
        private Rectangle _Rect;
        public Rectangle Rect
        {
            get => _Rect;
            set
            {
                _Rect = value;
                OnPropertyChanged("Rect");
            }
        }


        public override string ToString()
        {
            return $"{Id},{(int)Func},{Focus},{Rect.Left},{Rect.Top},{Rect.Right},{Rect.Bottom}";
        }

        public void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetParam_Rect()
        {
            return $"{Rect.Left},{Rect.Top},{Rect.Width},{Rect.Height}";
        }

        public string GetParam_ScriptFormat(int idx)
        {
            return $"MR MULTI {idx} = {{ {this.ToString()} }}";
        }

        public void SetParam(MultiFocusItem item)
        {
            this.Id = item.Id;
            this.Func = item.Func;
            this.Focus = item.Focus;
            this.Rect = item.Rect;
        }
        public static MultiFocusItem GetDummy()
        {
            return new MultiFocusItem()
            {
                Id = 1,
                Func = emMultiFocusFunc.AUTO_FOCUS,
                Focus = 160,
                Rect = new Rectangle(1700, 700, 256, 256)
            };
        }

    }
}
