using Autofac;
using JdCat.CatClient.Common;
using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using Jiandanmao.Code;
using Jiandanmao.Uc;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Jiandanmao.ViewModel
{
    public class FastFoodViewModel : BaseViewModel
    {

        #region  声明

        public object ThisContorler;
        public ICommand LoadedCommand => new AnotherCommandImplementation(Loaded);

        #endregion

        #region 属性



        #endregion



        #region 界面绑定方法

        private void Loaded(object o)
        {

        }

        #endregion
    }
}