

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
using Step = PM.Domain.Entities.Step;

namespace PM.Application.ImplementService
{
    public class StepService(
                IBaseRepository<Step> stepRepository,

        IUserRepository userRepository,
        IHttpContextAccessor contextAccessor
         ) : IStepService
    {
        private readonly IBaseRepository<Step> _stepRepository = stepRepository;

        private readonly IUserRepository _userRepository = userRepository;
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

        public async Task<ResponseObject<Step>> UpdateStepAsync(long stepId, Request_CreateStep request)
        {
            try
            {
                //var currentUser = _contextAccessor.HttpContext.User;

                //if (!currentUser.Identity.IsAuthenticated)
                //{
                //    return new ResponseObject<Step>
                //    {
                //        Status = StatusCodes.Status401Unauthorized,
                //        Message = "Unauthorized user.",
                //        Data = null
                //    };
                //}

                if (request == null)
                {
                    return new ResponseObject<Step>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Invalid request.",
                        Data = null
                    };
                }

                var step = await _stepRepository.GetByIdAsync(stepId);
                if (step == null)
                {
                    return new ResponseObject<Step>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Step not found.",
                        Data = null
                    };
                }

                step.Name = request.Name;
                step.FulfilledAt = request.FulfilledAt;
                step.Status = request.Status;
                step.Note = request.Note;
                await _stepRepository.UpdateAsync(step);

                return new ResponseObject<Step>
                {
                    Data = step,
                    Message = "Step updated successfully.",
                    Status = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<Step>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<Step[]>> UpdateStepsAsync(Request_UpdateSteps request)
        {
            try
            {
                //var currentUser = _contextAccessor.HttpContext.User;

                //if (!currentUser.Identity.IsAuthenticated)
                //{
                //    return new ResponseObject<Step>
                //    {
                //        Status = StatusCodes.Status401Unauthorized,
                //        Message = "Unauthorized user.",
                //        Data = null
                //    };
                //}

                if (request == null)
                {
                    return new ResponseObject<Step[]>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Invalid request.",
                        Data = null
                    };
                }

                foreach (var requestStep in request.Steps)
                {
                    var step = await _stepRepository.GetByIdAsync(requestStep.Id);
                    if (step == null)
                    {
                        return new ResponseObject<Step[]>
                        {
                            Status = StatusCodes.Status404NotFound,
                            Message = "Step not found.",
                            Data = null
                        };
                    }

                    step.Name = requestStep.Name;
                    if (requestStep.Status == "Completed")
                    {
                        if (step.FulfilledAt == null)
                        {
                            step.FulfilledAt = DateTime.Now;
                        }
                    }
                    else
                    {
                        step.FulfilledAt = null;
                    }
                    step.Status = requestStep.Status;
                    if (requestStep.Status == "Others")
                    {
                        step.Note = requestStep.Note;
                    }
                    else
                    {
                        step.Note = null;
                    }
                    await _stepRepository.UpdateAsync(step);

                }

                return new ResponseObject<Step[]>
                {
                    Data = null,
                    Message = "Step updated successfully.",
                    Status = StatusCodes.Status200OK
                };

            }
            catch (Exception ex)
            {
                return new ResponseObject<Step[]>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
