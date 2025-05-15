using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.ResponseModels.DataProjects
{
    public class DataResponsePrintjob : DataResponseBase
    {
        public long DesignId { get; set; }
        public string PrintJobStatus { get; set; }
    }
}
