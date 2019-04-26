using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Jiandanmao.Code
{
    public class PrinterCmdUtils
    {

        public const byte ESC = 27;    // 换码
        public const byte FS = 28;    // 文本分隔符
        public const byte GS = 29;    // 组分隔符
        public const byte DLE = 16;    // 数据连接换码
        public const byte EOT = 4;    // 传输结束
        public const byte ENQ = 5;    // 询问字符
        public const byte SP = 32;    // 空格
        public const byte HT = 9;    // 横向列表
        public const byte LF = 10;    // 打印并换行（水平定位）
        public const byte CR = 13;    // 归位键
        public const byte FF = 12;    // 走纸控制（打印并回到标准模式（在页模式下） ）
        public const byte CAN = 24;    // 作废（页模式下取消打印数据 ）

        /**
         * 打印纸一行最大的字节
         */
        private const int LINE_BYTE_SIZE = 32;
        /**
         * 分隔符
         */
        private const string SEPARATOR = "$";
        private static StringBuilder sb = new StringBuilder();

        /// <summary>
        /// 打印机初始化
        /// </summary>
        /// <returns></returns>
        public static byte[] Init_printer()
        {
            byte[] result = new byte[2];
            result[0] = ESC;
            result[1] = 64;
            return result;
        }

        public static byte[] SplitLine(string flag, int format = 80)
        {
            var sb = new StringBuilder();
            var len = format == 80 ? 48 : 32;
            for (int i = 0; i < len; i++)
            {
                sb.Append(flag);
            }
            return Encoding.GetEncoding("gbk").GetBytes(sb.ToString());
        }

        /// <summary>
        /// 打开钱箱
        /// </summary>
        /// <returns></returns>
        public static byte[] Open_money()
        {
            byte[] result = new byte[5];
            result[0] = ESC;
            result[1] = 112;
            result[2] = 48;
            result[3] = 64;
            result[4] = 0;
            return result;
        }

        /// <summary>
        /// 换行
        /// </summary>
        /// <param name="lineNum"></param>
        /// <returns></returns>
        public static byte[] NextLine(int lineNum = 1)
        {
            byte[] result = new byte[lineNum];
            for (int i = 0; i < lineNum; i++)
            {
                result[i] = LF;
            }

            return result;
        }

        /// <summary>
        /// 绘制下划线（1点宽）
        /// </summary>
        /// <returns></returns>
        public static byte[] UnderlineWithOneDotWidthOn()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 45;
            result[2] = 1;
            return result;
        }

        /// <summary>
        /// 绘制下划线（2点宽）
        /// </summary>
        /// <returns></returns>
        public static byte[] UnderlineWithTwoDotWidthOn()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 45;
            result[2] = 2;
            return result;
        }

        /// <summary>
        /// 取消绘制下划线
        /// </summary>
        /// <returns></returns>
        public static byte[] UnderlineOff()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 45;
            result[2] = 0;
            return result;
        }

        /// <summary>
        /// 选择加粗模式
        /// </summary>
        /// <returns></returns>
        public static byte[] BoldOn()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 69;
            result[2] = 0xF;
            return result;
        }

        /// <summary>
        /// 取消加粗模式
        /// </summary>
        /// <returns></returns>
        public static byte[] BoldOff()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 69;
            result[2] = 0;
            return result;
        }

        /// <summary>
        /// 左对齐
        /// </summary>
        /// <returns></returns>
        public static byte[] AlignLeft()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 97;
            result[2] = 0;
            return result;
        }

        /// <summary>
        /// 居中对齐
        /// </summary>
        /// <returns></returns>
        public static byte[] AlignCenter()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 97;
            result[2] = 1;
            return result;
        }

        /// <summary>
        /// 右对齐
        /// </summary>
        /// <returns></returns>
        public static byte[] AlignRight()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 97;
            result[2] = 2;
            return result;
        }

        /// <summary>
        /// 水平方向向右移动col列
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static byte[] Set_HT_position(byte col)
        {
            byte[] result = new byte[4];
            result[0] = ESC;
            result[1] = 68;
            result[2] = col;
            result[3] = 0;
            return result;
        }

        /// <summary>
        /// 字体变大为标准的n倍
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte[] FontSizeSetBig(int num)
        {
            byte realSize = 0;
            switch (num)
            {
                case 1:
                    realSize = 0;
                    break;
                case 2:
                    realSize = 17;
                    break;
                case 3:
                    realSize = 34;
                    break;
                case 4:
                    realSize = 51;
                    break;
                case 5:
                    realSize = 68;
                    break;
                case 6:
                    realSize = 85;
                    break;
                case 7:
                    realSize = 102;
                    break;
                case 8:
                    realSize = 119;
                    break;
            }
            byte[] result = new byte[3];
            result[0] = 29;
            result[1] = 33;
            result[2] = realSize;
            return result;
        }


        /// <summary>
        /// 字体取消倍宽倍高
        /// </summary>
        /// <returns></returns>
        public static byte[] FontSizeSetSmall()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 33;

            return result;
        }

        /// <summary>
        /// 进纸并全部切割
        /// </summary>
        /// <returns></returns>
        public static byte[] FeedPaperCutAll()
        {
            byte[] result = new byte[4];
            result[0] = GS;
            result[1] = 86;
            result[2] = 65;
            result[3] = 0;
            return result;
        }

        /// <summary>
        /// 进纸并切割（左边留一点不切）
        /// </summary>
        /// <returns></returns>
        public static byte[] FeedPaperCutPartial()
        {
            byte[] result = new byte[4];
            result[0] = GS;
            result[1] = 86;
            result[2] = 66;
            result[3] = 0;
            return result;
        }


        // ------------------------切纸-----------------------------
        public static byte[] ByteMerger(byte[] byte_1, byte[] byte_2)
        {
            byte[] byte_3 = new byte[byte_1.Length + byte_2.Length];
            System.Array.Copy(byte_1, 0, byte_3, 0, byte_1.Length);
            System.Array.Copy(byte_2, 0, byte_3, byte_1.Length, byte_2.Length);
            return byte_3;
        }

        public static byte[] ByteMerger(byte[][] byteList)
        {
            int Length = 0;
            for (int i = 0; i < byteList.Length; i++)
            {
                Length += byteList[i].Length;
            }
            byte[] result = new byte[Length];

            int index = 0;
            for (int i = 0; i < byteList.Length; i++)
            {
                byte[] nowByte = byteList[i];
                for (int k = 0; k < byteList[i].Length; k++)
                {
                    result[index] = nowByte[k];
                    index++;
                }
            }
            return result;
        }

        public static byte[][] Byte20Merger(byte[] bytes)
        {
            int size = bytes.Length / 10 + 1;
            byte[][] result = new byte[size][];
            for (int i = 0; i < size; i++)
            {
                byte[] by = new byte[((i + 1) * 10) - (i * 10)];
                System.Array.Copy(bytes, i * 10, by, 0, (i + 1) * 10);
                result[i] = by;
            }
            return result;
        }

        /// <summary>
        /// 打印带有说明的分隔线
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="text"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static byte[] SplitText(string flag, string text, int format = 80)
        {
            var sb = new StringBuilder();
            var len = format == 80 ? 48 : 32;
            var zhCount = CalcZhQuantity(text);             // 中文字符数
            var enCount = text.Length - zhCount;            // 其他字符数
            var textLen = len - (zhCount * 2) - enCount;    // 填充的字符数
            var isEven = textLen % 2 == 0;                  // 是否是偶数
            var width = textLen / 2;
            var str = string.Empty;
            for (int i = 0; i < width; i++)
            {
                str += flag;
            }
            text = str + text + str;
            if (!isEven)
            {
                text += flag;
            }
            return Encoding.GetEncoding("gbk").GetBytes(text);
        }

        /// <summary>
        /// 计算出文本中的中文字符数量
        /// </summary>
        /// <returns></returns>
        public static int CalcZhQuantity(string text)
        {
            // 一个中文字符占用三个字节，一个其他字符占用一个字节
            var len = text.Length;
            var byteLen = Encoding.UTF8.GetByteCount(text);
            return (byteLen - len) / 2;
        }

        /// <summary>
        /// 打印一行，左右两边对齐（如果超出，则左边换行）
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static byte[] PrintLineLeftRight(string left, string right, int formatLen = 48, int fontSize = 1)
        {
            formatLen = formatLen == 80 ? 48 : formatLen;
            formatLen = formatLen == 58 ? 32 : formatLen;
            var zhLeft = CalcZhQuantity(left);          // 左边文本的中文字符长度
            var enLeft = left.Length - zhLeft;          // 左边文本的其他字符长度
            var zhRight = CalcZhQuantity(right);        // 右边文本的中文字符长度
            var enRight = right.Length - zhRight;       // 右边文本的其他字符长度
            var len = formatLen - ((zhLeft * 2 + enLeft + zhRight * 2 + enRight) * fontSize);            // 缺少的字符长度
            if (len > 0)
            {
                for (int i = 0; i < len / fontSize; i++)
                {
                    left += " ";
                }
            }
            else
            {
                var rightWidth = (zhRight * 2 + enRight) * fontSize;
                var leftWidth = formatLen - rightWidth;
                var spaceLen = rightWidth / fontSize;
                var spaceChar = string.Empty;
                for (int i = 0; i < spaceLen; i++)
                {
                    spaceChar += " ";
                }
                var content = string.Empty;
                var index = 0;
                var rowIndex = 1;
                while (true)
                {
                    var line = string.Empty;
                    for (int i = 0; i < int.MaxValue; i++)
                    {
                        if(index + i == left.Length)
                        {
                            return Encoding.GetEncoding("gbk").GetBytes(content + line);
                        }
                        line += left.Substring(index + i, 1);
                        var zh = CalcZhQuantity(line);
                        var en = line.Length - zh;
                        var charLen = (zh * 2 + en) * fontSize;
                        if(leftWidth == charLen)
                        {
                            content += line;
                            if(rowIndex == 1)
                            {
                                content += right;
                            }
                            else
                            {
                                content += spaceChar;
                            }
                            rowIndex++;
                            index = index + i + 1;
                            break;
                        }
                        if (charLen > leftWidth)
                        {
                            var line1 = line.Substring(0, line.Length - 1);
                            zh = CalcZhQuantity(line1);
                            en = line1.Length - zh;
                            charLen = (zh * 2 + en) * fontSize;
                            for (var j = 0; j < (leftWidth - charLen) / fontSize; j++)
                            {
                                line1 += " ";
                            }
                            if (rowIndex == 1)
                            {
                                line1 += right;
                            }
                            else
                            {
                                line1 += spaceChar;
                            }
                            content += line1;
                            rowIndex++;
                            index = index + i;
                            break;
                        }
                    }

                }
            }
            return Encoding.GetEncoding("gbk").GetBytes(left + right);
        }
        /// <summary>
        /// 打印一行，左右两边对齐（如果超出，则右边换行）
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static byte[] PrintLineLeftRight2(string left, string right, int formatLen = 48, int fontSize = 1)
        {
            var zhLeft = CalcZhQuantity(left);          // 左边文本的中文字符长度
            var enLeft = left.Length - zhLeft;          // 左边文本的其他字符长度
            var zhRight = CalcZhQuantity(right);        // 右边文本的中文字符长度
            var enRight = right.Length - zhRight;       // 右边文本的其他字符长度
            var len = formatLen - ((zhLeft * 2 + enLeft + zhRight * 2 + enRight) * fontSize);            // 缺少的字符长度
            if (len > 0)
            {
                for (int i = 0; i < len / fontSize; i++)
                {
                    left += " ";
                }
            }
            else
            {
                var leftWidth = (zhLeft * 2 + enLeft) * fontSize;
                var rightWidth = formatLen - leftWidth;
                var spaceLen = leftWidth / fontSize;
                var spaceChar = string.Empty;
                for (int i = 0; i < spaceLen; i++)
                {
                    spaceChar += " ";
                }
                var content = string.Empty;
                var index = 0;
                var rowIndex = 1;
                while (true)
                {
                    var line = string.Empty;
                    for (int i = 0; i < int.MaxValue; i++)
                    {
                        if (index + i == right.Length)
                        {
                            return Encoding.GetEncoding("gbk").GetBytes(content + line);
                        }
                        line += right.Substring(index + i, 1);
                        var zh = CalcZhQuantity(line);
                        var en = line.Length - zh;
                        var charLen = (zh * 2 + en) * fontSize;
                        if (rightWidth == charLen)
                        {
                            content += line;
                            if (rowIndex == 1)
                            {
                                content = left + content;
                            }
                            else
                            {
                                content = spaceChar + content;
                            }
                            rowIndex++;
                            index = index + i + 1;
                            break;
                        }
                        if (charLen > rightWidth)
                        {
                            var line1 = line.Substring(0, line.Length - 1);
                            zh = CalcZhQuantity(line1);
                            en = line1.Length - zh;
                            charLen = (zh * 2 + en) * fontSize;
                            for (var j = 0; j < (rightWidth - charLen) / fontSize; j++)
                            {
                                line1 = " " + line1;
                            }
                            if (rowIndex == 1)
                            {
                                line1 = left + line1;
                            }
                            else
                            {
                                line1 = spaceChar + line1;
                            }
                            content += line1;
                            rowIndex++;
                            index = index + i;
                            break;
                        }
                    }

                }
            }
            return Encoding.GetEncoding("gbk").GetBytes(left + right);
        }
        /// <summary>
        /// 打印一行左中右对齐，中间与右边最多占用15个字符位
        /// </summary>
        /// <param name="left"></param>
        /// <param name="middle"></param>
        /// <param name="right"></param>
        /// <param name="formatLen"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        public static byte[] PrintLineLeftMidRight(string left, string middle, string right, int formatLen = 48, int fontSize = 1)
        {
            var place = string.Empty;
            for (int i = 0; i < 15 - middle.Length - right.Length; i++)
            {
                place += " ";
            }
            right = middle + place + right;

            var buffer = PrintLineLeftRight(left, right, formatLen, fontSize: fontSize);
            return buffer;
        }

    }
}
