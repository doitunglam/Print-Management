using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PM.Application.Handle.HandleEmail;
using PM.Application.InterfaceService;
using PM.Application.Payloads.Mappers;
using PM.Application.Payloads.RequestModels;
using PM.Application.Payloads.RequestModels.ProjectRequests;
using PM.Application.Payloads.ResponseModels;
using PM.Application.Payloads.ResponseModels.DataProjects;
using PM.Application.Payloads.ResponseModels.DataTeams;
using PM.Application.Payloads.Responses;
using PM.Domain.Entities;
using PM.Domain.InterfaceRepositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PM.Application.ImplementService
{
    public class ProjectService : IProjectService
    {
        private readonly IBaseRepository<Design> _designRepository;
        private readonly IBaseRepository<Project> _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBaseRepository<Team> _teamRepository;
        private readonly IBaseRepository<PrintJobs> _printjobRepository;
        private readonly IEmailService _emailService;
        private readonly IBaseRepository<ConfirmEmail> _baseConfirmEmailRepository;
        private readonly IBaseRepository<Customer> _customerRepository;

        public ProjectService(
            IBaseRepository<Project> projectRepository,
            IUserRepository userRepository,
            IHttpContextAccessor contextAccessor,
            IBaseRepository<Design> designRepository,
            IBaseRepository<Team> teamRepository,
            IBaseRepository<PrintJobs> printjobRepository, IEmailService emailService,
            IBaseRepository<ConfirmEmail> baseConfirmEmailRepository,
            IBaseRepository<Customer> customerRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
            _designRepository = designRepository;
            _teamRepository = teamRepository;
            _printjobRepository = printjobRepository;
            _emailService = emailService;
            _baseConfirmEmailRepository = baseConfirmEmailRepository;
            _customerRepository = customerRepository;
        }

        public async Task<ResponseObject<DataResponseProject>> CreateProjectAsync(Request_CreateProject request)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponseProject>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized user.",
                        Data = null
                    };
                }

                if (!currentUser.IsInRole("Employee"))
                {
                    return new ResponseObject<DataResponseProject>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "You must be an employee to create a project.",
                        Data = null
                    };
                }

                var user = await _userRepository.GetUserById(request.EmployeeId);

                if (!(user.TeamId == 2))
                {
                    return new ResponseObject<DataResponseProject>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "You must be  in the sales team to create a project.",
                        Data = null
                    };
                }

                if (request == null)
                {
                    return new ResponseObject<DataResponseProject>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Invalid request.",
                        Data = null
                    };
                }

                var project = new Project
                {
                    ProjectName = request.ProjectName,
                    RequestDescriptionFromCustomer = request.RequestDescriptionFromCustomer,
                    StartDate = request.StartDate,
                    ExpectedEndDate = request.ExpectedEndDate,
                    EmployeeId = request.EmployeeId,
                    CustomerId = request.CustomerId,
                    ProjectStatus = request.ProjectStatus,
                };

                await _projectRepository.CreateAsync(project);

                var response = new DataResponseProject
                {
                    ProjectName = project.ProjectName,
                    RequestDescriptionFromCustomer = project.RequestDescriptionFromCustomer,
                    StartDate = project.StartDate,
                    ExpectedEndDate = project.ExpectedEndDate,
                    EmployeeId = project.EmployeeId,
                    CustomerId = project.CustomerId,
                    ProjectStatus = project.ProjectStatus.ToString(),
                };

                return new ResponseObject<DataResponseProject>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Project created successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseProject>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponseFinishingProject>> ConfirmFinishingProjectAsync(Request_FinishingProject request)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponseFinishingProject>
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



                var project = await _projectRepository.GetByIdAsync(request.ProjectId);
                var printJob = await _printjobRepository.GetByIdAsync(request.PrintJobId);

                var leaderId = Convert.ToInt64(userId);
                var leader = await _userRepository.GetUserById(leaderId);

                //if (!leaderId.Equals(project.EmployeeId) && !currentUser.IsInRole("Admin"))
                //{
                //    return new ResponseObject<DataResponseProject>
                //    {
                //        Status = StatusCodes.Status403Forbidden,
                //        Message = "Only project leaders and admins.",
                //        Data = null
                //    };
                //}

                if (project == null)
                {
                    return new ResponseObject<DataResponseFinishingProject>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Project not found.",
                        Data = null
                    };
                }

                if (printJob == null)
                {
                    return new ResponseObject<DataResponseFinishingProject>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Print job not found.",
                        Data = null
                    };
                }

                printJob.PrintJobStatus = PrintJobStatus.Completed;
                project.ProjectStatus = ProjectStatus.DaHoanThanh;

                await _printjobRepository.UpdateAsync(printJob);
                await _projectRepository.UpdateAsync(project);

                var response = new DataResponseFinishingProject
                {
                    ProjectId = project.Id,
                    PrintJobId = printJob.Id
                };

                ConfirmEmail confirmEmail = new ConfirmEmail
                {
                    IsActive = true,
                    ConfirmCode = GenerateCodeActive(),
                    ExpiryTime = DateTime.Now.AddMinutes(1),
                    IsConfirmed = false,
                    UserId = project.EmployeeId
                };

                var Customer = await _customerRepository.GetByIdAsync(project.CustomerId);
                confirmEmail = await _baseConfirmEmailRepository.CreateAsync(confirmEmail);
                var message = new EmailMessage(new string[] { Customer.Email }, "Notification: ", $"{confirmEmail.ConfirmCode}");
                var responseMessage = _emailService.SendEmail(message);

                return new ResponseObject<DataResponseFinishingProject>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Print job is completed and the project is fisnished. Email is sent to customer.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseFinishingProject>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<List<DataResponseProject>>> GetAllProjectsAsync()
        {
            try
            {
                var projects = await _projectRepository.GetAllAsync();

                if (projects == null || !projects.Any())
                {
                    return new ResponseObject<List<DataResponseProject>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "No projects found.",
                        Data = null
                    };
                }

                var responseData = projects.Select(project => new DataResponseProject
                {
                    ProjectName = project.ProjectName,
                    RequestDescriptionFromCustomer = project.RequestDescriptionFromCustomer,
                    StartDate = project.StartDate,
                    ExpectedEndDate = project.ExpectedEndDate,
                    EmployeeId = project.EmployeeId,
                    CustomerId = project.CustomerId,
                    ProjectStatus = project.ProjectStatus.ToString(),
                    Id = project.Id,
                }).ToList();

                return new ResponseObject<List<DataResponseProject>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Projects retrieved successfully.",
                    Data = responseData
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<List<DataResponseProject>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<List<DataResponseCustomer>>> GetAllCustomersAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();

                if (customers == null || !customers.Any())
                {
                    return new ResponseObject<List<DataResponseCustomer>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "No customers found.",
                        Data = null
                    };
                }

                var responseData = customers.Select(customer => new DataResponseCustomer
                {
                    FullName = customer.FullName,
                    PhoneNumber = customer.PhoneNumber,
                    Address = customer.Address,
                    Email = customer.Email,
                    Id = customer.Id,
                }).ToList();

                return new ResponseObject<List<DataResponseCustomer>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Customers retrieved successfully.",
                    Data = responseData
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<List<DataResponseCustomer>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }


        private string GenerateCodeActive()
        {
            string str = "Print job is completed and the project is fisnished. Code: " + DateTime.Now.ToString();
            return str;
        }

        public async Task<ResponseObject<DataResponseProject>> UpdateProjectAsync(long projectId, Request_CreateProject request)
        {
            try
            {
                var project = await _projectRepository.GetByIdAsync(projectId);
                if (project == null)
                {
                    return new ResponseObject<DataResponseProject>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Project not found.",
                        Data = null
                    };
                }

                project.ProjectName = request.ProjectName;
                project.RequestDescriptionFromCustomer = request.RequestDescriptionFromCustomer;
                project.EmployeeId = request.EmployeeId;
                project.StartDate = request.StartDate;
                project.ExpectedEndDate = request.ExpectedEndDate;
                project.CustomerId = request.CustomerId;
                project.ProjectStatus = request.ProjectStatus;

                await _projectRepository.UpdateAsync(project);

                var response = new DataResponseProject
                {
                    ProjectName = project.ProjectName,
                    RequestDescriptionFromCustomer = project.RequestDescriptionFromCustomer,
                    EmployeeId = project.EmployeeId,
                    StartDate = project.StartDate,
                    ExpectedEndDate = project.ExpectedEndDate,
                    CustomerId = project.CustomerId,
                    ProjectStatus = project.ProjectStatus.ToString()
                };

                return new ResponseObject<DataResponseProject>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Project is updated successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseProject>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponseProject>> DeleteProjectAsync(long projectId)
        {
            try
            {
                var project = await _projectRepository.GetByIdAsync(projectId);
                if (project == null)
                {
                    return new ResponseObject<DataResponseProject>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Project not found.",
                        Data = null
                    };
                }

                await _projectRepository.DeleteAsync(projectId);

                return new ResponseObject<DataResponseProject>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Project is deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseProject>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }

        }
    }
}
