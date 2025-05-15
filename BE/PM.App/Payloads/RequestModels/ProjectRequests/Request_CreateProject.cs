using PM.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace PM.Application.Payloads.RequestModels.ProjectRequests
{
    public class Request_CreateProject
    {
        public string ProjectName { get; set; }
        public string RequestDescriptionFromCustomer { get; set; }
        public DateTime StartDate { get; set; }
        public long EmployeeId { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public long CustomerId { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
    }

}
