using Jiandanmao.Code;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Jiandanmao
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ApplicationObject.App.Config = new ApplicationConfig();
            var type = typeof(ApplicationConfig);
            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                var val = ConfigurationManager.AppSettings[key];
                var property = type.GetProperty(key);
                if (property == null) continue;
                property.SetValue(ApplicationObject.App.Config, val);
            }
        }
    }
}
