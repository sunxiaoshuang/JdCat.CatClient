﻿using System;
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
using System.Threading.Tasks;

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
        public ICommand OriginalPriceChangedCommand => new AnotherCommandImplementation(OriginalPriceChanged);
        public ICommand PriceChangedCommand => new AnotherCommandImplementation(PriceChanged);
        public ICommand DistanceChangedCommand => new AnotherCommandImplementation(DistanceChanged);
        public ICommand UnsubscribeCommand => new AnotherCommandImplementation(UnsubscribeAsync);

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
        /// <summary>
        /// 单品备注
        /// </summary>
        public List<string> GoodRemarks { get; set; }
        /// <summary>
        /// 退菜原因
        /// </summary>
        public List<string> ReturnReasons { get; set; }


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

        private double _discount;
        /// <summary>
        /// 折扣
        /// </summary>
        public double Discount { get => _discount; set => this.MutateVerbose(ref _discount, value, RaisePropertyChanged()); }

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

        private string _returnReason;
        /// <summary>
        /// 退菜原因
        /// </summary>
        public string ReturnReason { get => _returnReason; set => this.MutateVerbose(ref _returnReason, value, RaisePropertyChanged()); }

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
            Discount = good.Discount;
            Quantity = good.Quantity;
            Amount = good.Amount;
            Remark = good.Remark;
            ReturnQuantity = good.Quantity;
            using (var scope = ApplicationObject.App.DataBase.BeginLifetimeScope())
            {
                var service = scope.Resolve<IUtilService>();
                var marks = service.GetAll<SystemMark>();
                Flavors = marks?.Where(a => a.Category == MarkCategory.Flavor).Select(a => new SelectItem(false, a.Name)).ToList();
                Formats = product.Formats.Select(a => new SelectItem<ProductFormat>(false, a.Name, a)).ToList();
                ReturnReasons = marks?.Where(a => a.Category == MarkCategory.RefundFoodReason).Select(a => a.Name).ToList();
                GoodRemarks = marks?.Where(a => a.Category == MarkCategory.GoodRemark).Select(a => a.Name).ToList();

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
                Price = Math.Round(OriginalPrice * Discount / 10, 2);
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

        private void OriginalPriceChanged(object o)
        {
            var txt = (TextBox)o;
            OriginalPrice = GetTextBoxNumber((TextBox)o);
            Price = Math.Round(OriginalPrice * Discount / 10, 1);
            Calculate();
        }

        private void PriceChanged(object o)
        {
            var txt = (TextBox)o;
            Price = GetTextBoxNumber((TextBox)o);
            Discount = OriginalPrice == 0 ? 0 : Math.Round(Price / OriginalPrice * 10, 1);
            Calculate();
        }

        private void DistanceChanged(object o)
        {
            var txt = (TextBox)o;
            Discount = GetTextBoxNumber((TextBox)o);
            Price = Math.Round(OriginalPrice * Discount / 10, 1);
            Calculate();
        }

        private void Save(object o)
        {
            var format = Formats.FirstOrDefault(a => a.IsSelected)?.Target;
            var description = string.Empty;
            var diff = Good.Quantity - Quantity;
            if (Formats.Count > 1)
            {
                description += format.Name + "|";
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
            Good.Discount = Discount;
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
        


        private async void UnsubscribeAsync(object o)
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
                // 记录退菜
                Good.RefundReason = ReturnReason;
                var good = service.Unsubscribe(Order, Good, ReturnQuantity);
                // 退还库存
                var util = scope.Resolve<IUtilService>();
                var stock = await util.GetAsync<ProductStockModel>(good.ProductId.ToString());
                if (stock != null)
                {
                    stock.Stock += good.Quantity;
                    await util.SetProductStocksAsync(stock);
                    util.PubSubscribe("SystemMessage", $"StockChange|{stock.ToJson()}");
                }
                // 打印退菜
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
        /// <summary>
        /// 获取输入框中的数字
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="property"></param>
        private double GetTextBoxNumber(TextBox txt)
        {
            var val = 0d;
            if (string.IsNullOrEmpty(txt.Text))
            {
                val = 0;
                txt.Text = "0";
            }
            else
            {
                val = double.Parse(txt.Text);
            }
            Task.Run(() =>
            {
                Task.Delay(100);
                Mainthread.BeginInvoke((Action)delegate ()
                {
                    txt.CaretIndex = int.MaxValue;
                });
            });
            return val;
        }
    }
}
