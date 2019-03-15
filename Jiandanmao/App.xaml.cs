using Autofac;
using JdCat.CatClient.IService;
using JdCat.CatClient.Service;
using Jiandanmao.Code;
using Jiandanmao.DataBase;
using Jiandanmao.Redis;
using StackExchange.Redis;
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
            // 加载配置
            ApplicationObject.App.Config = new ApplicationConfig();
            var type = typeof(ApplicationConfig);
            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                var val = ConfigurationManager.AppSettings[key];
                var property = type.GetProperty(key);
                if (property == null) continue;
                property.SetValue(ApplicationObject.App.Config, val);
            }
            // 注册依赖
            var builder = new ContainerBuilder();
            //builder.RegisterType<CatDbContext>();
            //builder.RegisterType<RedisHelper>();
            builder.Register(a => new DatabaseConfig
            {
                Api = ApplicationObject.App.Config.ApiUrl,
                KeyPrefix = ApplicationObject.App.Config.RedisDefaultKey,
                StaffPrefix = ApplicationObject.App.Config.StaffPrefix
            }).SingleInstance();
            var conn = ConnectionMultiplexer.Connect(ConfigurationManager.ConnectionStrings["RedisConnectionString"].ConnectionString);
            builder.Register<IConnectionMultiplexer>(a => conn).SingleInstance();
            builder.RegisterType<StaffService>().As<IStaffService>();
            ApplicationObject.App.DataBase = builder.Build();
        }
    }
}
