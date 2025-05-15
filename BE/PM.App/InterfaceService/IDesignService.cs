using PM.Application.Payloads.RequestModels.ProjectRequests;
using PM.Application.Payloads.ResponseModels.DataProjects;
using PM.Application.Payloads.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.InterfaceService
{
    public interface IDesignService
    {
        Task<ResponseObject<DataResponseDesign>> CreateDesignAsync(Request_CreateDesign request);
        Task<ResponseObject<List<DataResponseDesign>>> GetAllDesignsForAllProjectsAsync();
        Task<ResponseObject<List<DataResponseDesign>>> GetAllDesignAsync(long projectId);
        Task<ResponseObject<DataResponseDesign>> ApproveDesignAsync(long designId);
        Task<ResponseObject<DataResponseDesign>> RejectDesignAsync(long designId);

        Task<ResponseObject<DataResponseDesign>> UpdateDesignAsync(long designId, Request_CreateDesign request);

        Task<ResponseObject<DataResponseDesign>> DeleteDesignAsync(long designId);
    }
}
