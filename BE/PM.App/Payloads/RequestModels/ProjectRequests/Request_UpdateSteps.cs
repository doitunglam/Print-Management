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
    public class Step
    {
        [Required]
        public long Id { get; set; }
        
        public string Name { get; set; }

        public DateTime? FulfilledAt { get; set; }

        public virtual string? Status { get; set; }

        public virtual string? Note { get; set; }
    }

    public class Request_UpdateSteps
    {
        [Required]
        public Step[] Steps { get; set; }
    }
}
