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
        public ContorllerItem[] Items { get; }


        public MainWindowViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            if (snackbarMessageQueue == null) throw new ArgumentNullException(nameof(snackbarMessageQueue));
            //new ContorllerItem("测试", new TransitionDemo()),
            //new ContorllerItem("餐桌", new Catering(new CateringViewModel())),
            //new ContorllerItem("主页", new Home(){ DataContext = new HomeViewModel() })
            var list = new List<ContorllerItem>();

            if (ApplicationObject.App.IsAdmin)
            {
                list.Add(new ContorllerItem("外卖订单", new OrderList() { DataContext = new OrderListViewModel() }));
                list.Add(new ContorllerItem("库存设置", new ProductStock() { DataContext = new ProductStockViewModel() }));
                list.Add(new ContorllerItem("系统设置", new SystemSetting { DataContext = new SystemSettingViewModel(snackbarMessageQueue) }));
            }
            else
            {
                if (ApplicationObject.App.ClientData.Mode == Enum.HostMode.Chinese)
                {
                    list.Add(new ContorllerItem("餐台", new ChineseFood { DataContext = new ChineseFoodViewModel(snackbarMessageQueue) }));
                }
                else
                {
                    list.Add(new ContorllerItem("收银", new FastFood { DataContext = new FastFoodViewModel() }));
                }
                list.Add(new ContorllerItem("外卖订单", new OrderList() { DataContext = new OrderListViewModel() }));
                list.Add(new ContorllerItem("库存设置", new ProductStock() { DataContext = new ProductStockViewModel() }));
                if ((ApplicationObject.App.Staff.StaffPost.Authority & StaffPostAuth.Manager) > 0)
                {
                    list.Add(new ContorllerItem("系统设置", new SystemSetting { DataContext = new SystemSettingViewModel(snackbarMessageQueue) }));
                }
            }
            Items = list.ToArray();

        }

    }
}
