using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace JdCat.CatClient.Model
{
    /// <summary>
    /// 商品类别表
    /// </summary>
    public partial class ProductType : ClientBaseEntity, INotifyPropertyChanged
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 所属分类产品
        /// </summary>
        public ObservableCollection<Product> Products { get; set; }
        /// <summary>
        /// 分类所属商家id
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 分类所属商家对象
        /// </summary>
        public virtual Business Business { get; set; }

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
