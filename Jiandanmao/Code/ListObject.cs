using JdCat.CatClient.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Jiandanmao.Code
{
    /// <summary>
    /// 列表对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListObject<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _pageIndex;
        private int _pageSize;
        private int _pageCount;

        public ListObject(int pageSize = 50)
        {
            _pageSize = pageSize;
        }
        private ObservableCollection<T> _list;
        public ObservableCollection<T> List
        {
            get { return _list; }
            set
            {
                _list = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("List"));
            }
        }

        private bool _canNext;
        public bool CanNext
        {
            get { return _canNext; }
            set
            {
                _canNext = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanNext"));
            }
        }

        private bool _canPrev;
        public bool CanPrev
        {
            get { return _canPrev; }
            set
            {
                _canPrev = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanPrev"));
            }
        }

        private ObservableCollection<T> _originalList;
        public ObservableCollection<T> OriginalList
        {
            get => _originalList; set
            {
                _originalList = value;
                _pageIndex = 0;
                CanPrev = false;
                if (value == null || value.Count <= _pageSize)
                {
                    CanNext = false;
                }
                else
                {
                    CanNext = true;
                }
                _pageCount = value == null ? 0 : (int)Math.Ceiling(value.Count / (double)_pageSize);
                SetList();
            }
        }

        private void SetList()
        {
            List = OriginalList?.Skip(_pageIndex * _pageSize).Take(_pageSize).ToObservable();
        }


        public ICommand NextCommand => new AnotherCommandImplementation(Next);
        public ICommand PrevCommand => new AnotherCommandImplementation(Prev);

        private void Next(object obj)
        {
            if (!CanNext) return;
            CanPrev = true;
            _pageIndex++;
            if (_pageCount <= _pageIndex + 1) CanNext = false;
            SetList();
        }
        private void Prev(object obj)
        {
            if (!_canPrev) return;
            CanNext = true;
            _pageIndex--;
            if (_pageIndex <= 0) CanPrev = false;
            SetList();
        }
    }
}
