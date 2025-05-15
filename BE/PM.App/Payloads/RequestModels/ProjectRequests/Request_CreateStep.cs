using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.RequestModels.ProjectRequests
{
    public class Request_CreateStep
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime? FulfilledAt { get; set; }

        [Required]
        public virtual string? Status { get; set; }

        public virtual string? Note { get; set; }
    }
}
