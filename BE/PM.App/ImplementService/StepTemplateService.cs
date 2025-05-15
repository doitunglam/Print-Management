

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
    public class StepTemplateService(
        IBaseRepository<Domain.Entities.StepTemplate> stepTemplateRepository,
        IUserRepository userRepository,
        IHttpContextAccessor contextAccessor
         ) : IStepTemplateService
    {
        private readonly IBaseRepository<Domain.Entities.StepTemplate> _stepTemplateRepository = stepTemplateRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

        public async Task<ResponseObject<Domain.Entities.StepTemplate>> CreateStepTemplateAsync(Request_CreateStepTemplate request)
        {
            try
            {
                //var currentUser = _contextAccessor.HttpContext.User;

                //if (!currentUser.Identity.IsAuthenticated)
                //{
                //    return new ResponseObject<StepTemplate>
                //    {
                //        Status = StatusCodes.Status401Unauthorized,
                //        Message = "Unauthorized user.",
                //        Data = null
                //    };
                //}

                if (request == null)
                {
                    return new ResponseObject<Domain.Entities.StepTemplate>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Invalid request.",
                        Data = null
                    };
                }

                var stepTemplate = new Domain.Entities.StepTemplate
                {
                 Name = request.Name,
                };

                await _stepTemplateRepository.CreateAsync(stepTemplate);

                return new ResponseObject<Domain.Entities.StepTemplate>
                {
                    Data = stepTemplate, 
                    Message = "Created Successfully",
                    Status = 201
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<Domain.Entities.StepTemplate>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<List<Domain.Entities.StepTemplate>>> GetAllStepTemplateAsync()
        {
            try
            {
                var stepTemplates = await _stepTemplateRepository.GetAllAsync();

                if (stepTemplates == null || !stepTemplates.Any())
                {
                    return new ResponseObject<List<Domain.Entities.StepTemplate>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "No step found.",
                        Data = null
                    };
                }

                var responseData = stepTemplates.Select(stepTemplate => new Domain.Entities.StepTemplate
                {
                    Id = stepTemplate.Id,
                    Name = stepTemplate.Name
                }).ToList();

                return new ResponseObject<List<Domain.Entities.StepTemplate>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Steps retrieved successfully.",
                    Data = responseData
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<List<Domain.Entities.StepTemplate>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<Domain.Entities.StepTemplate>> UpdateStepTemplateAsync(long stepTemplateId, Request_CreateStepTemplate request)
        {
            try
            {
                var project = await _stepTemplateRepository.GetByIdAsync(stepTemplateId);
                if (project == null)
                {
                    return new ResponseObject<Domain.Entities.StepTemplate>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Step not found.",
                        Data = null
                    };
                }

                project.Name = request.Name;

                await _stepTemplateRepository.UpdateAsync(project);

                return new ResponseObject<Domain.Entities.StepTemplate>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Step is updated successfully.",
                    Data = project
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<Domain.Entities.StepTemplate>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<Domain.Entities.StepTemplate>> DeleteStepTemplateAsync(long stepTemplateId)
        {
            try
            {
                var stepTemplate = await _stepTemplateRepository.GetByIdAsync(stepTemplateId);
                if (stepTemplate == null)
                {
                    return new ResponseObject<Domain.Entities.StepTemplate>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Step not found.",
                        Data = null
                    };
                }

                await _stepTemplateRepository.DeleteAsync(stepTemplateId);

                return new ResponseObject<Domain.Entities.StepTemplate>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Step is deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<Domain.Entities.StepTemplate>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
