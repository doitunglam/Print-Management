using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.RequestModels.ProjectRequests
{
    public class Request_CreateDesign
    {
        [Required]
        public long ProjectId { get; set; }
        [Required]
        public long DesignerId { get; set; }
        [Required]
        public string FilePath { get; set; }
    }
}
