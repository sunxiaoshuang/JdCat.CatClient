﻿using JdCat.CatClient.Common;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;

using Jiandanmao.Enum;
using System.Net.Sockets;

namespace Jiandanmao.Code
{
    /// <summary>
    /// 一菜一打
    /// </summary>
    public class ThirdFoodPrint : ThirdBackstagePrint
    {
        public ThirdFoodPrint(ThirdOrder order, Printer printer, Socket socket) : base(order, printer, socket)
        {
        }
        public override void Print()
        {
            if (Products.Count == 0) return;
            foreach (var product in Products)
            {
                if (product.Tag1 != null)
                {
                    product.Tag1.ForEach(item =>
                    {
                        if (Printer.Device.Foods.Contains(item.Id))
                        {
                            var name = item.Name;
                            Format(name, product.GetDesc(), product.Quantity + "");
                        }
                    });
                    continue;
                }
                else
                {
                    Format(product.Name, product.GetDesc(), product.Quantity + "");
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
            BufferList.Add(PrinterCmdUtils.PrintLineLeftRight(name, "*" + quantity, Printer.FormatLen, 3));
            BufferList.Add(PrinterCmdUtils.NextLine());
            AfterPrint();
            Send();

            //var content = name + "     *" + quantity;
            //UtilHelper.Log($"打印机：{Printer.Device.Name}，内容：#{Order.DaySeq}，{content}");

            //UtilHelper.ErrorLog("编号:" + Order.DaySeq.ToString());
            //UtilHelper.ErrorLog(BufferList);

        }
        protected override void Printing()
        {
        }
    }
}
