

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

namespace PM.Application.ImplementService
{
    public class FlowTemplateService(
        IBaseRepository<FlowTemplate> flowTemplateRepository,
        IBaseRepository<StepFlowTemplate> stepFlowTemplateRepository,
        IBaseRepository<Domain.Entities.StepTemplate> stepTemplateRepository,
        IUserRepository userRepository,
        IHttpContextAccessor contextAccessor
         ) : IFlowTemplateService
    {
        private readonly IBaseRepository<FlowTemplate> _flowTemplateRepository = flowTemplateRepository;
        private readonly IBaseRepository<StepFlowTemplate> _stepFlowTemplateRepository = stepFlowTemplateRepository;
        private readonly IBaseRepository<Domain.Entities.StepTemplate> _stepTemplateRepository = stepTemplateRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

        public async Task<ResponseObject<FlowTemplate>> CreateFlowTemplateService(Request_CreateFlowTemplate request)
        {
            try
            {
                //var currentUser = _contextAccessor.HttpContext.User;

                //if (!currentUser.Identity.IsAuthenticated)
                //{
                //    return new ResponseObject<FlowTemplate>
                //    {
                //        Status = StatusCodes.Status401Unauthorized,
                //        Message = "Unauthorized user.",
                //        Data = null
                //    };
                //}

                if (request == null)
                {
                    return new ResponseObject<FlowTemplate>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Invalid request.",
                        Data = null
                    };
                }

                var flowTemplate = new FlowTemplate
                {
                    ProductId = request.ProductId,
                    Name = request.Name,
                };

                await _flowTemplateRepository.CreateAsync(flowTemplate);

                // Tạo các StepFlowTemplate kết nối n-n
                foreach (var step in request.StepFlowTemplates)
                {
                    var stepTemplate = await _stepTemplateRepository.GetByIdAsync(step.StepTemplateId);
                    var stepFlow = new StepFlowTemplate
                    {
                        FlowTemplateId = flowTemplate.Id,
                        StepTemplateId = stepTemplate.Id, // Nếu StepTemplateId là string
                        Position = step.Position,
                        Name = stepTemplate.Name
                    };

                    await _stepFlowTemplateRepository.CreateAsync(stepFlow);
                }


                return new ResponseObject<FlowTemplate>
                {
                    Data = flowTemplate,
                    Message = "Created Successfully",
                    Status = 201
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<FlowTemplate>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<List<FlowTemplate>>> GetAllFlowTemplateAsync()
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<List<FlowTemplate>>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized user.",
                        Data = null
                    };
                }

                var flowTemplates = await _flowTemplateRepository.GetAllAsync();

                if (flowTemplates == null || !flowTemplates.Any())
                {
                    return new ResponseObject<List<FlowTemplate>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "No flow found.",
                        Data = null
                    };
                }
                
                var responseData = flowTemplates.ToList();

                foreach (var flow in responseData)
                {
                    var step = await _stepFlowTemplateRepository.GetAllAsync((step => step.FlowTemplateId == flow.Id));

                    flow.StepFlowTemplates = [.. step];
                }

                    return new ResponseObject<List<FlowTemplate>>
                {
                    Data = responseData,
                    Message = "Flows retrieved successfully.",
                    Status = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<List<FlowTemplate>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<FlowTemplate>> UpdateFlowTemplateAsync(long flowTemplateId, Request_CreateFlowTemplate request)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<FlowTemplate>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized user.",
                        Data = null
                    };
                }

                if (request == null)
                {
                    return new ResponseObject<FlowTemplate>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Invalid request.",
                        Data = null
                    };
                }

                var flowTemplate = await _flowTemplateRepository.GetByIdAsync(flowTemplateId);
                if (flowTemplate == null)
                {
                    return new ResponseObject<FlowTemplate>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Flow not found.",
                        Data = null
                    };
                }

                // Cập nhật name và productId
                flowTemplate.Name = request.Name;
                flowTemplate.ProductId = request.ProductId;

                await _stepFlowTemplateRepository.DeleteAsync(s => s.FlowTemplateId == flowTemplateId);

                // Thêm mới các bước từ request
                foreach (var step in request.StepFlowTemplates)
                {
                    var stepTemplate = await _stepTemplateRepository.GetByIdAsync(step.StepTemplateId);
                    var stepFlow = new StepFlowTemplate
                    {
                        FlowTemplateId = flowTemplate.Id,
                        StepTemplateId = stepTemplate.Id, // Nếu StepTemplateId là string
                        Position = step.Position,
                        Name = stepTemplate.Name
                    };

                    await _stepFlowTemplateRepository.CreateAsync(stepFlow);
                }

                await _flowTemplateRepository.UpdateAsync(flowTemplate);

                return new ResponseObject<FlowTemplate>
                {
                    Data = flowTemplate,
                    Message = "Flow updated successfully.",
                    Status = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<FlowTemplate>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<FlowTemplate>> DeleteFlowTemplateAsync(long flowTemplateId)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<FlowTemplate>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized user.",
                        Data = null
                    };
                }

                var flowTemplate = await _flowTemplateRepository.GetByIdAsync(flowTemplateId);
                if (flowTemplate == null)
                {
                    return new ResponseObject<FlowTemplate>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Flow not found.",
                        Data = null
                    };
                }

                await _stepFlowTemplateRepository.DeleteAsync(s => s.FlowTemplateId == flowTemplateId);

                await _flowTemplateRepository.DeleteAsync(flowTemplateId);

                return new ResponseObject<FlowTemplate>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Flow deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<FlowTemplate>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
