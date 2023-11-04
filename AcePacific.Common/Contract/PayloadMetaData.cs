using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcePacific.Common.Contract
{
    public class PayloadMetaData
    {
        public PayloadMetaData(int pageIndex, int pageSize, int totalCount)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
            PageCount = totalCount > PageSize ? ((int)Math.Ceiling((decimal)totalCount / pageSize)) : 1;
        }

        public PayloadMetaData(int pageSize, int totalCount)
        {
            PageIndex = 1;
            PageSize = pageSize;
            TotalCount = totalCount;
            PageCount = totalCount > PageSize ? ((int)Math.Ceiling((decimal)totalCount / pageSize)) : 1;
        }

        public PayloadMetaData()
        {

        }

        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int PageCount { get; private set; }
        public int TotalCount { get; private set; }
    }
}
