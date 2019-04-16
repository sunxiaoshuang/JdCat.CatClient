using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model
{
    /// <summary>
    /// 餐桌分类
    /// </summary>
    public class DeskType : ClientBaseEntity, INotifyPropertyChanged
    {
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
        /// <summary>
        /// 餐台
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<Desk> Desks { get; set; }
        /// <summary>
        /// 区域中餐台数量
        /// </summary>
        public int DeskQuantity { get => Desks?.Count() ?? 0; }
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
