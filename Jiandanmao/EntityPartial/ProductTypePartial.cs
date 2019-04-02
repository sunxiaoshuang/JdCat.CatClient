using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jiandanmao.Entity
{
    public partial class ProductType : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
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
    }
}
