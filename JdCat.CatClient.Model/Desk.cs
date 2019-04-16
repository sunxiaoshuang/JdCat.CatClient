using JdCat.CatClient.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Model
{
    /// <summary>
    /// 餐桌
    /// </summary>
    public partial class Desk : ClientBaseEntity, INotifyPropertyChanged
    {
        private string _name;
        /// <summary>
        /// 餐桌名称
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
        private int _quantity;
        /// <summary>
        /// 餐桌指定人数
        /// </summary>
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Quantity"));
            }
        }
        /// <summary>
        /// 所属的商户id
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 所属的餐桌类别
        /// </summary>
        public int DeskTypeId { get; set; }

        private TangOrder _order;
        /// <summary>
        /// 餐桌当前正在使用的订单
        /// </summary>
        [JsonIgnore]
        public TangOrder Order
        {
            get { return _order; }
            set
            {
                _order = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Order"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
