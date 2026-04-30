using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteX.UAF.Entities.Base
{
    public class APIResponse<T>
    {
        public bool isSuccess { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public PaginationMeta Meta { get; set; }
    }

    public class PaginationMeta
    {
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        public PaginationMeta(int totalRecords, int pageNumber, int pageSize)
        {
            TotalRecords = totalRecords;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        }
    }
}