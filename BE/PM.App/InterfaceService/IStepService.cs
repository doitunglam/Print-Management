using PM.Application.Payloads.RequestModels.ProjectRequests;
using PM.Application.Payloads.ResponseModels.DataProjects;
using PM.Application.Payloads.Responses;
using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Step = PM.Domain.Entities.Step;

namespace PM.Application.InterfaceService
{
    public interface IStepService
    {
        Task<ResponseObject<Step>> UpdateStepAsync(long stepId, Request_CreateStep request);
        Task<ResponseObject<Step[]>> UpdateStepsAsync(Request_UpdateSteps request);
    }
}
