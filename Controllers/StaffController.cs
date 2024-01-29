using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Identity.Data.Dtos.Request;
using Identity.Data.Dtos.Request.Auth;
using Identity.Data.Dtos.Request.User;
using Identity.Data.Dtos.Response;
using Identity.Data.Dtos.Response.User;
using Identity.Data.Models;
using Identity.Helpers;
using Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Identity.Controllers
{
    [Authorize(Policy = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly IUriService _uriService;

        public StaffController(IStaffService staffService, IUriService uriService)
        {
            _staffService = staffService;
            _uriService = uriService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaff staff)
        {
            bool isExist = await _staffService.Exist(staff.Email);
            if (isExist)
                return Conflict(new BaseResponse<string>("User already exist", HttpStatusCode.Conflict, "User already exists"));
            var response = await _staffService.CreateStaff(staff);
            if (response == null)
                return BadRequest(new BaseResponse<string>("Failed to create user", HttpStatusCode.BadRequest, "Failed to create user"));
            return Created("", new BaseResponse<Staff>(response, HttpStatusCode.Created, "User registered succesfully"));
        }

        [HttpGet]
        public async Task<IActionResult> Filter([FromQuery] StaffPaginationFilter filter)
        {
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var staffFilter = new StaffFilter { Email = filter.Email, Name = filter.Name, Position = filter.Position };
            var response = await _staffService.Get(validFilter.PageNumber, validFilter.PageSize, staffFilter);
            string message = "Staffs fetched successfully";
            var pagedReponse = PaginationHelper.CreatePagedReponse<Staff>(response.Item1, validFilter, response.Item2, _uriService, route, message);
            return Ok(pagedReponse);
        }

        [HttpGet("role")]
        public async Task<IActionResult> Role() =>
            Ok(new BaseResponse<List<Role>>(await _staffService.Role(), HttpStatusCode.OK, "Role fetched succesfully"));

        [HttpGet("permission")]
        public async Task<IActionResult> Permission() =>
           Ok(new BaseResponse<List<Permission>>(await _staffService.Pemissions(), HttpStatusCode.OK, "Permission fetched succesfully"));

        [HttpPut("role/{roleId}/permission")]
        public async Task<IActionResult> Role(string roleId, [FromBody] RolePermission rolePermission)
        {
            var role = await _staffService.Role(roleId);
            if (role == null)
                return NotFound(new BaseResponse<string>("Role not found", HttpStatusCode.NotFound, "Role not found"));
            var response = await _staffService.AssignPermmission(roleId, rolePermission.Permissions);
            if (response == null)
                return BadRequest(new BaseResponse<string>("Failed to assign permission", HttpStatusCode.BadRequest, "Failed to assign permission"));
            return Ok(new BaseResponse<Role>(response, HttpStatusCode.OK, "Permission assigned to role succesfully"));
        }
    }
}