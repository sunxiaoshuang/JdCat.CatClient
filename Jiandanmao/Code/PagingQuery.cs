using System;
using System.Collections.Generic;
using System.Text;

namespace Jiandanmao.Code
{
    public class PagingQuery
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int PageCount
        {
            get
            {
                return (int)Math.Ceiling((double)RecordCount / (double)PageSize);
            }
        }
        public int RecordCount { get; set; }

        public int Skip
        {
            get
            {
                return (PageIndex - 1) * PageSize;
            }
        }
        public bool CanPre
        {
            get
            {
                return PageIndex > 1;
            }
        }
        public bool CanNext
        {
            get
            {
                return PageCount > PageIndex;
            }
        }
    }
}
