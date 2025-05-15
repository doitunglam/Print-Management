using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.ResponseModels.DataProjects
{
    public class DataResponseDesign : DataResponseBase
    {
        public long ProjectId { get; set; }
        public long DesignerId { get; set; }
        public string FilePath { get; set; }
        public DateTime DesignTime { get; set; }
        public string DesignStatus { get; set; }
        public long? ApproverId { get; set; }

    }
}
