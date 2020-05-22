using Auth.Domain.Models;
using Auth.WebApi.Dtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.WebApi.Helper
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User,UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}
