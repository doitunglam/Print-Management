using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.RequestModels.ShippingRequests
{
    public class Request_CreateDelivery
    {
        public long ShippingMethodId { get; set; }
        public long CustomerId { get; set; }
        public long ProjectId { get; set; }
        public DateTime EstimateDeliveryTime { get; set; }
    }
}
