using Microsoft.AspNetCore.Http;
using PM.Application.Handle.HandleEmail;
using PM.Application.InterfaceService;
using PM.Application.Payloads.RequestModels.ShippingRequests;
using PM.Application.Payloads.ResponseModels.DataProjects;
using PM.Application.Payloads.ResponseModels.DataShipping;
using PM.Application.Payloads.Responses;
using PM.Domain.Entities;
using PM.Domain.InterfaceRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.ImplementService
{
    public class ShippingService : IShippingService
    {
        private readonly IBaseRepository<Project> _projectRepository;
        private readonly IBaseRepository<Customer> _customerRepository;
        private readonly IBaseRepository<User> _userBaseRepository;
        private readonly IBaseRepository<Team> _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBaseRepository<Notification> _notificationRepository;
        private readonly IBaseRepository<Delivery> _deliveryRepository;
        private readonly IBaseRepository<ShippingMethod> _shippingMethodRepository;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IBaseRepository<ConfirmEmail> _baseConfirmEmailRepository;
        public ShippingService(
            IBaseRepository<Project> projectRepository,
            IUserRepository userRepository,
            IHttpContextAccessor contextAccessor,
            IBaseRepository<Notification> notificationRepository,
            IBaseRepository<Delivery> deliveryRepository,
            IBaseRepository<Customer> customerRepository,
            IBaseRepository<ShippingMethod> shippingMethodRepository,
            INotificationService notificationService,
            IBaseRepository<User> userBaseRepository,
            IBaseRepository<Team> teamRepository,
            IEmailService emailService,
            IBaseRepository<ConfirmEmail> baseConfirmEmailRepository
            )
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
            _notificationRepository = notificationRepository;
            _deliveryRepository = deliveryRepository;
            _notificationService = notificationService;
            _shippingMethodRepository = shippingMethodRepository;
            _customerRepository = customerRepository;
            _userBaseRepository = userBaseRepository;
            _teamRepository = teamRepository;
            _emailService = emailService;
            _baseConfirmEmailRepository = baseConfirmEmailRepository;
        }

        public async Task<ResponseObject<DataResponseDelivery>> CreateDeliveryAsync(Request_CreateDelivery request)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized user.",
                        Data = null
                    };
                }

                string userId = "";

                foreach (var claim in currentUser.Claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                    if (claim.Type == "Id")
                    {
                        userId = claim.Value;
                        break;
                    }
                }

                var leaderId = Convert.ToInt64(userId);
                var leader = await _userRepository.GetUserById(leaderId);
                var team = await _teamRepository.GetByIdAsync(3); // shipping team id = 3

                if (leaderId != team.ManagerId)
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "Only shipping team leaders can create delivery.",
                        Data = null
                    };
                }

                var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
                var users = await _userBaseRepository.GetAllAsync();
                var teams = await _teamRepository.GetAllAsync();

                var shippingTeam = teams.FirstOrDefault(t => t.Name == "Shipping");
                var shippers = users.Where(u => u.TeamId == shippingTeam.Id);
                var shippingDeliveries = await _deliveryRepository.GetAllAsync();

                var delivery = new Delivery
                {
                    ShippingMethodId = request.ShippingMethodId,
                    CustomerId = request.CustomerId,
                    ProjectId = request.ProjectId,
                    DeliveryAddress = customer.Address,
                    EstimateDeliveryTime = request.EstimateDeliveryTime,
                    DeliveryStatus = DeliveryStatus.Shipping,
                    DeliverId = leaderId,
                };

                await _deliveryRepository.CreateAsync(delivery);

                foreach (var sd in shippingDeliveries)
                {
                    
                    if (AreAddressesSimilar(sd.DeliveryAddress, customer.Address))
                    {
                        delivery.DeliverId = sd.DeliverId;
                        break;
                    }
                    else
                    {
                        var differentshipper = shippers.FirstOrDefault(s => s.Id != sd.DeliverId);
                        delivery.DeliverId = differentshipper.Id;
                        break;
                    }
                }


                await _deliveryRepository.UpdateAsync(delivery);

                var response = new DataResponseDelivery
                {
                    ShippingMethodId = delivery.ShippingMethodId,
                    CustomerId = delivery.CustomerId,
                    DeliverId = delivery.DeliverId,
                    ProjectId = delivery.ProjectId,
                    DeliveryAddress = delivery.DeliveryAddress,
                    EstimateDeliveryTime = delivery.EstimateDeliveryTime,
                    DeliveryStatus = delivery.DeliveryStatus.ToString()
                };

                return new ResponseObject<DataResponseDelivery>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Delivery created successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseDelivery>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public double CalculateJaccardSimilarity(string address1, string address2)
        {
            var set1 = new HashSet<string>(address1.Split(' '));
            var set2 = new HashSet<string>(address2.Split(' '));

            var intersection = set1.Intersect(set2).Count();
            var union = set1.Union(set2).Count();

            return (double)intersection / union;
        }

        public bool AreAddressesSimilar(string address1, string address2, double threshold = 0.5)
        {
            return CalculateJaccardSimilarity(address1, address2) >= threshold;
        }

        public async Task<ResponseObject<List<DataResponseDelivery>>> GetAllDeliveriesAsync()
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<List<DataResponseDelivery>>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized user.",
                        Data = null
                    };
                }

                string userId = "";

                foreach (var claim in currentUser.Claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                    if (claim.Type == "Id")
                    {
                        userId = claim.Value;
                        break;
                    }
                }

                var userIdToLong = Convert.ToInt64(userId);
                var user = await _userRepository.GetUserById(userIdToLong);

                if (user == null)
                {
                    return new ResponseObject<List<DataResponseDelivery>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "User not found.",
                        Data = null
                    };
                }

                if (!user.TeamId.HasValue)
                {
                    return new ResponseObject<List<DataResponseDelivery>>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "User is not assigned to any team.",
                        Data = null
                    };
                }

                var teamId = user.TeamId.Value;
                var team = await _teamRepository.GetByIdAsync(3);

                //if (team == null || teamId != 3)
                //{
                //    return new ResponseObject<List<DataResponseDelivery>>
                //    {
                //        Status = StatusCodes.Status403Forbidden,
                //        Message = "Only users in the Shipping team can view deliveries.",
                //        Data = null
                //    };
                //}

                var deliveries = await _deliveryRepository.GetAllAsync();

                var response = deliveries.Select(d => new DataResponseDelivery
                {
                    ShippingMethodId = d.ShippingMethodId,
                    CustomerId = d.CustomerId,
                    DeliverId = d.DeliverId,
                    ProjectId = d.ProjectId,
                    DeliveryAddress = d.DeliveryAddress,
                    ActualDeliveryTime = d.ActualDeliveryTime,
                    EstimateDeliveryTime = d.EstimateDeliveryTime,
                    DeliveryStatus = d.DeliveryStatus.ToString(),
                    Id = d.Id,
                }).ToList();

                return new ResponseObject<List<DataResponseDelivery>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Deliveries retrieved successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<List<DataResponseDelivery>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponseDelivery>> DeliveryStatusUpdate(long DeliveryId)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized user.",
                        Data = null
                    };
                }

                string userId = "";

                foreach (var claim in currentUser.Claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                    if (claim.Type == "Id")
                    {
                        userId = claim.Value;
                        break;
                    }
                }
                var userIdToLong = Convert.ToInt64(userId);
                var user = await _userRepository.GetUserById(userIdToLong);
                if (user.TeamId.HasValue)
                {
                    long teamId = user.TeamId.Value;
                    var team = await _teamRepository.GetByIdAsync(teamId);
                    var shippingLeader = await _userRepository.GetUserById(team.ManagerId);
                    var delivery = await _deliveryRepository.GetByIdAsync(DeliveryId);
                    var customer = await _customerRepository.GetByIdAsync(delivery.CustomerId);
                    var project = await _projectRepository.GetByIdAsync(delivery.ProjectId);
                    var projectLeader = await _userRepository.GetUserById(project.EmployeeId);

                    if(delivery == null)
                    {
                        return new ResponseObject<DataResponseDelivery>
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = "Bad request.",
                            Data = null
                        };
                    }

                    if (!currentUser.IsInRole("Shipper"))
                    {
                        return new ResponseObject<DataResponseDelivery>
                        {
                            Status = StatusCodes.Status403Forbidden,
                            Message = "Only for shippers.",
                            Data = null
                        };
                    }



                    delivery.DeliveryStatus = DeliveryStatus.Delivered;
                    delivery.ActualDeliveryTime = DateTime.UtcNow;

                    await _deliveryRepository.UpdateAsync(delivery);

                    var response = new DataResponseDelivery
                    {
                        ShippingMethodId = delivery.ShippingMethodId,
                        CustomerId = delivery.CustomerId,
                        DeliverId = delivery.DeliverId,
                        ProjectId = delivery.ProjectId,
                        DeliveryAddress = delivery.DeliveryAddress,
                        EstimateDeliveryTime = delivery.EstimateDeliveryTime,
                        DeliveryStatus = delivery.DeliveryStatus.ToString()
                    };

                    var notification = new Notification
                    {
                        UserId = user.Id,
                        Content = "Product is delivered successfully.",
                        Link = $"...",
                        CreateTime = DateTime.UtcNow,
                        IsSeen = false
                    };

                    await _notificationRepository.CreateAsync(notification);
                    await _notificationService.SendNotificationAsync(projectLeader.Id, notification.Content);
                    await _notificationService.SendNotificationAsync(shippingLeader.Id, notification.Content);

                    ConfirmEmail confirmEmail = new ConfirmEmail
                    {
                        IsActive = true,
                        ConfirmCode = "Product is delivered successfully.",
                        ExpiryTime = DateTime.Now.AddMinutes(1),
                        IsConfirmed = false,
                        UserId = customer.Id,
                    };

                    var Receiver = await _customerRepository.GetByIdAsync(customer.Id);
                    confirmEmail = await _baseConfirmEmailRepository.CreateAsync(confirmEmail);
                    var message = new EmailMessage(new string[] { Receiver.Email }, "Notification: ", $"{confirmEmail.ConfirmCode}");
                    var responseMessage = _emailService.SendEmail(message);

                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Product delivered successfully.",
                        Data = new DataResponseDelivery
                        {
                            ShippingMethodId = response.ShippingMethodId,
                            CustomerId = response.CustomerId,
                            DeliverId = response.DeliverId,
                            ProjectId = response.ProjectId,
                            DeliveryAddress = response.DeliveryAddress,
                            EstimateDeliveryTime = response.EstimateDeliveryTime,
                            ActualDeliveryTime = response.ActualDeliveryTime,
                            DeliveryStatus = response.DeliveryStatus.ToString()
                        }
                    };
                }
                else
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Bad request.",
                        Data = null
                    };
                }
                
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseDelivery>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

    }
}
