using Newtonsoft.Json;
using System.ComponentModel;

namespace Jiandanmao.Entity
{
    public partial class Product : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
