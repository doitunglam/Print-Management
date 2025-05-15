using PM.Application.Payloads.ResponseModels.DataUsers;
using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.Mappers
{
    public class UserConverter
    {
        public DataResponseUser EntityToDTO(User user)
        {
            return new DataResponseUser
            {
                Avatar = user.Avatar,
                CreateTime = user.CreateTime,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                FullName = user.FullName,
                Id = user.Id,
                Password = user.Password,
                PhoneNumber = user.PhoneNumber,
                TeamId = user.TeamId,
                UpdateTime = user.UpdateTime,
                UserName = user.UserName,
            };
        }
    }
}
