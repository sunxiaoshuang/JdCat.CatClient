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
        /// 类别中包含的餐桌数量
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
        /// 类别中包含的餐桌数量
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
        public int BusinessId { get; set; }
        public int DeskTypeId { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
