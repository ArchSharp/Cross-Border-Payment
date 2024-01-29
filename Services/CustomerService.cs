using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Data.Dtos.Response.User;
using Identity.Interfaces;

namespace Identity.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUserService _userService;
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;
        public CustomerService(IUserService userService, ITransactionService transactionService, IMapper mapper)
        {
            _userService = userService;
            _transactionService = transactionService;
            _mapper = mapper;
        }

        public async Task<Customer> Activate(string id)
        {
            Customer response = null;
            var user = await _userService.Status(id, true);
            if (user != null)
                response = _mapper.Map<Customer>(user);
            return response;
        }

        public async Task<Customer> DeActivate(string id)
        {
            Customer response = null;
            var user = await _userService.Status(id, false);
            if (user != null)
                response = _mapper.Map<Customer>(user);
            return response;
        }

        public async Task<Tuple<List<Customer>, int>> Get(int pageNumber, int pageSize, string name, string email)
        {
            var users = await _userService.Users(pageNumber, pageSize, name, email, "User");
            var totalRecords = await _userService.Count("User");
            var customers = _mapper.Map<List<Customer>>(users);
            foreach (var customer in customers)
                customer.Transactions = await _transactionService.Get(customer.Id);
            return new Tuple<List<Customer>, int>(customers, totalRecords);
        }
    }
}