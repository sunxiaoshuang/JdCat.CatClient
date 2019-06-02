using JdCat.CatClient.Model.Enum;
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
    /// 订单付款类
    /// </summary>
    [Serializable]
    public class TangOrderPayment: ClientBaseEntity, INotifyPropertyChanged
    {
        private string _name;
        /// <summary>
        /// 支付名称
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

        private double _amount;
        /// <summary>
        /// 支付金额
        /// </summary>
        public double Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Amount"));
            }
        }

        /// <summary>
        /// 支付方式id（远程）
        /// </summary>
        public int PaymentTypeId { get; set; }
        /// <summary>
        /// 支付方式id（本地）
        /// </summary>
        public string PaymentTypeObjectId { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        [JsonIgnore]
        public PaymentType PaymentType { get; set; }
        /// <summary>
        /// 所属堂食订单id（远程）
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 所属堂食订单id（本地）
        /// </summary>
        public string OrderObjectId { get; set; }
        /// <summary>
        /// 所属堂食订单
        /// </summary>
        [JsonIgnore]
        public TangOrder TangOrder { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
