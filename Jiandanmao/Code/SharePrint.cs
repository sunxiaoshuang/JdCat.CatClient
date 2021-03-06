﻿using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using Jiandanmao.Enum;
using System.Net.Sockets;

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
                    if (product.Feature == ProductFeature.SetMeal)
                    {
                        if (product.Tag1 == null) continue;
                        product.Tag1.ForEach(item =>
                        {
                            if (Printer.Device.Foods.Contains(item.Id))
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
