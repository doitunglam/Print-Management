﻿using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.RequestModels.ProjectRequests
{
    public class Request_CreateStepTemplate
    {
        [Required]
        public string Name { get; set; }
    }
}
