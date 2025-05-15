using PM.Application.Payloads.RequestModels.NewFolder;
using PM.Application.Payloads.ResponseModels.DataTeams;
using PM.Application.Payloads.Responses;
using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.InterfaceService
{
    public interface ITeamService
    {
        Task<ResponseObject<DataResponseTeam>> CreateTeamAsync(Request_Create request);
        Task<ResponseObject<DataResponseTeam>> UpdateTeamAsync(long teamId, Request_Create request);
        Task<ResponseObject<DataResponseTeam>> GetTeamByIdAsync(long teamId);
        Task<ResponseObject<DataResponseTeam>> DeleteTeamAsync(long teamId);

        Task<ResponseObject<IEnumerable<DataResponseTeam>>> GetAllTeamsAsync();
        Task<ResponseObject<DataResponseTeam>> AddUserToTeamAsync(long teamId, long userId);
    }
}
