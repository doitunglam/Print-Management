using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.ResponseModels.DataProjects
{
    public class DataResponseOrder
    {
        public long Id { get; set; }
        public long ProductID { get; set; }
        public long DesignID { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string DeliveryAddress { get; set; }
        public string PhoneNumber { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Step[] Steps { get; set; }
        public long? Rating { get; set; }
    }

}
