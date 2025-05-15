using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PM.Application.InterfaceService;
using PM.Application.Payloads.Mappers;
using PM.Application.Payloads.RequestModels.NewFolder;
using PM.Application.Payloads.ResponseModels.DataTeams;
using PM.Application.Payloads.Responses;
using PM.Domain.Entities;
using PM.Domain.InterfaceRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.ImplementService
{
    public class TeamService : ITeamService
    {

        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly UserConverter _userConverter;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IBaseRepository<ConfirmEmail> _baseConfirmEmailRepository;
        private readonly IBaseRepository<Permissions> _basePermissionsRepository;
        private readonly IBaseRepository<Role> _baseRoleRepository;
        private readonly IBaseRepository<RefreshToken> _baseRefreshTokenRepository;
        private readonly IBaseRepository<Team> _baseTeamRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        public TeamService(IBaseRepository<User> baseUserRepository, UserConverter userConverter, IConfiguration configuration, IUserRepository userRepository, IEmailService emailService,
            IBaseRepository<ConfirmEmail> baseConfirmEmailRepository, IBaseRepository<Permissions> basePermissionsRepository, IBaseRepository<Role> baseRoleRepository,
            IBaseRepository<RefreshToken> baseRefreshTokenRepository, IHttpContextAccessor contextAccessor, IBaseRepository<Team> baseTeamRepository)
        {
            _baseUserRepository = baseUserRepository;
            _userConverter = userConverter;
            _configuration = configuration;
            _userRepository = userRepository;
            _emailService = emailService;
            _baseConfirmEmailRepository = baseConfirmEmailRepository;
            _basePermissionsRepository = basePermissionsRepository;
            _baseRoleRepository = baseRoleRepository;
            _baseRefreshTokenRepository = baseRefreshTokenRepository;
            _contextAccessor = contextAccessor;
            _baseTeamRepository = baseTeamRepository;
        }

        public async Task<ResponseObject<DataResponseTeam>> AddUserToTeamAsync(long teamId, long userId)
        {
            try
            {
                var team = await _baseTeamRepository.GetByIdAsync(teamId);
                var user = await _userRepository.GetUserById(userId);

                if (team == null)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Team not found.",
                        Data = null
                    };
                }

                if (user == null)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "User not found.",
                        Data = null
                    };
                }

                if (user.TeamId == teamId)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "User is already assigned to this team.",
                        Data = null
                    };
                }

                user.TeamId = teamId;
                team.NumberOfMember++;

                await _userRepository.UpdateAsync(user);
                await _baseTeamRepository.UpdateAsync(team);

                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "User added to team successfully.",
                    Data = new DataResponseTeam
                    {
                        Name = team.Name,
                        Description = team.Description,
                        NumberOfMember = team.NumberOfMember,
                        CreateTime = team.CreateTime,
                        UpdateTime = team.UpdateTime,
                        ManagerId = team.ManagerId
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }




        public async Task<ResponseObject<DataResponseTeam>> CreateTeamAsync(Request_Create request)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Invalidate user.",
                        Data = null
                    };
                }
                if (!currentUser.IsInRole("Admin"))
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "Only for admin",
                        Data = null
                    };
                }

                if (request == null)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Invalid request",
                        Data = null
                    };
                }

                var team = new Team
                {
                    Name = request.Name,
                    Description = request.Description,
                    NumberOfMember = request.NumberOfMember ?? 0,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    ManagerId = request.ManagerId
                };

                await _baseTeamRepository.CreateAsync(team);

                var response = new DataResponseTeam
                {
                    Name = team.Name,
                    Description = team.Description,
                    NumberOfMember = team.NumberOfMember,
                    CreateTime = team.CreateTime,
                    UpdateTime = team.UpdateTime,
                    ManagerId = team.ManagerId
                };

                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Team created successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }


        public async Task<ResponseObject<DataResponseTeam>> DeleteTeamAsync(long teamId)
        {
            try
            {
                var team = await _baseTeamRepository.GetByIdAsync(teamId);
                if (team == null)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Team not found.",
                        Data = null
                    };
                }

                var users = await _userRepository.GetUsersByTeamId(teamId);

                foreach (var user in users)
                {
                    user.TeamId = 0;
                    await _userRepository.UpdateAsync(user);
                }

                await _baseTeamRepository.DeleteAsync(teamId);

                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Team deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }


        public async Task<ResponseObject<IEnumerable<DataResponseTeam>>> GetAllTeamsAsync()
        {
            try
            {
                var teams = await _baseTeamRepository.GetAllAsync();

                if (teams == null || !teams.Any())
                {
                    return new ResponseObject<IEnumerable<DataResponseTeam>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "No teams found.",
                        Data = null
                    };
                }

                var response = teams.Select(team => new DataResponseTeam
                {
                    Name = team.Name,
                    Description = team.Description,
                    NumberOfMember = team.NumberOfMember,
                    CreateTime = team.CreateTime,
                    UpdateTime = team.UpdateTime,
                    ManagerId = team.ManagerId,
                    Id = team.Id,
                });

                return new ResponseObject<IEnumerable<DataResponseTeam>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Teams retrieved successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<IEnumerable<DataResponseTeam>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }


        public async Task<ResponseObject<DataResponseTeam>> GetTeamByIdAsync(long teamId)
        {
            try
            {
                var team = await _baseTeamRepository.GetByIdAsync(teamId);
                if (team == null)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Team not found.",
                        Data = null
                    };
                }

                var response = new DataResponseTeam
                {
                    Name = team.Name,
                    Description = team.Description,
                    NumberOfMember = team.NumberOfMember,
                    CreateTime = team.CreateTime,
                    UpdateTime = team.UpdateTime,
                    ManagerId = team.ManagerId
                };

                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Team retrieved successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }


        public async Task<ResponseObject<DataResponseTeam>> UpdateTeamAsync(long teamId, Request_Create request)
        {
            try
            {
                var team = await _baseTeamRepository.GetByIdAsync(teamId);
                if (team == null)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Team not found.",
                        Data = null
                    };
                }

                team.Name = request.Name;
                team.Description = request.Description;
                team.NumberOfMember = request.NumberOfMember;
                team.ManagerId = request.ManagerId;
                team.UpdateTime = DateTime.Now;

                await _baseTeamRepository.UpdateAsync(team);

                var response = new DataResponseTeam
                {
                    Name = team.Name,
                    Description = team.Description,
                    NumberOfMember = team.NumberOfMember,
                    CreateTime = team.CreateTime,
                    UpdateTime = team.UpdateTime,
                    ManagerId = team.ManagerId
                };

                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Team updated successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

    }
}
