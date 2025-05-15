using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.RequestModels.NewFolder
{
    public class Request_Create
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public int? NumberOfMember { get; set; } = 0;
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        [Required]
        public long ManagerId { get; set; }
    }
}
