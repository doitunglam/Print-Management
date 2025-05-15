using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.RequestModels.ProjectRequests
{
    public class Request_CreateResource
    {
        public string ResourceName { get; set; }
        public string Image { get; set; }
        public ResourceType ResourceType { get; set; }
        public int AvailableQuantity { get; set; }
        public ResourceStatus ResourceStatus { get; set; } 
    }
}
