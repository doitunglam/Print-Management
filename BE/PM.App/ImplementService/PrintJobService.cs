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
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.ImplementService
{
    public class PrintJobService : IPrintJobService
    {
        private readonly IBaseRepository<Design> _designRepository;
        private readonly IBaseRepository<Project> _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBaseRepository<Team> _teamRepository;
        private readonly IBaseRepository<PrintJobs> _printjobRepository;

        public PrintJobService(
            IBaseRepository<Project> projectRepository,
            IUserRepository userRepository,
            IHttpContextAccessor contextAccessor,
            IBaseRepository<Design> designRepository,
            IBaseRepository<Team> teamRepository,
            IBaseRepository<PrintJobs> printjobRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
            _designRepository = designRepository;
            _teamRepository = teamRepository;
            _printjobRepository = printjobRepository;
        }

        public async Task<ResponseObject<List<DataResponsePrintjob>>> GetAllPrintJobsAsync()
        {
            try
            {
                var printJobs = await _printjobRepository.GetAllAsync();

                if (printJobs == null || !printJobs.Any())
                {
                    return new ResponseObject<List<DataResponsePrintjob>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "No print jobs found.",
                        Data = null
                    };
                }

                var response = printJobs.Select(printJob => new DataResponsePrintjob
                {
                    DesignId = printJob.DesignId,
                    PrintJobStatus = printJob.PrintJobStatus.ToString(),
                    Id = printJob.Id,
                }).ToList();

                return new ResponseObject<List<DataResponsePrintjob>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Print jobs retrieved successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<List<DataResponsePrintjob>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponsePrintjob>> UpdatePrintJobAsync(long id, Request_CreatePrintJob updateRequest)
        {
            try
            {
                var printJob = await _printjobRepository.GetByIdAsync(id);

                if (printJob == null)
                {
                    return new ResponseObject<DataResponsePrintjob>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Print job not found.",
                        Data = null
                    };
                }

                // Perform the necessary updates based on the request
                printJob.DesignId = updateRequest.DesignId;
                await _printjobRepository.UpdateAsync(printJob);

                var updatedResponse = new DataResponsePrintjob
                {
                    Id = printJob.Id,
                    DesignId = printJob.DesignId,
                    PrintJobStatus = printJob.PrintJobStatus.ToString()
                };

                return new ResponseObject<DataResponsePrintjob>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Print job updated successfully.",
                    Data = updatedResponse
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponsePrintjob>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponsePrintjob>> DeletePrintJobAsync(long printJobId)
        {
            try
            {
                var printJob = await _printjobRepository.GetByIdAsync(printJobId);
                if (printJob == null)
                {
                    return new ResponseObject<DataResponsePrintjob>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Print job not found.",
                        Data = null
                    };
                }

                await _printjobRepository.DeleteAsync(printJobId);

                return new ResponseObject<DataResponsePrintjob>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Print job is deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponsePrintjob>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }


        public async Task<ResponseObject<DataResponsePrintjob>> ConfirmDesignForPrintingAsync(Request_CreatePrintJob request)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponsePrintjob>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized user.",
                        Data = null
                    };
                }

                string userId = "";

                foreach (var claim in currentUser.Claims)
                {
                    if (claim.Type == "Id")
                    {
                        userId = claim.Value;
                        break;
                    }
                }

                if (request.DesignId <= 0)
                {
                    return new ResponseObject<DataResponsePrintjob>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Invalid Design ID.",
                        Data = null
                    };
                }

                Console.WriteLine($"Fetching design with ID: {request.DesignId}");
                var design = await _designRepository.GetByIdAsync(request.DesignId);
                if (design == null)
                {
                    return new ResponseObject<DataResponsePrintjob>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Design not found.",
                        Data = null
                    };
                }

                var project = await _projectRepository.GetByIdAsync(design.ProjectId);
                if (project == null)
                {
                    return new ResponseObject<DataResponsePrintjob>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Project not found.",
                        Data = null
                    };
                }

                var leaderId = Convert.ToInt64(userId);
                var leader = await _userRepository.GetUserById(leaderId);

                if (design.DesignStatus != DesignStatus.Approved)
                {
                    return new ResponseObject<DataResponsePrintjob>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Only approved designs.",
                        Data = null
                    };
                }

                var printJob = new PrintJobs
                {
                    DesignId = design.Id,
                    PrintJobStatus = PrintJobStatus.InProgress,
                };

                await _printjobRepository.CreateAsync(printJob);

                var response = new DataResponsePrintjob
                {
                    DesignId = printJob.DesignId,
                    PrintJobStatus = printJob.PrintJobStatus.ToString(),
                };

                return new ResponseObject<DataResponsePrintjob>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Design is confirmed, a printjob is created",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponsePrintjob>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }



    }
}
