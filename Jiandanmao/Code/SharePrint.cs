using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Jiandanmao.Model;

namespace Jiandanmao.Code
{
    public class SharePrint : BackstagePrint
    {
        public SharePrint(Order order, Printer printer, Socket socket) : base(order, printer, socket)
        {
        }

        public override void Print()
        {
            if (Products.Count == 0) return;
            foreach (var product in Products)
            {
                for (int i = 0; i < product.Quantity; i++)
                {
                    if (product.Feature == Enum.ProductFeature.SetMeal)
                    {
                        if (product.Tag1 == null) continue;
                        product.Tag1.ForEach(item =>
                        {
                            if (Printer.Foods.Contains(item.ID))
                            {
                                Format(item.Name + $"[{product.Name}]", product.Description);
                            }
                        });
                    }
                    else
                    {
                        Format(product.Name, product.Description);
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
