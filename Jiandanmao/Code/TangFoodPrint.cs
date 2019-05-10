using JdCat.CatClient.Model;
using Jiandanmao.Enum;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Jiandanmao.Code
{
    /// <summary>
    /// 一菜一打（堂食）
    /// </summary>
    public class TangFoodPrint : TangBackstagePrint
    {
        public TangFoodPrint(TangOrder order, Printer printer, Socket socket, PrintOption option) : base(order, printer, socket, option)
        {
            
        }
        public override void Print()
        {
            foreach (var product in Products)
            {
                if (product.Feature == JdCat.CatClient.Model.Enum.ProductFeature.SetMeal)
                {
                    if (product.Tag == null) continue;
                    if (product.Tag is List<Product> products)
                    {
                        products.ForEach(item =>
                        {
                            if (Printer.Device.Foods.Contains(item.Id))
                            {
                                var name = item.Name + $"[{product.Name}]";
                                Format(name, product.Description, product.Quantity, product.Remark);
                            }
                        });
                    }
                }
                else
                {
                    Format(product.Name, product.Description, product.Quantity, product.Remark);
                }
            }
        }

        private void Format(string name, string description, double quantity, string remark)
        {
            BeforePrint();
            if (!string.IsNullOrEmpty(description))
            {
                name += $"({description})";
            }
            BufferList.Add(PrinterCmdUtils.FontSizeSetBig(2));
            BufferList.Add(PrinterCmdUtils.AlignLeft());
            BufferList.Add(PrinterCmdUtils.PrintLineLeftRight2("*" + quantity, name, Printer.FormatLen, 2));
            BufferList.Add(PrinterCmdUtils.NextLine());
            if (remark != null)
            {
                BufferList.Add(TextToByte($"备注：{remark}"));
                BufferList.Add(PrinterCmdUtils.NextLine());
            }
            AfterPrint();
            Send();
        }
        protected override void Printing()
        {
        }
    }
}
