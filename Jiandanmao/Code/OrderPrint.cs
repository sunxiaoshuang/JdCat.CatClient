using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Jiandanmao.Model;

namespace Jiandanmao.Code
{
    public class OrderPrint : BackstagePrint
    {
        public OrderPrint(Order order, Printer printer, Socket socket) : base(order, printer, socket)
        {
        }

        protected override void Printing()
        {
            BufferList.Add(PrinterCmdUtils.FontSizeSetBig(3));
            BufferList.Add(PrinterCmdUtils.AlignLeft());
            foreach (var product in Products)
            {
                var name = product.Name;
                if (!string.IsNullOrEmpty(product.Description))
                {
                    name += $"({product.Description})";
                }
                BufferList.Add(PrinterCmdUtils.PrintLineLeftRight(name, "*" + double.Parse(product.Quantity + "").ToString(), Printer.FormatLen, 3));
                BufferList.Add(PrinterCmdUtils.NextLine());
            }
        }
    }
}
