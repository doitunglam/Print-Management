using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PM.Application.InterfaceService;
using PM.Application.Payloads.RequestModels.ProjectRequests;
using PM.Application.Payloads.ResponseModels.DataProjects;
using PM.Application.Payloads.Responses;
using PM.Domain.Entities;
using PM.Domain.InterfaceRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.ImplementService
{
    public class DesignService : IDesignService
    {
        private readonly IBaseRepository<Design> _designRepository;
        private readonly IBaseRepository<Project> _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBaseRepository<Team> _teamRepository;
        private readonly IBaseRepository<Notification> _notificationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public DesignService(
            IBaseRepository<Project> projectRepository,
            IUserRepository userRepository,
            IHttpContextAccessor contextAccessor,
            IBaseRepository<Design> designRepository,
            IBaseRepository<Team> teamRepository,
            IBaseRepository<Notification> notificationRepository,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
            _designRepository = designRepository;
            _teamRepository = teamRepository;
            _notificationRepository = notificationRepository;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public async Task<ResponseObject<DataResponseDesign>> CreateDesignAsync(Request_CreateDesign request)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponseDesign>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized user.",
                        Data = null
                    };
                }

                //if (!currentUser.IsInRole("Designer"))
                //{
                //    return new ResponseObject<DataResponseDesign>
                //    {
                //        Status = StatusCodes.Status403Forbidden,
                //        Message = "You must be a designer to add a design.",
                //        Data = null
                //    };
                //}

                if (request == null)
                {
                    return new ResponseObject<DataResponseDesign>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Invalid request.",
                        Data = null
                    };
                }

                var existingDesigns = await _designRepository.GetAllAsync(d => d.ProjectId == request.ProjectId);
                if (existingDesigns.Any(d => d.DesignStatus == DesignStatus.Approved))
                {
                    return new ResponseObject<DataResponseDesign>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "Designs for this project cannot be added as one has already been approved.", 
                        Data = null
                    };
                }

                    var design = new Design
                {
                    ProjectId = request.ProjectId,
                    DesignerId = request.DesignerId,
                    FilePath = request.FilePath,
                    DesignTime = DateTime.UtcNow,
                    DesignStatus = DesignStatus.Pending
                };

                await _designRepository.CreateAsync(design);

                var response = new DataResponseDesign
                {
                    ProjectId = design.ProjectId,
                    DesignerId = design.DesignerId,
                    FilePath = design.FilePath,
                    DesignTime = design.DesignTime,
                    DesignStatus = design.DesignStatus.ToString(),
                };

                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Design added successfully. Please wait for the leader to approve your design.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<List<DataResponseDesign>>> GetAllDesignsForAllProjectsAsync()
        {
            try
            {
                //var currentUser = _contextAccessor.HttpContext.User;

                //if (!currentUser.Identity.IsAuthenticated)
                //{
                //    return new ResponseObject<List<DataResponseDesign>>
                //    {
                //        Status = StatusCodes.Status401Unauthorized,
                //        Message = "Unauthorized user.",
                //        Data = null
                //    };
                //}

                var designs = await _designRepository.GetAllAsync();

                if (designs == null || !designs.Any())
                {
                    return new ResponseObject<List<DataResponseDesign>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "No designs found for any project.",
                        Data = null
                    };
                }

                var responseData = designs.Select(d => new DataResponseDesign
                {
                    ProjectId = d.ProjectId,
                    DesignerId = d.DesignerId,
                    FilePath = d.FilePath,
                    DesignTime = d.DesignTime,
                    DesignStatus = d.DesignStatus.ToString(),
                    ApproverId = d.ApproverId,
                    Id = d.Id,
                }).ToList();

                return new ResponseObject<List<DataResponseDesign>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Designs fetched successfully.",
                    Data = responseData
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<List<DataResponseDesign>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponseDesign>> UpdateDesignAsync(long designId, Request_CreateDesign request)
        {
            try
            {
                var design = await _designRepository.GetByIdAsync(designId);
                if (design == null)
                {
                    return new ResponseObject<DataResponseDesign>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Design not found.",
                        Data = null
                    };
                }

                // Update design properties
                design.ProjectId = request.ProjectId;
                design.DesignerId = request.DesignerId;
                design.FilePath = request.FilePath;

                await _designRepository.UpdateAsync(design);

                var response = new DataResponseDesign
                {
                    ProjectId = design.ProjectId,
                    DesignerId = design.DesignerId,
                    FilePath = design.FilePath,
                };

                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Design is updated successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponseDesign>> DeleteDesignAsync(long designId)
        {
            try
            {
                var design = await _designRepository.GetByIdAsync(designId);
                if (design == null)
                {
                    return new ResponseObject<DataResponseDesign>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Design not found.",
                        Data = null
                    };
                }

                await _designRepository.DeleteAsync(designId);

                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Design is deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }


        public async Task<ResponseObject<List<DataResponseDesign>>> GetAllDesignAsync(long projectId)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<List<DataResponseDesign>>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized user.",
                        Data = null
                    };
                }

                var designs = await _designRepository.GetAllAsync(d => d.ProjectId == projectId);

                if (designs == null || !designs.Any())
                {
                    return new ResponseObject<List<DataResponseDesign>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "No designs found for the specified project.",
                        Data = null
                    };
                }

                var responseData = designs.Select(d => new DataResponseDesign
                {
                    ProjectId = d.ProjectId,
                    DesignerId = d.DesignerId,
                    FilePath = d.FilePath,
                    DesignTime = d.DesignTime,
                    DesignStatus = d.DesignStatus.ToString(),
                    Id = d.Id,
                }).ToList();

                return new ResponseObject<List<DataResponseDesign>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Designs fetched successfully.",
                    Data = responseData
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<List<DataResponseDesign>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponseDesign>> ApproveDesignAsync(long designId)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponseDesign>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized user.",
                        Data = null
                    };
                }

                string userId = "";

                foreach (var claim in currentUser.Claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                    if(claim.Type == "Id")
                    {
                        userId = claim.Value;
                        break;
                    }
                }

                var design = await _designRepository.GetByIdAsync(designId);
                var project = await _projectRepository.GetByIdAsync(design.ProjectId);

                var approverId = Convert.ToInt64(userId);
                var approver = await _userRepository.GetUserById(approverId);

                if (!approverId.Equals(project.EmployeeId))
                {
                    return new ResponseObject<DataResponseDesign>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "Only project leaders can approve designs.",
                        Data = null
                    };
                }


                if (design == null)
                {
                    return new ResponseObject<DataResponseDesign>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Design not found.",
                        Data = null
                    };
                }

                if (project == null)
                {
                    return new ResponseObject<DataResponseDesign>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Project not found.",
                        Data = null
                    };
                }

                if (design.DesignStatus != DesignStatus.Pending)
                {
                    return new ResponseObject<DataResponseDesign>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Design is not in pending status.",
                        Data = null
                    };
                }

                design.DesignStatus = DesignStatus.Approved;
                design.ApproverId = approverId;
                project.ProjectStatus = ProjectStatus.DangIn;

                await _designRepository.UpdateAsync(design);
                await _projectRepository.UpdateAsync(project);

                var notification = new Notification
                {
                    UserId = design.DesignerId,
                    Content = "Your design has been approved.",
                    Link = $"/projects/{project.Id}/designs/{design.Id}",
                    CreateTime = DateTime.UtcNow,
                    IsSeen = false
                };

                await _notificationRepository.CreateAsync(notification);
                await _notificationService.SendNotificationAsync(notification.UserId, notification.Content);

                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Design approved successfully.",
                    Data = new DataResponseDesign
                    {
                        ProjectId = design.ProjectId,
                        DesignerId = design.DesignerId,
                        FilePath = design.FilePath,
                        DesignTime = design.DesignTime,
                        DesignStatus = design.DesignStatus.ToString(),
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponseDesign>> RejectDesignAsync(long designId)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponseDesign>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized user.",
                        Data = null
                    };
                }

                string userId = "";

                foreach (var claim in currentUser.Claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                    if (claim.Type == "Id")
                    {
                        userId = claim.Value;
                        break;
                    }
                }

                var design = await _designRepository.GetByIdAsync(designId);
                var project = await _projectRepository.GetByIdAsync(design.ProjectId);

                var approverId = Convert.ToInt64(userId);
                var approver = await _userRepository.GetUserById(approverId);

                if (!approverId.Equals(project.EmployeeId))
                {
                    return new ResponseObject<DataResponseDesign>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "Only project leaders can reject designs.",
                        Data = null
                    };
                }


                if (design == null)
                {
                    return new ResponseObject<DataResponseDesign>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Design not found.",
                        Data = null
                    };
                }

                if (project == null)
                {
                    return new ResponseObject<DataResponseDesign>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Project not found.",
                        Data = null
                    };
                }

                if (design.DesignStatus != DesignStatus.Pending)
                {
                    return new ResponseObject<DataResponseDesign>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Design is not in pending status.",
                        Data = null
                    };
                }

                design.DesignStatus = DesignStatus.Rejected;
                design.ApproverId = approverId;

                await _designRepository.UpdateAsync(design);

                var notification = new Notification
                {
                    UserId = design.DesignerId,
                    Content = "Your design has been rejected.",
                    Link = $"/projects/{project.Id}/designs/{design.Id}",
                    CreateTime = DateTime.UtcNow,
                    IsSeen = false
                };

                await _notificationRepository.CreateAsync(notification);
                await _notificationService.SendNotificationAsync(notification.UserId, notification.Content);

                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Design rejected.",
                    Data = new DataResponseDesign
                    {
                        ProjectId = design.ProjectId,
                        DesignerId = design.DesignerId,
                        FilePath = design.FilePath,
                        DesignTime = design.DesignTime,
                        DesignStatus = design.DesignStatus.ToString(),
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
