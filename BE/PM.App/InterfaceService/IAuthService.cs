using PM.Application.Payloads.RequestModels.UserRequests;
using PM.Application.Payloads.ResponseModels.DataUsers;
using PM.Application.Payloads.Responses;
using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.InterfaceService
{
    public interface IAuthService
    {
        Task<ResponseObject<DataResponseUser>> Register(Request_Register request);
        Task<string> ConfirmRegisterAccount(string confirmCode);
        Task<ResponseObject<DataResponseLogin>> GetJwtTokenAsync(User user);
        Task<ResponseObject<DataResponseLogin>> Login(Request_Login request);

        Task<ResponseObject<List<DataResponseUser>>> GetAllUsersAsync();

        Task<ResponseObject<DataResponseUser>> UpdateUserAsync(long userId, Request_UpdateUser request);

        //Task<ResponseObject<DataResponseUser>> GetUserForUpdateAsync(long userId);
        Task<ResponseObject<DataResponseUser>> DeleteUserAsync(long userId);
        Task<ResponseObject<DataResponseUser>> GetUserInfoAsync();
        Task<ResponseObject<DataResponseUser>> ChangePassword(long userId, Request_ChangePassword request);
        Task<string> AddRolesToUser(long userId, List<string> roles);
        Task<string> DeleteRoles (long userId, List<string> roles);
    }
}
