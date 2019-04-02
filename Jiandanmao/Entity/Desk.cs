using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jiandanmao.Entity
{
    /// <summary>
    /// 餐桌
    /// </summary>
    public partial class Desk : INotifyPropertyChanged, ICloneable
    {
        public int Id { get; set; }
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

        public event PropertyChangedEventHandler PropertyChanged;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
