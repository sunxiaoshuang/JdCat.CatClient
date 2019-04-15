
using JdCat.CatClient.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Jiandanmao.Code
{
    public class SelectItem : INotifyPropertyChanged
    {
        public SelectItem(bool isSelect = false, string content = null)
        {
            _isSelected = isSelect;
            _content = content;
        }
        private bool _isSelected;
        /// <summary>
        /// 选否选中
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => this.MutateVerbose(ref _isSelected, value, RaisePropertyChanged());
        }
        private string _content;
        /// <summary>
        /// 显示文本
        /// </summary>
        public string Content
        {
            get => _content;
            set => this.MutateVerbose(ref _content, value, RaisePropertyChanged());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
    public class SelectItem<T> : SelectItem
    {
        public SelectItem(bool isSelect = false, string content = null, T target = default) : base(isSelect, content)
        {
            _target = target;
        }
        private T _target;
        /// <summary>
        /// 关联对象
        /// </summary>
        public T Target
        {
            get => _target;
            set => this.MutateVerbose(ref _target, value, RaisePropertyChanged());
        }
    }
}
