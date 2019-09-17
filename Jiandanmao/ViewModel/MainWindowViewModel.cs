using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JdCat.CatClient.Model.Enum;
using Jiandanmao.Code;
using Jiandanmao.Demo;
using Jiandanmao.Uc;
using MaterialDesignThemes.Wpf;

namespace Jiandanmao.ViewModel
{
    public class MainWindowViewModel
    {
        public ControllerItem[] Items { get; }


        public MainWindowViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            if (snackbarMessageQueue == null) throw new ArgumentNullException(nameof(snackbarMessageQueue));
            //new ContorllerItem("测试", new TransitionDemo()),
            //new ContorllerItem("餐桌", new Catering(new CateringViewModel())),
            //new ContorllerItem("主页", new Home(){ DataContext = new HomeViewModel() })
            var list = new List<ControllerItem>();

            // 仅接单
            if (!ApplicationObject.App.Config.IsCash)
            {
                list.Add(new ControllerItem("外卖订单", new OrderList() { DataContext = new OrderListViewModel() }));
                Items = list.ToArray();
                return;
            }

            // 收银
            if (ApplicationObject.App.IsAdmin)
            {
                list.Add(new ControllerItem("外卖订单", new OrderList() { DataContext = new OrderListViewModel() }));
                list.Add(new ControllerItem("库存设置", new ProductStock() { DataContext = new ProductStockViewModel() }));
                list.Add(new ControllerItem("系统设置", new SystemSetting { DataContext = new SystemSettingViewModel(snackbarMessageQueue) }));
            }
            else
            {
                //list.Add(new ContorllerItem("实施", new FastFoodHoogup { DataContext = new FastFoodHoogupViewModel() }));
                if (ApplicationObject.App.ClientData.Mode == Enum.HostMode.Chinese)
                {
                    list.Add(new ControllerItem("餐台", new ChineseFood { DataContext = new ChineseFoodViewModel(snackbarMessageQueue) }));
                }
                else
                {
                    list.Add(new ControllerItem("收银", new FastFood { DataContext = new FastFoodViewModel(snackbarMessageQueue) }));
                }
                list.Add(new ControllerItem("外卖订单", new OrderList() { DataContext = new OrderListViewModel() }));
                list.Add(new ControllerItem("堂食订单", new TangOrderList() { DataContext = new TangOrderListViewModel() }));
                list.Add(new ControllerItem("库存设置", new ProductStock() { DataContext = new ProductStockViewModel() }));
                if ((ApplicationObject.App.Staff.StaffPost.Authority & StaffPostAuth.Manager) > 0)
                {
                    list.Add(new ControllerItem("系统设置", new SystemSetting { DataContext = new SystemSettingViewModel(snackbarMessageQueue) }));
                }
            }
            Items = list.ToArray();

        }

    }
}
