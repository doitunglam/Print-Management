using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.ResponseModels.DataProjects
{
    public class DataResponseFinishingProject : DataResponseBase
    {
        public long PrintJobId { get; set; }
        public long ProjectId { get; set; }
    }
}
