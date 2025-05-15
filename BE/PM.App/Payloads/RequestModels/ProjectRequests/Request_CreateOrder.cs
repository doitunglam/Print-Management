using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.RequestModels.ProjectRequests
{
    public class Request_CreateOrder
    {
        public long ProductID { get; set; }
        public long DesignID { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string DeliveryAddress { get; set; }
        public string PhoneNumber { get; set; }
        public int? Rating { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }

}
