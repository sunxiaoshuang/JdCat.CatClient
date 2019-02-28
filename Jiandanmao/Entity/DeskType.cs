using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jiandanmao.Entity
{
    /// <summary>
    /// 餐桌分类
    /// </summary>
    public partial class DeskType: INotifyPropertyChanged, ICloneable
    {
        public int Id { get; set; }
        private string _name;
        /// <summary>
        /// 区域名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }
        private int _sort;
        /// <summary>
        /// 排序码
        /// </summary>
        public int Sort
        {
            get { return _sort; }
            set
            {
                _sort = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Sort"));
            }
        }
        public int BusinessId { get; set; }
        public ObservableCollection<Desk> Desks { get; set; }
        private int _deskQuantity;
        /// <summary>
        /// 类别中包含的餐桌数量
        /// </summary>
        [JsonIgnore]
        public int DeskQuantity
        {
            get { return _deskQuantity; }
            set
            {
                _deskQuantity = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DeskQuantity"));
            }
        }
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
        public void ReloadDeskQuantity()
        {
            DeskQuantity = Desks == null ? 0 : Desks.Count();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
