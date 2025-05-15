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
    public interface IResourceManagementService
    {
        Task<ResponseObject<DataResponseResourceForPrintJob>> CreateResourceForPrintJobAsync(Request_CreateResourceForPrintJob request);
        Task<ResponseObject<DataResponseResourcePropertyDetail>> CreateResourcePropertyDetailAsync(Request_CreateResourcePropertyDetail request);
        Task<ResponseObject<DataResponseResourceProperty>> CreateResourcePropertyAsync(Request_CreateResourceProperty request);
        Task<ResponseObject<DataResponseResource>> CreateResourceAsync(Request_CreateResource request);

        Task<ResponseObject<List<DataResponseResource>>> GetAllResourcesAsync();

        Task<ResponseObject<List<DataResponseResourceProperty>>> GetAllResourcePropertiesAsync();

        Task<ResponseObject<List<DataResponseResourcePropertyDetail>>> GetAllResourcePropertyDetailsAsync();
        Task<ResponseObject<DataResponseResourceForPrintJob>> UsingResourceForPrintJob(Request_CreateResourceForPrintJob request);

    }
}
