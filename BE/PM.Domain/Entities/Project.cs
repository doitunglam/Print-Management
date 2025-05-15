using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string ProjectName { get; set; }
        public string RequestDescriptionFromCustomer { get; set; }
        public DateTime StartDate { get; set; }
        public long EmployeeId { get; set; }
        public virtual User? Employee { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public long CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
    }

    public enum ProjectStatus
    {
        DangThietKe = 0,
        DangIn = 1,
        DaHoanThanh = 2
    }
}
