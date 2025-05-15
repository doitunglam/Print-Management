using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.ResponseModels.DataProjects
{
    public class DataResponseNotification
    {
        public long UserId { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsSeen { get; set; }
    }
}
