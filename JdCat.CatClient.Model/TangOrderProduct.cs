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
    [Serializable]
    public class TangOrderProduct: ClientBaseEntity, INotifyPropertyChanged, ICloneable
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }
        private double _quantity;
        /// <summary>
        /// 商品数量
        /// </summary>
        public double Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Quantity"));
            }
        }
        private double _price;
        /// <summary>
        /// 商品单价
        /// </summary>
        public double Price
        {
            get { return _price; }
            set
            {
                _price = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Price"));
            }
        }

        private double _amount;
        /// <summary>
        /// 总价
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

        private double _originalPrice;
        /// <summary>
        /// 原价
        /// </summary>
        public double OriginalPrice
        {
            get { return _originalPrice; }
            set
            {
                _originalPrice = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OriginalPrice"));
            }
        }

        private double _discount;
        /// <summary>
        /// 折扣
        /// </summary>
        public double Discount
        {
            get { return _discount; }
            set
            {
                _discount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Discount"));
            }
        }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string Src { get; set; }
        private string _description;
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
            }
        }
        private string _refundReason;
        /// <summary>
        /// 退菜原因
        /// </summary>
        public string RefundReason
        {
            get { return _refundReason; }
            set
            {
                _refundReason = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RefundReason"));
            }
        }
        private string _remark;
        /// <summary>
        /// 备注
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
        /// <summary>
        /// 套餐商品的id集
        /// </summary>
        public string ProductIdSet { get; set; }
        /// <summary>
        /// 商品特性
        /// </summary>
        public ProductFeature Feature { get; set; }
        private TangOrderProductStatus _productStatus;
        /// <summary>
        /// 商品状态
        /// </summary>
        public TangOrderProductStatus ProductStatus
        {
            get { return _productStatus; }
            set
            {
                _productStatus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProductStatus"));
            }
        }
        /// <summary>
        /// 所属商品id
        /// </summary>
        public int ProductId { get; set; }
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
        public TangOrder TangOrder { get; set; }
        /// <summary>
        /// 商品规格id
        /// </summary>
        public int FormatId { get; set; }

        [JsonIgnore]
        public object Tag { get; set; }

        private bool _isSelected;
        /// <summary>
        /// 是否被选中
        /// </summary>
        [JsonIgnore]
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
