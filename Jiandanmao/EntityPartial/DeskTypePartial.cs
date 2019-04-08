using Jiandanmao.Code;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Jiandanmao.Entity
{
    public partial class DeskType
    {
        [JsonIgnore]
        public ICommand SaveCommand => new AnotherCommandImplementation(Save);
        [JsonIgnore]
        public ICommand SelectTypeCommand => new AnotherCommandImplementation(SelectType);

        private async void Save(object o)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return;
            }
            var exist = ApplicationObject.App.DeskTypes.Any(a => a.Name == Name && a.Id != Id);
            if (exist)
            {
                MessageBox.Show($"已存在名为[{Name}]的区域，请更换名称");
                return;
            }
            if (Id > 0)
            {
                var result = await Request.UpdateDeskTypeAsync(this);
                var type = ApplicationObject.App.DeskTypes.First(a => a.Id == Id);
                type.Name = Name;
                type.Sort = Sort;
            }
            else
            {
                var result = await Request.SaveDeskTypeAsync(this);
                ApplicationObject.App.DeskTypes.Add(result.Data);
            }
            ApplicationObject.App.DeskTypes.OrderBy(a => a.Sort);
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
        private async void SelectType(object o)
        {

        }
    }
}
