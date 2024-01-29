
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.Data.Dtos.Response.User;

namespace Identity.Interfaces
{
    public interface ICustomerService
    {
        Task<Tuple<List<Customer>, int>> Get(int pageNumber, int pageSize, string name, string email);
        Task<Customer> Activate(string id);
        Task<Customer> DeActivate(string id);
    }
}