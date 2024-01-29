
using System;
using Identity.Data.Dtos.Request;

namespace Identity.Interfaces
{
    public interface IUriService
    {
        public Uri GetPageUri(PaginationFilter filter, string route);
    }
}