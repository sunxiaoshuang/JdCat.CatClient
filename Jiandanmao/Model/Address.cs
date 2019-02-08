using Jiandanmao.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiandanmao.Model
{
    /// <summary>
    /// 用户地址表
    /// </summary>
    public class Address : BaseEntity
    {
        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string Receiver { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 地图选择信息
        /// </summary>
        public string MapInfo { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double Lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string DetailInfo { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public UserGender Gender { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string PostalCode { get; set; }
        /// 修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// <summary>
        /// 用户
        /// </summary>
        public virtual User User { get; set; }
    }
}
