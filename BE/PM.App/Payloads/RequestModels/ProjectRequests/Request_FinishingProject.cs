using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.RequestModels.ProjectRequests
{
    public class Request_FinishingProject
    {
        public long PrintJobId { get; set; }
        public long ProjectId { get; set; }
    }
}
