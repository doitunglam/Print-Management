using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.Entities
{
    public class Delivery : BaseEntity
    {
        public long ShippingMethodId { get; set; }
        public virtual ShippingMethod? ShippingMethod { get; set; }
        public long CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public long DeliverId { get; set; }
        public virtual User? Deliver { get; set; }
        public long ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime EstimateDeliveryTime { get; set; }
        public DateTime? ActualDeliveryTime { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
    }

    public enum DeliveryStatus
    {
        Shipping,
        Delivered
        // Add other statuses as needed
    }
}
