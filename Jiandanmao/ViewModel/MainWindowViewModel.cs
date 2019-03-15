using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jiandanmao.Code;
using Jiandanmao.Demo;
using Jiandanmao.Uc;
using MaterialDesignThemes.Wpf;

namespace Jiandanmao.ViewModel
{
    class MainWindowViewModel
    {
        public ContorllerItem[] Items { get; }


        public MainWindowViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            if (snackbarMessageQueue == null) throw new ArgumentNullException(nameof(snackbarMessageQueue));
            Items = new[]
            {
                //new ContorllerItem("测试", new TransitionDemo()),
                //new ContorllerItem("餐桌", new Catering(new CateringViewModel())),
                new ContorllerItem("员工管理", new StaffManager{ DataContext = new StaffViewModel(snackbarMessageQueue) }),
                new ContorllerItem("系统设置", new SystemSetting{ DataContext = new SystemSettingViewModel() }),
                new ContorllerItem("今日订单", new OrderList(){ DataContext = new OrderListViewModel() }),
                new ContorllerItem("餐桌设定", new DeskSetting(){ DataContext = new DeskViewModel() }),
                new ContorllerItem("打印机配置", new PrinterSetting(){ DataContext = new PrinterSettingViewModel() }),
                //new ContorllerItem("主页", new Home(){ DataContext = new HomeViewModel() })
            };
            

        }

    }
}
