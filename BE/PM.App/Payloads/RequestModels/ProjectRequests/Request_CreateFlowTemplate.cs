using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.RequestModels.ProjectRequests
{
    public class StepTemplate
    {
        [Required]
        public long StepTemplateId { get; set; }

        [Required]
        public int Position { get; set; }

    }
    public class Request_CreateFlowTemplate
    {
        [Required]
        public long ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public StepTemplate[] StepFlowTemplates { get; set; }
    }
}
