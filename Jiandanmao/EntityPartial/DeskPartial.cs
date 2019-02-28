using Jiandanmao.Code;
using Jiandanmao.Uc;
using Jiandanmao.ViewModel;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Jiandanmao.Entity
{
    public partial class Desk
    {
        [JsonIgnore]
        public ICommand SaveCommand => new AnotherCommandImplementation(Save);
        [JsonIgnore]
        public ICommand UpdateCommand => new AnotherCommandImplementation(Update);
        [JsonIgnore]
        public ICommand DeleteCommand => new AnotherCommandImplementation(Delete);
        [JsonIgnore]
        public ICommand OpenPeopleNumberCommand => new AnotherCommandImplementation(OpenPeopleNumber);

        private async void Save(object o)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return;
            }
            var exist = ApplicationObject.App.Desks.Any(a => a.Name == Name && a.Id != Id);
            if (exist)
            {
                MessageBox.Show($"已存在名为[{Name}]的餐桌，请更换名称");
                return;
            }
            var type = ApplicationObject.App.DeskTypes.FirstOrDefault(a => a.IsCheck);
            if (type != null)
            {
                if (Id > 0)
                {
                    var result = await Request.UpdateDesk(this);
                    if (type.Desks != null)
                    {
                        var desk = type.Desks.FirstOrDefault(a => a.Id == Id);
                        desk.Name = Name;
                        desk.Quantity = Quantity;
                    }
                }
                else
                {
                    var result = await Request.SaveDesk(this);
                    if(type.Desks == null)
                    {
                        type.Desks = new ObservableCollection<Desk>();
                    }
                    type.Desks.Add(result.Data);
                }
                type.ReloadDeskQuantity();
            }
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private async void Update(object o)
        {
            var dialog = new AddDesk(this);
            await DialogHost.Show(dialog, "RootDialog");
        }

        private async void Delete(object o)
        {
            var result = MessageBox.Show($"确定删除餐桌[{Name}]吗？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) return;
            var json = await Request.DeleteDesk(this);
            if (!json.Success) return;
            var type = ApplicationObject.App.DeskTypes.FirstOrDefault(a => a.Id == DeskTypeId);
            if (type == null) return;
            type.Desks.Remove(this);
            type.ReloadDeskQuantity();
        }

        private async void OpenPeopleNumber(object o)
        {
            ((CateringViewModel)o).Desk = this;
            await DialogHost.Show(new PeopleNumber { DataContext = o });
        }
    }
}
