using JdCat.CatClient.Model;
using Jiandanmao.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Jiandanmao.Code
{
    /// <summary>
    /// 后厨打印（堂食）
    /// </summary>
    public abstract class TangBackstagePrint
    {
        public TangOrder Order { get; set; }
        public Printer Printer { get; set; }
        private Socket _socket;
        public List<byte[]> BufferList { get; set; }
        public PrintOption Option { get; set; }
        public string Title { get; set; }

        public TangBackstagePrint(TangOrder order, Printer printer, Socket socket, PrintOption option)
        {
            Order = order;
            Printer = printer;
            _socket = socket;
            Option = option;
            Title = option.Title;
            if (option.Mode == PrintMode.PreOrder) Title = "点菜单";
        }
        public virtual void Print()
        {
            if (Option.Products.Count() == 0) return;
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
            // 打印当日序号
            BufferList.Add(PrinterCmdUtils.AlignCenter());
            BufferList.Add(PrinterCmdUtils.FontSizeSetBig(2));
            BufferList.Add(TextToByte(Title));
            BufferList.Add(PrinterCmdUtils.NextLine());
            BufferList.Add(PrinterCmdUtils.AlignLeft());
            BufferList.Add(PrinterCmdUtils.PrintLineLeftRight($"餐桌：{Order.DeskName}", $"单号：{Order.Identifier}", fontSize: 2));
            BufferList.Add(PrinterCmdUtils.NextLine());
            if(!string.IsNullOrEmpty(Order.Remark))
            {
                BufferList.Add(PrinterCmdUtils.BoldOn());
                BufferList.Add(TextToByte($"备注：{Order.Remark}"));
                BufferList.Add(PrinterCmdUtils.NextLine());
                BufferList.Add(PrinterCmdUtils.BoldOff());
                BufferList.Add(PrinterCmdUtils.NextLine());
            }
            BufferList.Add(PrinterCmdUtils.FontSizeSetBig(1));
            BufferList.Add(TextToByte($"服务员：{Order.StaffName}"));
            BufferList.Add(PrinterCmdUtils.NextLine());
            BufferList.Add(TextToByte($"打印时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}"));
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

        protected byte[] TextToByte(string text)
        {
            return Encoding.GetEncoding("gbk").GetBytes(text);
        }
    }
}
