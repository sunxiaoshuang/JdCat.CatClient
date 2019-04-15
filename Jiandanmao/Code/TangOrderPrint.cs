using JdCat.CatClient.Model;
using System.Net.Sockets;

namespace Jiandanmao.Code
{
    /// <summary>
    /// 一单一打（堂食）
    /// </summary>
    public class TangOrderPrint : TangBackstagePrint
    {
        public TangOrderPrint(TangOrder order, Printer printer, Socket socket, PrintOption option) : base(order, printer, socket, option)
        {
            if(option.Mode == Enum.PrintMode.PreOrder)
            {
                Title = "厨房总单";
            }
            else
            {
                Title = $"厨房总单（{option.Title}）";
            }
        }

        protected override void Printing()
        {
            foreach (var product in Option.Products)
            {
                var name = product.Name;
                if (!string.IsNullOrEmpty(product.Description))
                {
                    name += $"({product.Description})";
                }
                BufferList.Add(PrinterCmdUtils.PrintLineLeftRight2("*" + product.Quantity, name, Printer.FormatLen, 2));
                BufferList.Add(PrinterCmdUtils.NextLine());
            }
        }
    }
}
