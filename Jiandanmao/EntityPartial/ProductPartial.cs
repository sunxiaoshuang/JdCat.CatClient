using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Jiandanmao.Entity
{
    public partial class Product : INotifyPropertyChanged, ICloneable
    {
        private bool _isCheck;
        /// <summary>
        /// 是否选中
        /// </summary>
        [JsonIgnore]
        public bool IsCheck
        {
            get { return _isCheck; }
            set
            {
                _isCheck = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsCheck"));
            }
        }
        private double _selectedQuantity;
        /// <summary>
        /// 已选择的数量
        /// </summary>
        public double SelectedQuantity
        {
            get { return _selectedQuantity; }
            set
            {
                _selectedQuantity = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedQuantity"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
