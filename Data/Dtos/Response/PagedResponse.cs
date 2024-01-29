using System;
using System.Net;

namespace Identity.Data.Dtos.Response
{
    public class PagedResponse<T> : BaseResponse<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Uri FirstPage { get; set; }
        public Uri LastPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public Uri NextPage { get; set; }
        public Uri PreviousPage { get; set; }

        public PagedResponse(T data, int pageNumber, int pageSize, string message) : base(data, HttpStatusCode.OK, message)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
        }
    }
}