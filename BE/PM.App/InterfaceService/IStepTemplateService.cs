using PM.Application.Payloads.RequestModels.ProjectRequests;
using PM.Application.Payloads.ResponseModels.DataProjects;
using PM.Application.Payloads.Responses;
using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.InterfaceService
{
    public interface IStepTemplateService
    {
        Task<ResponseObject<Domain.Entities.StepTemplate>> CreateStepTemplateAsync(Request_CreateStepTemplate request);

        Task<ResponseObject<List<Domain.Entities.StepTemplate>>> GetAllStepTemplateAsync();
        Task<ResponseObject<Domain.Entities.StepTemplate>> UpdateStepTemplateAsync(long stepTemplateId, Request_CreateStepTemplate request);
        Task<ResponseObject<Domain.Entities.StepTemplate>> DeleteStepTemplateAsync(long stepTemplateId);
    }
}
