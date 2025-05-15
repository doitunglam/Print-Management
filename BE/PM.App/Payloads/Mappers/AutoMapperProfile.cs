using AutoMapper;
using PM.Application.Payloads.RequestModels.UserRequests;
using PM.Application.Payloads.ResponseModels.DataUsers;
using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Request_Register, User>();
            CreateMap<User, DataResponseUser>();
        }
        
    }
}
