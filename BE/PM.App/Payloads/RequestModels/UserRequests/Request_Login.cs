using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Payloads.RequestModels.UserRequests
{
    public class Request_Login
    {
        [Required(ErrorMessage = "User is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string PassWord { get; set; }
    }
}
