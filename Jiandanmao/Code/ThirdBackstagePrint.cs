﻿using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using Jiandanmao.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Jiandanmao.Code
{
    /// <summary>
    /// 后厨打印（第三方订单）
    /// </summary>
    public abstract class ThirdBackstagePrint
    {
        public ThirdOrder Order { get; set; }
        public Printer Printer { get; set; }
        private Socket _socket;
        public List<byte[]> BufferList { get; set; }
        public List<ThirdOrderProduct> Products { get; set; }

        public ThirdBackstagePrint(ThirdOrder order, Printer printer, Socket socket)
        {
            Order = order;
            Printer = printer;
            _socket = socket;
            Products = Order.ThirdOrderProducts.Where(a => a.Tag1 != null || Printer.Device.Foods.Contains(a.ProductId)).ToList();
        }
        public virtual void Print()
        {
            if (Products.Count == 0) return;
            BeforePrint();
            Printing();
            AfterPrint();
            Send();
        }
        public void Send()
        {
            if (BufferList == null || BufferList.Count == 0) return;
            BufferList.ForEach(a =>
            {
                _socket.Send(a);
            });
        }
        protected virtual void BeforePrint()
        {
            BufferList = new List<byte[]>();
            var sign = Order.OrderSource == 0 ? "美团" : "饿了么";
            // 打印当日序号
            BufferList.Add(PrinterCmdUtils.AlignCenter());
            BufferList.Add(PrinterCmdUtils.FontSizeSetBig(2));
            BufferList.Add(Encoding.GetEncoding("gbk").GetBytes("#" + Order.DaySeq + sign));
            BufferList.Add(PrinterCmdUtils.NextLine());
            BufferList.Add(PrinterCmdUtils.NextLine());
            BufferList.Add(PrinterCmdUtils.AlignLeft());
            // 备注
            if (!string.IsNullOrEmpty(Order.Caution))
            {
                BufferList.Add(PrinterCmdUtils.FontSizeSetBig(2));
                BufferList.Add(PrinterCmdUtils.BoldOn());
                BufferList.Add(Encoding.GetEncoding("gbk").GetBytes($"备注：{Order.Caution}"));
                BufferList.Add(PrinterCmdUtils.NextLine());
                BufferList.Add(PrinterCmdUtils.FontSizeSetBig(1));
                BufferList.Add(PrinterCmdUtils.BoldOff());
                BufferList.Add(PrinterCmdUtils.NextLine());
            }
            BufferList.Add(PrinterCmdUtils.FontSizeSetBig(1));

            BufferList.Add(Encoding.GetEncoding("gbk").GetBytes("下单时间：" + Order.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

            BufferList.Add(PrinterCmdUtils.NextLine());
            BufferList.Add(PrinterCmdUtils.SplitLine("-", Printer.Device.Format));
            BufferList.Add(PrinterCmdUtils.NextLine());
            BufferList.Add(PrinterCmdUtils.FontSizeSetBig(2));
        }
        protected virtual void AfterPrint()
        {
            BufferList.Add(PrinterCmdUtils.NextLine(2));
            BufferList.Add(PrinterCmdUtils.FeedPaperCutAll());
        }
        protected abstract void Printing();
    }
}
