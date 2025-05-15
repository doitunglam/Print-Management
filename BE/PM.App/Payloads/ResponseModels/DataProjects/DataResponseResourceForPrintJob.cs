using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.ResponseModels.DataProjects
{
    public class DataResponseResourceForPrintJob : DataResponseBase
    {
        public long ResourcePropertyDetailId { get; set; }
        public long PrintJobId { get; set; }
    }
}
