using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.Entities
{
    public class KeyPerformanceIndicators : BaseEntity
    {
        public int? EmployeeId { get; set; }
        public virtual User? Employee { get; set; }
        public string? IndicatorName { get; set; }
        public int? Target { get; set; }
        public int? ActuallyAchieved { get; set; }
        public Period? Period { get; set; }
        public bool? AchieveKPI { get; set; }
    }

    public enum Period
    {
        Thang,
        Quy,
        Nam
    }
}
