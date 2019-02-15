using Jiandanmao.Code;
using Jiandanmao.Extension;
using Jiandanmao.Uc;
using Jiandanmao.ViewModel;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Jiandanmao.Model
{
    public partial class ProductType : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _isCheck;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsCheck
        {
            get { return _isCheck; }
            set
            {
                _isCheck = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsCheck"));
            }
        }
        public string CheckedColor { get; set; } = "#e64a19";
    }
}
