using JdCat.CatClient.Model.Enum;
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
    [Serializable]
    public class TangOrder : ClientBaseEntity, INotifyPropertyChanged
    {
        /// <summary>
        /// 订单当日编号（为0表示还没有下单）
        /// </summary>
        public int Identifier { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string Code { get; set; }
        private double _originalAmount;
        /// <summary>
        /// 原价
        /// </summary>
        public double OriginalAmount
        {
            get { return _originalAmount; }
            set
            {
                _originalAmount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OriginalAmount"));
            }
        }
        private double _amount;
        /// <summary>
        /// 订单金额
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
        /// 订单实收
        /// </summary>
        public double ActualAmount { get; set; }
        /// <summary>
        /// 订单折扣
        /// </summary>
        public double OrderDiscount { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        public double PreferentialAmount { get; set; }
        /// <summary>
        /// 收到的金额
        /// </summary>
        public double ReceivedAmount { get; set; }
        /// <summary>
        /// 找赎
        /// </summary>
        public double GiveAmount { get; set; }
        private int _peopleNumber;
        /// <summary>
        /// 用餐人数
        /// </summary>
        public int PeopleNumber
        {
            get { return _peopleNumber; }
            set
            {
                _peopleNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PeopleNumber"));
            }
        }
        private double _mealFee;
        /// <summary>
        /// 餐位费
        /// </summary>
        public double MealFee
        {
            get { return _mealFee; }
            set
            {
                _mealFee = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MealFee"));
            }
        }
        /// <summary>
        /// 小费
        /// </summary>
        public double Tips { get; set; }
        private string _remark;
        /// <summary>
        /// 订单备注
        /// </summary>
        public string Remark
        {
            get { return _remark; }
            set
            {
                _remark = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Remark"));
            }
        }
        private string _paymentRemark;
        /// <summary>
        /// 支付备注
        /// </summary>
        public string PaymentRemark
        {
            get { return _paymentRemark; }
            set
            {
                _paymentRemark = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PaymentRemark"));
            }
        }
        private TangOrderStatus _orderStatus;
        /// <summary>
        /// 订单状态
        /// </summary>
        public TangOrderStatus OrderStatus
        {
            get { return _orderStatus; }
            set
            {
                _orderStatus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrderStatus"));
            }
        }
        /// <summary>
        /// 订单来源
        /// </summary>
        public OrderSource OrderSource { get; set; }
        /// <summary>
        /// 订单类别
        /// </summary>
        public OrderCategory OrderMode { get; set; }
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
        /// 支付方式名称
        /// </summary>
        public string PaymentTypeName { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayTime { get; set; }
        /// <summary>
        /// 退款原因
        /// </summary>
        public string CancelReason { get; set; }
        /// <summary>
        /// 餐品数量
        /// </summary>
        public int ProductQuantity { get; set; }
        /// <summary>
        /// 所属员工id（远程）
        /// </summary>
        public int StaffId { get; set; }
        /// <summary>
        /// 所属员工id（本地）
        /// </summary>
        public string StaffObjectId { get; set; }
        /// <summary>
        /// 所属员工
        /// </summary>
        [JsonIgnore]
        public Staff Staff { get; set; }
        /// <summary>
        /// 员工名称
        /// </summary>
        public string StaffName { get; set; }
        /// <summary>
        /// 对应的餐桌id
        /// </summary>
        public int? DeskId { get; set; }
        /// <summary>
        /// 餐桌名称
        /// </summary>
        public string DeskName { get; set; }
        /// <summary>
        /// 操作订单的服务台名称
        /// </summary>
        public string CashierName { get; set; }
        /// <summary>
        /// 订单所属商户id
        /// </summary>
        public int BusinessId { get; set; }


        private ObservableCollection<TangOrderProduct> _tangOrderProducts;
        /// <summary>
        /// 订单商品
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<TangOrderProduct> TangOrderProducts
        {
            get { return _tangOrderProducts; }
            set
            {
                _tangOrderProducts = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TangOrderProducts"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
