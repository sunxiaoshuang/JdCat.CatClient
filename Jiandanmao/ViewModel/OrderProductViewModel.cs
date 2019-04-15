
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using Jiandanmao.Uc;
using Jiandanmao.Code;
using System.ComponentModel;
using JdCat.CatClient.Common;
using JdCat.CatClient.Model;

namespace Jiandanmao.ViewModel
{
    public class CateringProductViewModel : BaseViewModel
    {
        public ObservableCollection<ProductTypeViewModel> Types { get; set; }
        private ObservableCollection<ProductViewModel> _products;
        public ObservableCollection<ProductViewModel> Products
        {
            get
            {
                return _products;
            }
            set
            {
                this.MutateVerbose(ref _products, value, RaisePropertyChanged());
            }
        }
        private ObservableCollection<ProductViewModel> _all { get; set; }
        public ObservableCollection<ProductViewModel> All
        {
            get
            {
                if (_all == null)
                {
                    _all = new ObservableCollection<ProductViewModel>();
                    Types.ForEach(a => {
                        if(a.Products != null)
                        {
                            a.Products.ForEach(b => _all.Add(b));
                        }
                    });
                }
                return _all;
            }
        }

        public class ProductTypeViewModel : INotifyPropertyChanged
        {
            /// <summary>
            /// 菜单类别
            /// </summary>
            public ProductType ProductType { get; set; }
            private ObservableCollection<ProductViewModel> _products;
            /// <summary>
            /// 类别所包含的菜品
            /// </summary>
            public ObservableCollection<ProductViewModel> Products
            {
                get
                {
                    return _products;
                }
                set
                {
                    _products = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Products"));
                }
            }
            private bool _isCheck;
            /// <summary>
            /// 是否选中
            /// </summary>
            public bool IsCheck
            {
                get { return _isCheck; }
                set
                {
                    _isCheck = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsCheck"));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
        public class ProductViewModel : INotifyPropertyChanged
        {
            private Product _product;
            /// <summary>
            /// 数量
            /// </summary>
            public Product Product
            {
                get { return _product; }
                set
                {
                    _product = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Product"));
                }
            }
            private int _quantity;
            /// <summary>
            /// 数量
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

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
