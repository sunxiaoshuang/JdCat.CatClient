﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jiandanmao.Code
{
    public class ClientData: INotifyPropertyChanged, ICloneable
    {
        private bool _isReceive;
        /// <summary>
        /// 是否接单（外卖订单接单打印）
        /// </summary>
        public bool IsReceive
        {
            get
            {
                return _isReceive;
            }
            set
            {
                _isReceive = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsReceive"));
            }
        }

        private bool _isHost;
        /// <summary>
        /// 是否是主收银台（主收银上传门店信息）
        /// </summary>
        public bool IsHost
        {
            get
            {
                return _isHost;
            }
            set
            {
                _isHost = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsHost"));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}