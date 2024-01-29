using AutoMapper;
using Identity.Data.Dtos.Request.Auth;
using Identity.Data.Dtos.Response.Auth;
using Identity.Data.Dtos.Response.User;
using Identity.Data.Models;

namespace Identity.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateUser, User>();
            CreateMap<CreateStaff, User>();
            CreateMap<User, UserProfile>();
            CreateMap<User, Customer>();
            CreateMap<User, Staff>();
        }
    }
}