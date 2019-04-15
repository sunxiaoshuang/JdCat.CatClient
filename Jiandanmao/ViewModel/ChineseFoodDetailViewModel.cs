using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Jiandanmao.Uc;
using Jiandanmao.Code;
using MaterialDesignThemes.Wpf.Transitions;

using Autofac;
using Jiandanmao.Enum;
using Jiandanmao.Helper;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using System.Collections.Generic;
using JdCat.CatClient.IService;
using System.Text.RegularExpressions;
using JdCat.CatClient.Common;

namespace Jiandanmao.ViewModel
{
    /// <summary>
    /// ChineseFoodDetail 订单商品详情页
    /// </summary>
    public class ChineseFoodDetailViewModel : BaseViewModel
    {
        private ChineseFoodDetail ThisControl;
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);
        public ICommand SaveCommand => new AnotherCommandImplementation(Save);
        public ICommand ClickItemCommand => new AnotherCommandImplementation(ClickItem);
        public ICommand SelectedFormatCommand => new AnotherCommandImplementation(SelectedFormat);
        public ICommand IncreaseCommand => new AnotherCommandImplementation(Increase);
        public ICommand ReduceCommand => new AnotherCommandImplementation(Reduce);
        public ICommand PriceChangedCommand => new AnotherCommandImplementation(PriceChanged);
        public ICommand DistanceChangedCommand => new AnotherCommandImplementation(DistanceChanged);
        public ICommand UnsubscribeCommand => new AnotherCommandImplementation(Unsubscribe);

        /// <summary>
        /// 订单
        /// </summary>
        public TangOrder Order { get; set; }
        /// <summary>
        /// 订单产品对象
        /// </summary>
        public TangOrderProduct Good { get; set; }
        /// <summary>
        /// 产品对象
        /// </summary>
        public Product Product { get; set; }
        /// <summary>
        /// 是否可以修改
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 是否可以退菜
        /// </summary>
        public bool CanReturn { get; set; }
        /// <summary>
        /// 是否有多个规格
        /// </summary>
        public bool IsMutilFormat { get; set; }
        /// <summary>
        /// 是否提交
        /// </summary>
        public bool IsSubmit { get; set; }
        /// <summary>
        /// 规格列表
        /// </summary>
        public List<SelectItem<ProductFormat>> Formats { get; set; }
        /// <summary>
        /// 口味列表
        /// </summary>
        public List<SelectItem> Flavors { get; set; }


        #region 界面属性
        private double _originalPrice;
        /// <summary>
        /// 原价
        /// </summary>
        public double OriginalPrice { get => _originalPrice; set => this.MutateVerbose(ref _originalPrice, value, RaisePropertyChanged()); }

        private double _price;
        /// <summary>
        /// 单价
        /// </summary>
        public double Price { get => _price; set => this.MutateVerbose(ref _price, value, RaisePropertyChanged()); }

        private double _distance;
        /// <summary>
        /// 折扣
        /// </summary>
        public double Distance { get => _distance; set => this.MutateVerbose(ref _distance, value, RaisePropertyChanged()); }

        private double _quantity;
        /// <summary>
        /// 数量
        /// </summary>
        public double Quantity { get => _quantity; set => this.MutateVerbose(ref _quantity, value, RaisePropertyChanged()); }

        private double _amount;
        /// <summary>
        /// 小计
        /// </summary>
        public double Amount { get => _amount; set => this.MutateVerbose(ref _amount, value, RaisePropertyChanged()); }

        private string _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get => _remark; set => this.MutateVerbose(ref _remark, value, RaisePropertyChanged()); }

        private double _ReturnQuantity;
        /// <summary>
        /// 退菜数量
        /// </summary>
        public double ReturnQuantity { get => _ReturnQuantity; set => this.MutateVerbose(ref _ReturnQuantity, value, RaisePropertyChanged()); }

        #endregion
        public ChineseFoodDetailViewModel(TangOrder order, TangOrderProduct good, Product product)
        {
            Order = order;
            Good = good;
            Product = product;
            Enable = (good.ProductStatus & TangOrderProductStatus.Cumulative) > 0;
            CanReturn = (good.ProductStatus & TangOrderProductStatus.CanReturn) > 0;

            OriginalPrice = good.OriginalPrice;
            Price = good.Price;
            Distance = good.Discount;
            Quantity = good.Quantity;
            Amount = good.Amount;
            Remark = good.Remark;
            ReturnQuantity = good.Quantity;
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IUtilService>();
                Flavors = service.GetAll<SystemMark>()?.Where(a => a.Category == MarkCategory.Flavor).Select(a => new SelectItem(false, a.Name)).ToList();
                Formats = product.Formats.Select(a => new SelectItem<ProductFormat>(false, a.Name, a)).ToList();

                var format = Formats.FirstOrDefault(a => a.Target.Id == Good.FormatId);
                if (format != null) format.IsSelected = true;

                var descritions = good.Description?.Split('|');
                if (descritions != null)
                {
                    descritions.ForEach(a =>
                    {
                        var flavor = Flavors.FirstOrDefault(b => b.Content == a);
                        if (flavor == null) return;
                        flavor.IsSelected = true;
                    });
                }
            }
        }

        private void Loaded(object o)
        {
            ThisControl = (ChineseFoodDetail)o;
            if (Formats.Count <= 1)
            {
                ThisControl.formatContainter.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void ClickItem(object o)
        {
            var item = (SelectItem)o;
            item.IsSelected = !item.IsSelected;
        }

        private void SelectedFormat(object o)
        {
            if (o is SelectItem<ProductFormat> format)
            {
                if (format.IsSelected) return;
                Formats.ForEach(a => a.IsSelected = false);
                format.IsSelected = true;
                OriginalPrice = format.Target.Price;
                Price = Math.Round(OriginalPrice * Distance / 10, 2);
                Calculate();
            }
        }

        private void Reduce(object o)
        {
            if (Quantity == 1) return;
            Quantity--;
            Calculate();
        }

        private void Increase(object o)
        {
            Quantity++;
            Calculate();
        }

        private void PriceChanged(object o)
        {
            var txt = (TextBox)o;
            var text = txt.Text.Trim();
            if (!double.TryParse(text, out double price))
            {
                var reg = Regex.Match(text, @"\d+");
                txt.Text = reg.Value;
                txt.SelectionStart = int.MaxValue;
                return;
            }
            Price = price;
            Distance = Math.Round(Price / OriginalPrice * 10, 1);
            Calculate();
        }

        private void DistanceChanged(object o)
        {
            var txt = (TextBox)o;
            var text = txt.Text.Trim();
            if (!double.TryParse(text, out double distance))
            {
                var reg = Regex.Match(text, @"\d+");
                txt.Text = reg.Value;
                txt.SelectionStart = int.MaxValue;
                return;
            }
            Distance = distance;
            Price = Math.Round(OriginalPrice * distance / 10, 1);
            Calculate();
        }

        private void Save(object o)
        {
            var format = Formats.FirstOrDefault(a => a.IsSelected)?.Target;
            var description = string.Empty;
            var diff = Good.Quantity - Quantity;
            if (Formats.Count > 1)
            {
                description += format?.Name + "|";
            }
            if (Flavors != null && Flavors.Count > 0)
            {
                foreach (var item in Flavors.Where(a => a.IsSelected))
                {
                    description += item.Content + "|";
                }
            }
            if (description.Length > 0)
            {
                description = description.Substring(0, description.Length - 1);
            }
            Good.Description = description;
            Good.FormatId = format?.Id ?? 0;
            Good.OriginalPrice = OriginalPrice;
            Good.Price = Price;
            Good.Quantity = Quantity;
            Good.Amount = Amount;
            Good.Discount = Distance;
            Good.Remark = Remark;
            Product.SelectedQuantity -= diff;
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                service.UpdateOrderProduct(Good);
            }
            IsSubmit = true;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private void Unsubscribe(object o)
        {
            var snack = (Snackbar)o;
            if (ReturnQuantity > Quantity)
            {
                snack.MessageQueue.Enqueue("退菜数量不能大于点菜数量");
                return;
            }
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IOrderService>();
                var good = service.Unsubscribe(Order, Good, ReturnQuantity);
                Print(good);
            }
            IsSubmit = true;
            DialogHost.CloseDialogCommand.Execute(null, null);
            DialogHost.CloseDialogCommand.Execute(null, null);
        }




        private void Calculate()
        {
            Amount = Price * Quantity;
        }
        private void Print(params TangOrderProduct[] products)
        {
            products.ForEach(item =>
            {
                if (item.Tag != null || item.Feature != ProductFeature.SetMeal) return;
                var ids = item.ProductIdSet.Split(',').Select(a => int.Parse(a));
                item.Tag = ApplicationObject.App.Products.Where(a => ids.Contains(a.Id)).ToList();
            });
            ApplicationObject.Print(Order, 2, option: new PrintOption
            {
                Title = "退菜单",
                Order = Order,
                Type = 2,
                Mode = PrintMode.Return,
                Products = products
            });
        }

    }
}
