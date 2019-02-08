
using Jiandanmao.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiandanmao.Model
{
    /// <summary>
    /// 商户表
    /// </summary>
    public class Business : BaseEntity, ICloneable
    {
        /// <summary>
        /// 商户名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 登录帐号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime? RegisterDate { get; set; }
        /// <summary>
        /// 商户备注
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 营业执照
        /// </summary>
        public string BusinessLicense { get; set; }
        /// <summary>
        /// 营业执照地址
        /// </summary>
        public string BusinessLicenseImage { get; set; }
        /// <summary>
        /// 特殊资质
        /// </summary>
        public string SpecialImage { get; set; }
        /// <summary>
        /// 邀请码
        /// </summary>
        public string InvitationCode { get; set; }
        /// <summary>
        /// 小程序id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 小程序App Secret
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string MchId { get; set; }
        /// <summary>
        /// 商户密钥
        /// </summary>
        public string MchKey { get; set; }
        /// <summary>
        /// 门店id
        /// </summary>
        public string StoreId { get; set; }
        /// <summary>
        /// LOGO地址
        /// </summary>
        public string LogoSrc { get; set; }
        /// <summary>
        /// 是否自动接单
        /// </summary>
        public bool IsAutoReceipt { get; set; }
        /// <summary>
        /// 运费
        /// </summary>
        public decimal? Freight { get; set; } = 4;
        /// <summary>
        /// 商户编号
        /// </summary>
        public string DadaSourceId { get; set; }
        /// <summary>
        /// 门店编号
        /// </summary>
        public string DadaShopNo { get; set; }
        /// <summary>
        /// 商户所属的城市
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 商户所属城市的区号
        /// </summary>
        public string CityCode { get; set; }
        /// <summary>
        /// 配送范围
        /// </summary>
        public double Range { get; set; } = 0;

        /// <summary>
        /// 飞印商户编码
        /// </summary>
        public string FeyinMemberCode { get; set; }
        /// <summary>
        /// 飞印API密钥
        /// </summary>
        public string FeyinApiKey { get; set; }
        /// <summary>
        /// 默认打印设备
        /// </summary>
        public string DefaultPrinterDevice { get; set; }
        /// <summary>
        /// 商铺位置经度
        /// </summary>
        public double Lng { get; set; }
        /// <summary>
        /// 商铺位置纬度
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// 经营开始时间
        /// </summary>
        public string BusinessStartTime { get; set; } = "06:00";
        /// <summary>
        /// 经营结束时间
        /// </summary>
        public string BusinessEndTime { get; set; } = "21:00";
        /// <summary>
        /// 起送金额
        /// </summary>
        public decimal MinAmount { get; set; }
        /// <summary>
        /// 是否打烊
        /// </summary>
        public bool IsClose { get; set; }
        /// <summary>
        /// 配送服务商
        /// </summary>
        public ServiceProvider ServiceProvider { get; set; }
        /// <summary>
        /// 是否正式发布
        /// </summary>
        public bool IsPublish { get; set; } = true;

        /// <summary>
        /// 产品列表集合
        /// </summary>
        public virtual ICollection<Product> Products { get; set; }
        /// <summary>
        /// 产品类别集合
        /// </summary>
        public virtual ICollection<ProductType> ProductsTypes { get; set; }
        /// <summary>
        /// 商户订单集合
        /// </summary>
        public virtual ICollection<Order> Orders { get; set; }
        /// <summary>
        /// 满减活动
        /// </summary>
        public virtual ICollection<SaleFullReduce> SaleFullReduces { get; set; }
        /// <summary>
        /// 商品折扣活动
        /// </summary>
        public virtual ICollection<SaleProductDiscount> SaleProductDiscounts { get; set; }


        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
