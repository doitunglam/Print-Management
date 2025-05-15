using PM.Application.Payloads.RequestModels.ProjectRequests;
using PM.Application.Payloads.RequestModels.ShippingRequests;
using PM.Application.Payloads.ResponseModels.DataProjects;
using PM.Application.Payloads.ResponseModels.DataShipping;
using PM.Application.Payloads.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.InterfaceService
{
    public interface IShippingService
    {
        Task<ResponseObject<DataResponseDelivery>> CreateDeliveryAsync(Request_CreateDelivery request);

        Task<ResponseObject<List<DataResponseDelivery>>> GetAllDeliveriesAsync();
        Task<ResponseObject<DataResponseDelivery>> DeliveryStatusUpdate(long DeliveryId);
    }
}
