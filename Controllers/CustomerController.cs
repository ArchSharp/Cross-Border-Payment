using System.Net;
using System.Threading.Tasks;
using Identity.Data.Dtos.Request;
using Identity.Data.Dtos.Request.User;
using Identity.Data.Dtos.Response;
using Identity.Data.Dtos.Response.User;
using Identity.Helpers;
using Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [Authorize(Policy = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IUriService _uriService;

        public CustomerController(ICustomerService customerService, IUriService uriService)
        {
            _customerService = customerService;
            _uriService = uriService;
        }

        [HttpGet]
        public async Task<IActionResult> Filter([FromQuery] CustomerFilter filter)
        {
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var response = await _customerService.Get(validFilter.PageNumber, validFilter.PageSize, filter.Name, filter.Email);
            string message = "Customers fetched successfully";
            var pagedReponse = PaginationHelper.CreatePagedReponse<Customer>(response.Item1, validFilter, response.Item2, _uriService, route, message);
            return Ok(pagedReponse);
        }

        [HttpPut("{customerId}/activate")]
        public async Task<IActionResult> Activate(string customerId)
        {
            var response = await _customerService.Activate(customerId);
            if (response == null)
                return Unauthorized(new BaseResponse<string>("User not found", HttpStatusCode.Unauthorized, "User not found"));
            return Ok(new BaseResponse<Customer>(response, HttpStatusCode.OK, "User activated succesfully"));
        }

        [HttpPut("{customerId}/deactivate")]
        public async Task<IActionResult> DeActivate(string customerId)
        {
            var response = await _customerService.DeActivate(customerId);
            if (response == null)
                return Unauthorized(new BaseResponse<string>("User not found", HttpStatusCode.Unauthorized, "User not found"));
            return Ok(new BaseResponse<Customer>(response, HttpStatusCode.OK, "User activated succesfully"));
        }
    }
}