using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.ResponseModels.DataProjects
{
    public class DataResponseProject : DataResponseBase
    {
        public string ProjectName { get; set; }
        public string RequestDescriptionFromCustomer { get; set; }
        public DateTime StartDate { get; set; }
        public long EmployeeId { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public long CustomerId { get; set; }
        public string ProjectStatus { get; set; }
    }

}
