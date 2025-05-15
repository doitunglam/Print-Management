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
    public interface IPrintJobService
    {
        Task<ResponseObject<DataResponsePrintjob>> ConfirmDesignForPrintingAsync(Request_CreatePrintJob request);

        Task<ResponseObject<List<DataResponsePrintjob>>> GetAllPrintJobsAsync();

        Task<ResponseObject<DataResponsePrintjob>> DeletePrintJobAsync(long printJobId);

        Task<ResponseObject<DataResponsePrintjob>> UpdatePrintJobAsync(long id, Request_CreatePrintJob updateRequest);
    }
}
