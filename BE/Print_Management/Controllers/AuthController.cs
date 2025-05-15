using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Application.Constants;
using PM.Application.InterfaceService;
using PM.Application.Payloads.RequestModels.UserRequests;

namespace Print_Management.Controllers
{
    [Route(Constant.DefaultValue.DEFAULT_CONTROLLER_ROUTE)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] Request_Register request)
        {
            return Ok(await _authService.Register(request));
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmRegisterAccount(string confirmCode)
        {
            return Ok(await _authService.ConfirmRegisterAccount(confirmCode));
        }

        [HttpPost]
        public async Task<IActionResult> Login(Request_Login request)
        {
            return Ok(await _authService.Login(request));
        }


        [HttpGet("userinfo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserInfo()
        {
            var result = await _authService.GetUserInfoAsync();
            if (result.Status != StatusCodes.Status200OK)
            {
                return StatusCode(result.Status, result.Message);
            }
            return Ok(result.Data);
        }

        [HttpGet("all-users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _authService.GetAllUsersAsync();

            if (result.Status != StatusCodes.Status200OK)
            {
                return StatusCode(result.Status, result.Message);
            }
            return Ok(result.Data);
        }


        [HttpPut("update-user/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(long userId, [FromBody] Request_UpdateUser request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.UpdateUserAsync(userId, request);

            if (result.Status != StatusCodes.Status200OK)
            {
                return StatusCode(result.Status, result.Message);
            }
            return Ok(result.Data);
        }

        [HttpDelete("delete-user/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(long userId)
        {
            var result = await _authService.DeleteUserAsync(userId);

            if (result.Status != StatusCodes.Status200OK)
            {
                return StatusCode(result.Status, result.Message);
            }
            return Ok(new { message = result.Message });
        }


        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword([FromBody] Request_ChangePassword request)
        {
            long id = long.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _authService.ChangePassword(id, request);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            return StatusCode(result.Status, result);
        }

        [HttpPost("{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddRolesToUser([FromRoute] long userId, [FromBody] List<string> roles)
        {
            if (roles == null || !roles.Any())
            {
                return BadRequest("Roles must be provided.");
            }

            var result = await _authService.AddRolesToUser(userId, roles);
            if (result == "Add roles successfully.")
            {
                return Ok(new { message = result });
            }
            return BadRequest(new { error = result });
        }
    }
}
