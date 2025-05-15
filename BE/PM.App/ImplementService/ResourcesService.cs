using Microsoft.AspNetCore.Http;
using PM.Application.InterfaceService;
using PM.Application.Payloads.RequestModels.ProjectRequests;
using PM.Application.Payloads.ResponseModels.DataProjects;
using PM.Application.Payloads.Responses;
using PM.Domain.Entities;
using PM.Domain.InterfaceRepositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.ImplementService
{
    public class ResourcesService : IResourcesService
    {
        private readonly IBaseRepository<Design> _designRepository;
        private readonly IBaseRepository<Project> _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBaseRepository<Resources> _resourceRepository;
        private readonly IBaseRepository<ResourceForPrintJob> _resourceForPrintJobRepository;
        private readonly IBaseRepository<ResourcePropertyDetail> _resourcePropertyDetailRepository;
        private readonly IBaseRepository<ResourceProperty> _resourcePropertyRepository;
        private readonly IBaseRepository<Notification> _notificationRepository;

        public ResourcesService(
            IBaseRepository<Project> projectRepository,
            IUserRepository userRepository,
            IHttpContextAccessor contextAccessor,
            IBaseRepository<Design> designRepository,
            IBaseRepository<Notification> notificationRepository,
            IBaseRepository<Resources> resourceRepository,
            IBaseRepository<ResourcePropertyDetail> resourcePropertyDetailRepository,
            IBaseRepository<ResourceForPrintJob> resourceForPrintJobRepository,
            IBaseRepository<ResourceProperty> resourcePropertyRepository
            )
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
            _designRepository = designRepository;
            _notificationRepository = notificationRepository;
            _resourceRepository = resourceRepository;
            _resourcePropertyRepository = resourcePropertyRepository;
            _resourceForPrintJobRepository = resourceForPrintJobRepository;
            _resourcePropertyRepository = resourcePropertyRepository;
        }
        public async Task<ResponseObject<DataResponseResource>> CreateResourceAsync(Request_CreateResource request)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponseResource>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized user.",
                        Data = null
                    };
                }

                if (!currentUser.IsInRole("Admin"))
                {
                    return new ResponseObject<DataResponseResource>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Only admins have rights.",
                        Data = null
                    };
                }
                var resources = new Resources
                {
                    ResourceName = request.ResourceName,
                    Image = request.Image,
                    ResourceType = request.ResourceType,
                    AvailableQuantity = request.AvailableQuantity,
                    ResourceStatus = request.ResourceStatus,
                };

                await _resourceRepository.CreateAsync(resources);

                var response = new DataResponseResource
                {
                    ResourceName = resources.ResourceName,
                    Image = resources.Image,
                    ResourceType = resources.ResourceType.ToString(),
                    AvailableQuantity = resources.AvailableQuantity,
                    ResourceStatus = resources.ResourceStatus.ToString(),
                };

                return new ResponseObject<DataResponseResource>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Resources created successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseResource>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
