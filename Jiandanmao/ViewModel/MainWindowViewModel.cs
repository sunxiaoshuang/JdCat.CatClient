using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jiandanmao.Demo;
using Jiandanmao.Uc;

namespace Jiandanmao.ViewModel
{
    class MainWindowViewModel
    {
        public ContorllerItem[] Items { get; }


        public MainWindowViewModel()
        {
            Items = new[]
            {
                //new ContorllerItem("测试", new TransitionDemo()),
                new ContorllerItem("餐桌", new Catering(new CateringViewModel())),
                new ContorllerItem("今日订单", new OrderList(){ DataContext = new OrderListViewModel() }),
                new ContorllerItem("打印机配置", new PrinterSetting(){ DataContext = new PrinterSettingViewModel() }),
                new ContorllerItem("餐桌设定", new DeskSetting(){ DataContext = new DeskViewModel() }),
                //new ContorllerItem("主页", new Home(){ DataContext = new HomeViewModel() })
            };
            

        }

    }
}
