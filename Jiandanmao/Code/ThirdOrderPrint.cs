using JdCat.CatClient.Common;
using JdCat.CatClient.Model;

using System.Net.Sockets;

namespace Jiandanmao.Code
{
    public class ThirdOrderPrint : ThirdBackstagePrint
    {
        public ThirdOrderPrint(ThirdOrder order, Printer printer, Socket socket) : base(order, printer, socket)
        {
        }

        protected override void Printing()
        {
            BufferList.Add(PrinterCmdUtils.FontSizeSetBig(3));
            BufferList.Add(PrinterCmdUtils.AlignLeft());
            foreach (var product in Products)
            {
                var name = product.Name;
                if (!string.IsNullOrEmpty(product.GetDesc()))
                {
                    name += $"({product.GetDesc()})";
                }
                BufferList.Add(PrinterCmdUtils.PrintLineLeftRight(name, "*" + product.Quantity, Printer.FormatLen, 3));
                BufferList.Add(PrinterCmdUtils.NextLine());
            }
        }

    }
}
