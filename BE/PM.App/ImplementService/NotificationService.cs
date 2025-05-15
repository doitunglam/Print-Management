using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PM.Application.Handle.HandleEmail;
using PM.Application.InterfaceService;
using PM.Domain.Entities;
using PM.Domain.InterfaceRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.ImplementService
{
    public class NotificationService : INotificationService
    {
        private readonly IBaseRepository<Notification> _notificationRepository;
        private readonly IBaseRepository<Design> _designRepository;
        private readonly IBaseRepository<Project> _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBaseRepository<Team> _teamRepository;
        private readonly IBaseRepository<PrintJobs> _printjobRepository;
        private readonly IEmailService _emailService;
        private readonly IBaseRepository<ConfirmEmail> _baseConfirmEmailRepository;
        private readonly IBaseRepository<Customer> _customerRepository;

        public NotificationService(IBaseRepository<Notification> notificationRepository,
            IBaseRepository<Project> projectRepository,
            IUserRepository userRepository,
            IHttpContextAccessor contextAccessor,
            IBaseRepository<Design> designRepository,
            IBaseRepository<Team> teamRepository,
            IBaseRepository<PrintJobs> printjobRepository, 
            IEmailService emailService,
            IBaseRepository<ConfirmEmail> baseConfirmEmailRepository,
            IBaseRepository<Customer> customerRepository)
        {
            _notificationRepository = notificationRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
            _designRepository = designRepository;
            _teamRepository = teamRepository;
            _printjobRepository = printjobRepository;
            _emailService = emailService;
            _baseConfirmEmailRepository = baseConfirmEmailRepository;
            _customerRepository = customerRepository;
        }
        public async Task SendNotificationAsync(long UserId, string MessageToSend)
        {
            ConfirmEmail confirmEmail = new ConfirmEmail
            {
                IsActive = true,
                ConfirmCode = MessageToSend,
                ExpiryTime = DateTime.Now.AddMinutes(1),
                IsConfirmed = false,
                UserId = UserId,
            };

            var Receiver = await _userRepository.GetUserById(UserId);
            confirmEmail = await _baseConfirmEmailRepository.CreateAsync(confirmEmail);
            var message = new EmailMessage(new string[] { Receiver.Email }, "Notification: ", $"{confirmEmail.ConfirmCode}");
            var responseMessage = _emailService.SendEmail(message);
        }

    }
}
