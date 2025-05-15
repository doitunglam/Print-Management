using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.RequestModels.ProjectRequests
{
    public class Request_CreateResourceProperty
    {
        public string ResourcePropertyName { get; set; }
        public long ResourceId { get; set; }
        public int Quantity { get; set; }
    }
}
