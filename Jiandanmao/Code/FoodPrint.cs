using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;

using Jiandanmao.Enum;
using System.Net.Sockets;

namespace Jiandanmao.Code
{
    /// <summary>
    /// 一菜一打
    /// </summary>
    public class FoodPrint : BackstagePrint
    {
        public FoodPrint(Order order, Printer printer, Socket socket) : base(order, printer, socket)
        {
        }
        public override void Print()
        {
            if (Products.Count == 0) return;
            foreach (var product in Products)
            {
                if (product.Feature == ProductFeature.SetMeal)
                {
                    if (product.Tag1 == null) continue;
                    product.Tag1.ForEach(item =>
                    {
                        if (Printer.Device.Foods.Contains(item.Id))
                        {
                            var name = item.Name + $"[{product.Name}]";
                            Format(name, product.Description, product.Quantity + "");
                        }
                    });
                    continue;
                }
                else
                {
                    Format(product.Name, product.Description, product.Quantity + "");
                }
            }
        }

        private void Format(string name, string description, string quantity)
        {
            BeforePrint();
            if (!string.IsNullOrEmpty(description))
            {
                name += $"({description})";
            }
            BufferList.Add(PrinterCmdUtils.FontSizeSetBig(3));
            BufferList.Add(PrinterCmdUtils.AlignLeft());
            BufferList.Add(PrinterCmdUtils.PrintLineLeftRight(name, "*" + double.Parse(quantity).ToString(), Printer.FormatLen, 3));
            BufferList.Add(PrinterCmdUtils.NextLine());
            AfterPrint();
            Send();
        }
        protected override void Printing()
        {
        }
    }
}
