using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.ResponseModels.DataProjects
{
    public class DataResponseResource: DataResponseBase
    {
        public string ResourceName { get; set; }
        public string Image { get; set; }
        public string ResourceType { get; set; }
        public int AvailableQuantity { get; set; }
        public string ResourceStatus { get; set; }
    }
}
