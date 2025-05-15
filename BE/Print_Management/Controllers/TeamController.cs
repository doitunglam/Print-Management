using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Application.InterfaceService;
using PM.Application.Payloads.RequestModels.NewFolder;
using PM.Domain.Entities;

namespace Print_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;
        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpPost("CreateTeam")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateTeam([FromBody] Request_Create request)
        {
            var result = await _teamService.CreateTeamAsync(request);
            return StatusCode(result.Status, result);
        }

        [HttpGet("GetTeamById/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetTeamById(long id)
        {
            var result = await _teamService.GetTeamByIdAsync(id);
            return StatusCode(result.Status, result);
        }

        [HttpGet("GetAllTeams")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllTeams()
        {
            var result = await _teamService.GetAllTeamsAsync();
            return Ok(new
            {
                Status = StatusCodes.Status200OK,
                Message = "Teams retrieved successfully.",
                Data = result
            });
        }

        [HttpPut("UpdateTeam/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateTeam(long id, [FromBody] Request_Create request)
        {
            var result = await _teamService.UpdateTeamAsync(id, request);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("DeleteTeam/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteTeam(long id)
        {
            var result = await _teamService.DeleteTeamAsync(id);
            return StatusCode(result.Status, result);
        }

        [HttpPost("team/{teamId}/user/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddUserToTeam(long teamId, long userId)
        {
            var result = await _teamService.AddUserToTeamAsync(teamId, userId);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            return StatusCode(result.Status, result);
        }
    }
}
