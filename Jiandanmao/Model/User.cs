using System;
using System.Collections.Generic;
using System.Text;
using Jiandanmao.Enum;

namespace Jiandanmao.Model
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// 用户年龄
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// 用户头像地址
        /// </summary>
        public string AvatarUrl { get; set; }
        /// <summary>
        /// 用户所在城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 用户国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public UserGender Gender { get; set; }
        /// <summary>
        /// 常用语言
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 微信唯一标识码
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 是否已经注册
        /// </summary>
        public bool IsRegister { get; set; }
        /// <summary>
        /// 是否获取了手机号
        /// </summary>
        public bool IsPhone { get; set; }
        /// <summary>
        /// 依赖注册的商户
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 用户地址
        /// </summary>
        public virtual ICollection<Address> Addresses { get; set; }
        /// <summary>
        /// 用户订单集合
        /// </summary>
        public virtual ICollection<Order> Orders { get; set; }
        /// <summary>
        /// 微信的会话密钥
        /// </summary>
        public int Skey { get; set; }
    }
}
