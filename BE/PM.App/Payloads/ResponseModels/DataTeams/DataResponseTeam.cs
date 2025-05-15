using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.ResponseModels.DataTeams
{
    public class DataResponseTeam : DataResponseBase
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? NumberOfMember { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public long ManagerId { get; set; }
    }
}
