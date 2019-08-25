using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using Jiandanmao.Enum;
using System.Net.Sockets;

namespace Jiandanmao.Code
{
    public class ThirdSharePrint : ThirdBackstagePrint
    {
        public ThirdSharePrint(ThirdOrder order, Printer printer, Socket socket) : base(order, printer, socket)
        {
        }

        public override void Print()
        {
            if (Products.Count == 0) return;
            foreach (var product in Products)
            {
                for (int i = 0; i < product.Quantity; i++)
                {
                    if (product.Tag1 != null)
                    {
                        product.Tag1.ForEach(item =>
                        {
                            if (Printer.Device.Foods.Contains(item.Id))
                            {
                                Format(item.Name, product.GetDesc());
                            }
                        });
                    }
                    else
                    {
                        Format(product.Name, product.GetDesc());
                    }
                }
            }
        }

        private void Format(string name, string description)
        {
            BeforePrint();
            if (!string.IsNullOrEmpty(description))
            {
                name += $"({description})";
            }
            BufferList.Add(PrinterCmdUtils.FontSizeSetBig(3));
            BufferList.Add(PrinterCmdUtils.AlignLeft());
            BufferList.Add(PrinterCmdUtils.PrintLineLeftRight(name, "*1", Printer.FormatLen, 3));
            BufferList.Add(PrinterCmdUtils.NextLine());
            AfterPrint();
            Send();
        }

        protected override void Printing()
        {
        }
    }
}
