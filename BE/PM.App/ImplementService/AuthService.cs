using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PM.Application.InterfaceService;
using PM.Application.Payloads.Mappers;
using PM.Application.Payloads.RequestModels.UserRequests;
using PM.Application.Payloads.ResponseModels.DataUsers;
using PM.Application.Payloads.Responses;
using PM.Domain.Entities;
using PM.Domain.InterfaceRepositories;
using PM.Domain.Validation;
using BCryptNet = BCrypt.Net.BCrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PM.Application.Handle.HandleEmail;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using PM.Application.Payloads.ResponseModels.DataTeams;

namespace PM.Application.ImplementService
{
    public class AuthService : IAuthService
    {
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly UserConverter _userConverter;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IBaseRepository<Team> _baseTeamRepository;
        private readonly IEmailService _emailService;
        private readonly IBaseRepository<ConfirmEmail> _baseConfirmEmailRepository;
        private readonly IBaseRepository<Permissions> _basePermissionsRepository;
        private readonly IBaseRepository<Role> _baseRoleRepository;
        private readonly IBaseRepository<RefreshToken> _baseRefreshTokenRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        public AuthService(IBaseRepository<User> baseUserRepository, UserConverter userConverter, IConfiguration configuration, IUserRepository userRepository, IEmailService emailService, 
            IBaseRepository<ConfirmEmail> baseConfirmEmailRepository, IBaseRepository<Permissions> basePermissionsRepository, IBaseRepository<Role> baseRoleRepository, 
            IBaseRepository<RefreshToken> baseRefreshTokenRepository, IHttpContextAccessor contextAccessor, IBaseRepository<Team> baseTeamRepository)
        {
            _baseUserRepository = baseUserRepository;
            _userConverter = userConverter;
            _configuration = configuration;
            _userRepository = userRepository;
            _baseTeamRepository = baseTeamRepository;
            _emailService = emailService;
            _baseConfirmEmailRepository = baseConfirmEmailRepository;
            _basePermissionsRepository = basePermissionsRepository;
            _baseRoleRepository = baseRoleRepository;
            _baseRefreshTokenRepository = baseRefreshTokenRepository;
            _contextAccessor = contextAccessor;
        }

        public async Task<string> ConfirmRegisterAccount(string confirmCode)
        {
            try
            {
                var code = await _baseConfirmEmailRepository.GetAsync(x => x.ConfirmCode.Equals(confirmCode));
                if(code == null)
                {
                    return "Invalid cofirmation code.";
                }
                var user = await _baseUserRepository.GetAsync(x => x.Id == code.UserId);
                if(code.ExpiryTime < DateTime.Now.AddMinutes(14))
                {
                    return "The confirmation code has expired.";
                }
                code.IsConfirmed = true;
                await _baseUserRepository.UpdateAsync(user);
                await _baseConfirmEmailRepository.UpdateAsync(code);
                return "Successfully registered.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #region Private Methods
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            _ = int.TryParse(_configuration["JWT:TokenValidityInHours"], out int tokenValidityInHours);
            var expirationUTC = DateTime.Now.AddHours(tokenValidityInHours);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: expirationUTC,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            return token;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }
        #endregion

        public async Task<ResponseObject<DataResponseLogin>> GetJwtTokenAsync(User user)
        {
            var permissions = await _basePermissionsRepository.GetAllAsync(x => x.UserId == user.Id);


            var roles = await _baseRoleRepository.GetAllAsync();

            var authClaims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("UserName", user.UserName.ToString()),
                new Claim("Email", user.Email.ToString()),
                new Claim("PhoneNumber", user.PhoneNumber.ToString()),
            };

            foreach (var permission in permissions)
            {
                foreach (var role in roles)
                {
                    if (role.Id == permission.RoleId)
                    {
                        authClaims.Add(new Claim("Permission", role.RoleName));
                    }
                }
            }

            var userRoles = await _userRepository.GetRolesOfUserAsync(user);
            foreach (var item in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, item));
            }

            var jwtToken = GetToken(authClaims);
            var refreshToken = GenerateRefreshToken();
            _ = int.TryParse(_configuration["JWT:RefreshTokenValidity"], out int refreshTokenValidity);

            var rf = new RefreshToken
            {
                IsActive = true,
                ExpiryTime = DateTime.Now.AddHours(refreshTokenValidity),
                UserId = user.Id,
                Token = refreshToken
            };

            await _baseRefreshTokenRepository.CreateAsync(rf);

            return new ResponseObject<DataResponseLogin>
            {
                Status = 200,
                Message = "Create token successfully!",
                Data = new DataResponseLogin
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    RefreshToken = refreshToken,
                }
            };
        }


        public async Task<ResponseObject<DataResponseLogin>> Login(Request_Login request)
        {
            var user = await _baseUserRepository.GetAsync(x => x.UserName == request.UserName);
            if (user == null)
            {
                return new ResponseObject<DataResponseLogin>
                {
                    Status = 100,
                    Message = "Incorrect username.",
                    Data = null
                };
            }

            bool checkPass = BCryptNet.Verify(request.PassWord, user.Password);
            if (!checkPass)
            {
                return new ResponseObject<DataResponseLogin>
                {
                    Status = 400,
                    Message = "Incorrect password.",
                    Data = null
                };
            }

            var jwtTokenResponse = await GetJwtTokenAsync(user);
            return new ResponseObject<DataResponseLogin>
            {
                Status = 200,
                Message = "Login successfully!",
                Data = jwtTokenResponse.Data
            };
        }


        public async Task<ResponseObject<DataResponseUser>> GetUserInfoAsync()
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;
                if (currentUser == null || !currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "User is not authenticated.",
                        Data = null
                    };
                }

                var userIdClaim = currentUser.FindFirst("Id")?.Value;
                if (!long.TryParse(userIdClaim, out long userId))
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Invalid user ID in token.",
                        Data = null
                    };
                }

                var user = await _baseUserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "User not found.",
                        Data = null
                    };
                }

                var userRoles = await _userRepository.GetRolesOfUserAsync(user);

                var responseData = new DataResponseUser
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Roles = userRoles.ToList()
                };

                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "User information retrieved successfully.",
                    Data = responseData
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }


        public async Task<ResponseObject<DataResponseUser>> Register(Request_Register request)
        {
            try
            {
                if (!ValidateInput.IsValidEmail(request.Email))
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Invalid email!",
                        Data = null
                    };
                }

                if (!ValidateInput.IsValidPhoneNumber(request.PhoneNumber))
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Invalid phone number!",
                        Data = null
                    };
                }

                if (await _userRepository.GetUserByEmail(request.Email) != null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "This email address is already in use. Please provide a different one.",
                        Data = null
                    };
                }

                if (await _userRepository.GetUserByPhoneNumber(request.PhoneNumber) != null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "This phone number is already in use. Please provide a different one.",
                        Data = null
                    };
                }

                if (await _userRepository.GetUserByUsername(request.UserName) != null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "This username is already in use. Please choose a different one.",
                        Data = null
                    };
                }

                var user = new User
                {
                    Avatar = "https://static.vecteezy.com/system/resources/previews/009/292/244/original/default-avatar-icon-of-social-media-user-vector.jpg",
                    IsActive = true,
                    CreateTime = DateTime.Now,
                    DateOfBirth = request.DateOfBirth,
                    FullName = request.FullName,
                    Password = BCryptNet.HashPassword(request.Password),
                    UserName = request.UserName,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email 
                };

                user = await _baseUserRepository.CreateAsync(user);
                await _userRepository.AddRoleToUserAsync(user, new List<string> { "User" });
                ConfirmEmail confirmEmail = new ConfirmEmail
                {
                    IsActive = true,
                    ConfirmCode = GenerateCodeActive(),
                    ExpiryTime = DateTime.Now.AddMinutes(1),
                    IsConfirmed = false,
                    UserId = user.Id,   
                };
                confirmEmail = await _baseConfirmEmailRepository.CreateAsync(confirmEmail);
                var message = new EmailMessage(new string[] {request.Email}, "User information: ", $"Username: {request.UserName} Password: {request.Password}");
                var responseMessage = _emailService.SendEmail(message);
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "You have submitted a registration request. Please check your email to receive the confirmation code.",
                    Data = _userConverter.EntityToDTO(user)
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred: " + ex.Message,
                    Data = null
                };
            }
        }

        private string GenerateCodeActive()
        {
            string str = "QH_" + DateTime.Now.Ticks.ToString();
            return str;
        }

        public async Task<string> AddRolesToUser(long userId, List<string> roles)
        {
            var currentUser = _contextAccessor.HttpContext.User;
            try
            {
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return "Invalidate user.";
                }
                //if (!currentUser.IsInRole("Admin"))
                //{
                //    return "Only for admin";
                //}
                var user = await _baseUserRepository.GetByIdAsync(userId);
                if(user == null)
                {
                    return "Unknown user.";
                }
                await _userRepository.AddRoleToUserAsync(user, roles);
                return "Add roles successfully.";
            }
            catch (Exception ex) 
            {
                return ex.Message;
            }
        }

        public Task<string> DeleteRoles(long userId, List<string> roles)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseObject<List<DataResponseUser>>> GetAllUsersAsync()
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (currentUser == null || !currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<List<DataResponseUser>>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "User is not authenticated.",
                        Data = null
                    };
                }

                //if (!currentUser.IsInRole("Admin"))
                //{
                //    return new ResponseObject<List<DataResponseUser>>
                //    {
                //        Status = StatusCodes.Status403Forbidden,
                //        Message = "Only administrators can view all users.",
                //        Data = null
                //    };
                //}

                var users = await _baseUserRepository.GetAllAsync();
                if (users == null || !users.Any())
                {
                    return new ResponseObject<List<DataResponseUser>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "No users found.",
                        Data = null
                    };
                }

                var responseData = new List<DataResponseUser>();
                foreach (var user in users)
                {
                    var roles = await _userRepository.GetRolesOfUserAsync(user);
                    responseData.Add(new DataResponseUser
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        UserName = user.UserName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Roles = roles.ToList(),
                        CreateTime = user.CreateTime,
                        UpdateTime = user.UpdateTime,
                        TeamId = user.TeamId,
                    });
                }

                return new ResponseObject<List<DataResponseUser>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Users retrieved successfully.",
                    Data = responseData
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<List<DataResponseUser>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponseUser>> UpdateUserAsync(long userId, Request_UpdateUser request)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (currentUser == null || !currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "User is not authenticated.",
                        Data = null
                    };
                }

                if (!currentUser.IsInRole("Admin"))
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "Only administrators can update user information.",
                        Data = null
                    };
                }

                // Retrieve the user to update
                var existingUser = await _baseUserRepository.GetByIdAsync(userId);
                if (existingUser == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "User not found.",
                        Data = null
                    };
                }

                if(request.DateOfBirth == null)
                {
                    request.DateOfBirth = existingUser.DateOfBirth;
                }


                // Update user details based on the request payload
                existingUser.UserName = request.UserName;
                existingUser.FullName = request.FullName;
                existingUser.DateOfBirth = request.DateOfBirth;
                existingUser.Email = request.Email;
                existingUser.PhoneNumber = request.PhoneNumber;
                existingUser.UpdateTime = DateTime.UtcNow;
                existingUser.Password = existingUser.Password;


                await _userRepository.UpdateAsync(existingUser);

                var roles = await _userRepository.GetRolesOfUserAsync(existingUser);

                var responseData = new DataResponseUser
                {
                    Id = existingUser.Id,
                    UserName = existingUser.UserName,
                    FullName = existingUser.FullName,
                    Email = existingUser.Email,
                    PhoneNumber = existingUser.PhoneNumber,
                    Roles = roles.ToList(),
                    CreateTime = existingUser.CreateTime,
                    UpdateTime = existingUser.UpdateTime,
                    TeamId = existingUser.TeamId
                };

                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "User updated successfully.",
                    Data = responseData
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        //public async Task<ResponseObject<DataResponseUser>> GetUserForUpdateAsync(long userId)
        //{
        //    try
        //    {
        //        var user = await _baseUserRepository.GetByIdAsync(userId);
        //        if (user == null)
        //        {
        //            return new ResponseObject<DataResponseUser>
        //            {
        //                Status = StatusCodes.Status404NotFound,
        //                Message = "User not found.",
        //                Data = null
        //            };
        //        }

        //        var userRoles = await _userRepository.GetRolesOfUserAsync(user);

        //        return new ResponseObject<DataResponseUser>
        //        {
        //            Status = StatusCodes.Status200OK,
        //            Message = "User retrieved successfully.",
        //            Data = new DataResponseUser
        //            {
        //                Id = user.Id,
        //                UserName = user.UserName,
        //                Email = user.Email,
        //                FullName = user.FullName,
        //                PhoneNumber = user.PhoneNumber,
        //                DateOfBirth = user.DateOfBirth,
        //                TeamId = user.TeamId,
        //                Roles = userRoles.ToList(),
        //                CreateTime = user.CreateTime,
        //                UpdateTime = user.UpdateTime
        //            }
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseObject<DataResponseUser>
        //        {
        //            Status = StatusCodes.Status500InternalServerError,
        //            Message = ex.Message,
        //            Data = null
        //        };
        //    }
        //}


        public async Task<ResponseObject<DataResponseUser>> DeleteUserAsync(long userId)
        {
            try
            {
                var team = await _baseUserRepository.GetByIdAsync(userId);
                if (team == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "User not found.",
                        Data = null
                    };
                }

                var user = await _userRepository.GetUserById(userId);
                if (user.TeamId == null)
                {
                    user.TeamId = 0;
                }

                var userTeam = await _baseTeamRepository.GetByIdAsync(user.TeamId.Value);
                await _userRepository.DeleteAsync(userId);

                if(userTeam != null)
                {
                    userTeam.NumberOfMember--;
                    await _baseTeamRepository.UpdateAsync(userTeam);
                }


                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "User is deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponseUser>> ChangePassword(long userId, Request_ChangePassword request)
        {
            try
            {
                var user = await _baseUserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "User not found.",
                        Data = null
                    };
                }

                bool checkPass = BCryptNet.Verify(request.OldPassword, user.Password);
                if (!checkPass)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Incorrect old password.",
                        Data = null
                    };
                }

                if (!request.NewPassword.Equals(request.ConfirmPassword))
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Passwords do not match.",
                        Data = null
                    };
                }

                user.Password = BCryptNet.HashPassword(request.NewPassword);
                user.UpdateTime = DateTime.Now;
                await _baseUserRepository.UpdateAsync(user);

                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Password changed successfully.",
                    Data = _userConverter.EntityToDTO(user)
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

    }
}
