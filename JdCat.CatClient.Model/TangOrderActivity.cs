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
    public class TangOrderActivity : ClientBaseEntity, INotifyPropertyChanged
    {
        private double _amount;
        /// <summary>
        /// 活动金额
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
        private OrderActivityType _type;
        /// <summary>
        /// 活动类别
        /// </summary>
        public OrderActivityType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Type"));
            }
        }
        /// <summary>
        /// 活动id
        /// </summary>
        public int? ActivityId { get; set; }
        /// <summary>
        /// 活动备注
        /// </summary>
        private string _remark;
        public string Remark
        {
            get { return _remark; }
            set
            {
                _remark = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Remark"));
            }
        }
        /// <summary>
        /// 所属堂食订单id（本地）
        /// </summary>
        public string TangOrderObjectId { get; set; }
        /// <summary>
        /// 所属堂食订单id（远程）
        /// </summary>
        public int TangOrderId { get; set; }
        /// <summary>
        /// 所属堂食订单
        /// </summary>
        [JsonIgnore]
        public TangOrder TangOrder { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
