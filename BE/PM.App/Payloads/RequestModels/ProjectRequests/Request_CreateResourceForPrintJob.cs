using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.RequestModels.ProjectRequests
{
    public class Request_CreateResourceForPrintJob
    {
        public long ResourcePropertyDetailId { get; set; }
        public long PrintJobId { get; set; }
    }
}
