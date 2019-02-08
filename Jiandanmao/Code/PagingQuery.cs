using System;
using System.Collections.Generic;
using System.Text;

namespace Jiandanmao.Code
{
    public class PagingQuery
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int RecordCount { get; set; }

        public int Skip
        {
            get
            {
                return (PageIndex - 1) * PageSize;
            }
        }
    }
}
