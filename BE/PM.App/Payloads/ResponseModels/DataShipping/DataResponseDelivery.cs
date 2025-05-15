using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.ResponseModels.DataShipping
{
    public class DataResponseDelivery
    {
        public long ShippingMethodId { get; set; }
        public long CustomerId { get; set; }
        public long DeliverId { get; set; }
        public long ProjectId { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime EstimateDeliveryTime { get; set; }
        public DateTime? ActualDeliveryTime { get; set; }
        public string DeliveryStatus { get; set; }

        public long Id { get; set; }
    }
}
