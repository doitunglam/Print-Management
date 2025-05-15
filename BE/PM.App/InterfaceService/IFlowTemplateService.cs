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
    public interface IFlowTemplateService
    {
        Task<ResponseObject<FlowTemplate>> CreateFlowTemplateService(Request_CreateFlowTemplate request);
        Task<ResponseObject<List<FlowTemplate>>> GetAllFlowTemplateAsync();
        Task<ResponseObject<FlowTemplate>> UpdateFlowTemplateAsync(long flowTemplateSerivice, Request_CreateFlowTemplate request);
        Task<ResponseObject<FlowTemplate>> DeleteFlowTemplateAsync(long flowTemplateSerivice);
    }
}
