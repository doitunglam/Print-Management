using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.Entities
{

    public class Order : BaseEntity
    {
        public long ProductID { get; set; }
        public long DesignID { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
        public string DeliveryAddress { get; set; }
        public string PhoneNumber { get; set; }
        public long? Rating { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }

}
