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
    public interface IProjectService
    {
        Task<ResponseObject<DataResponseProject>> CreateProjectAsync(Request_CreateProject request);

        Task<ResponseObject<List<DataResponseProject>>> GetAllProjectsAsync();
        Task<ResponseObject<DataResponseProject>> UpdateProjectAsync(long projectId, Request_CreateProject request);
        Task<ResponseObject<DataResponseProject>> DeleteProjectAsync(long projectId);
        Task<ResponseObject<DataResponseFinishingProject>> ConfirmFinishingProjectAsync(Request_FinishingProject request);

        Task<ResponseObject<List<DataResponseCustomer>>> GetAllCustomersAsync();
    }
}
