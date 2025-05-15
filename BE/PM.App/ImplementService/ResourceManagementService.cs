using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Asn1.Cmp;
using PM.Application.InterfaceService;
using PM.Application.Payloads.RequestModels.ProjectRequests;
using PM.Application.Payloads.ResponseModels.DataProjects;
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
    public class ResourceManagementService : IResourceManagementService
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
        private readonly IBaseRepository<PrintJobs> _printJobRepository;

        public ResourceManagementService(
            IBaseRepository<Project> projectRepository,
            IUserRepository userRepository,
            IHttpContextAccessor contextAccessor,
            IBaseRepository<Design> designRepository,
            IBaseRepository<Notification> notificationRepository,
            IBaseRepository<Resources> resourceRepository,
            IBaseRepository<ResourcePropertyDetail> resourcePropertyDetailRepository,
            IBaseRepository<ResourceForPrintJob> resourceForPrintJobRepository,
            IBaseRepository<ResourceProperty> resourcePropertyRepository,
            IBaseRepository<PrintJobs> printJobRepository
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
            _resourcePropertyDetailRepository = resourcePropertyDetailRepository;
            _printJobRepository = printJobRepository;
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

        public async Task<ResponseObject<List<DataResponseResource>>> GetAllResourcesAsync()
        {
            try
            {
                var resources = await _resourceRepository.GetAllAsync();

                if (resources == null || !resources.Any())
                {
                    return new ResponseObject<List<DataResponseResource>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "No resources found.",
                        Data = null
                    };
                }

                var response = resources.Select(r => new DataResponseResource
                {
                    ResourceName = r.ResourceName,
                    Image = r.Image,
                    ResourceType = r.ResourceType.ToString(),
                    AvailableQuantity = r.AvailableQuantity,
                    ResourceStatus = r.ResourceStatus.ToString(),
                    Id = r.Id,
                }).ToList();

                return new ResponseObject<List<DataResponseResource>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Resources retrieved successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<List<DataResponseResource>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponseResourceProperty>> CreateResourcePropertyAsync(Request_CreateResourceProperty request)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponseResourceProperty>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized user.",
                        Data = null
                    };
                }

                if (!currentUser.IsInRole("Admin"))
                {
                    return new ResponseObject<DataResponseResourceProperty>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Only admins have rights.",
                        Data = null
                    };
                }
                var resourceProperty = new ResourceProperty
                {
                    ResourcePropertyName = request.ResourcePropertyName,
                    Quantity = request.Quantity,
                    ResourceId = request.ResourceId,
                };

                await _resourcePropertyRepository.CreateAsync(resourceProperty);

                var response = new DataResponseResourceProperty
                {
                    ResourcePropertyName = resourceProperty.ResourcePropertyName,
                    Quantity = resourceProperty.Quantity,
                    ResourceId = resourceProperty.ResourceId,
                };

                return new ResponseObject<DataResponseResourceProperty>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Resources property created successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseResourceProperty>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<List<DataResponseResourceProperty>>> GetAllResourcePropertiesAsync()
        {
            try
            {
                var resourceProperties = await _resourcePropertyRepository.GetAllAsync();

                if (resourceProperties == null || !resourceProperties.Any())
                {
                    return new ResponseObject<List<DataResponseResourceProperty>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "No resource properties found.",
                        Data = null
                    };
                }

                var response = resourceProperties.Select(rp => new DataResponseResourceProperty
                {
                    ResourcePropertyName = rp.ResourcePropertyName,
                    Quantity = rp.Quantity,
                    ResourceId = rp.ResourceId,
                    Id = rp.Id,
                }).ToList();

                return new ResponseObject<List<DataResponseResourceProperty>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Resource properties retrieved successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<List<DataResponseResourceProperty>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }


        public async Task<ResponseObject<DataResponseResourcePropertyDetail>> CreateResourcePropertyDetailAsync(Request_CreateResourcePropertyDetail request)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponseResourcePropertyDetail>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorized user.",
                        Data = null
                    };
                }

                //if (!currentUser.IsInRole("Admin"))
                //{
                //    return new ResponseObject<DataResponseResourcePropertyDetail>
                //    {
                //        Status = StatusCodes.Status401Unauthorized,
                //        Message = "Only admins have rights.",
                //        Data = null
                //    };
                //}

                var resourcePropertyDetail = new ResourcePropertyDetail
                {
                    PropertyId = request.PropertyId,
                    PropertyDetailName = request.PropertyDetailName,
                    Price = request.Price,
                    Quantity = request.Quantity,
                };

                await _resourcePropertyDetailRepository.CreateAsync(resourcePropertyDetail);

                var response = new DataResponseResourcePropertyDetail
                {
                    PropertyId = resourcePropertyDetail.PropertyId,
                    PropertyDetailName = resourcePropertyDetail.PropertyDetailName,
                    Price = resourcePropertyDetail.Price,
                    Quantity = resourcePropertyDetail.Quantity,
                };

                return new ResponseObject<DataResponseResourcePropertyDetail>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Resources property detail number " + resourcePropertyDetail.Id.ToString() + "created successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseResourcePropertyDetail>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<List<DataResponseResourcePropertyDetail>>> GetAllResourcePropertyDetailsAsync()
        {
            try
            {
                var resourcePropertyDetails = await _resourcePropertyDetailRepository.GetAllAsync();

                if (resourcePropertyDetails == null || !resourcePropertyDetails.Any())
                {
                    return new ResponseObject<List<DataResponseResourcePropertyDetail>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "No resource property details found.",
                        Data = null
                    };
                }

                var response = resourcePropertyDetails.Select(rpd => new DataResponseResourcePropertyDetail
                {
                    PropertyId = rpd.PropertyId,
                    PropertyDetailName = rpd.PropertyDetailName,
                    Price = rpd.Price,
                    Quantity = rpd.Quantity,
                    Id = rpd.Id,
                }).ToList();

                return new ResponseObject<List<DataResponseResourcePropertyDetail>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Resource property details retrieved successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<List<DataResponseResourcePropertyDetail>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponseResourceForPrintJob>> CreateResourceForPrintJobAsync(Request_CreateResourceForPrintJob request)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponseResourceForPrintJob>
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

                var printJob = await _printJobRepository.GetByIdAsync(request.PrintJobId);
                var design = await _designRepository.GetByIdAsync(printJob.DesignId);
                var project = await _projectRepository.GetByIdAsync(design.ProjectId);

                var leaderId = Convert.ToInt64(userId);
                var leader = await _userRepository.GetUserById(leaderId);

                if (printJob == null || design == null || project == null || leader == null)
                {
                    return new ResponseObject<DataResponseResourceForPrintJob>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Incorrect information.",
                        Data = null
                    };
                }

                //if (!leaderId.Equals(project.EmployeeId))
                //{
                //    return new ResponseObject<DataResponseResourceForPrintJob>
                //    {
                //        Status = StatusCodes.Status403Forbidden,
                //        Message = "Only project leader can create resources to printjob.",
                //        Data = null
                //    };
                //}

                var resourceForPrintJob = new ResourceForPrintJob
                {
                    ResourcePropertyDetailId = request.ResourcePropertyDetailId,
                    PrintJobId = request.PrintJobId,
                };

                await _resourceForPrintJobRepository.CreateAsync(resourceForPrintJob);

                var response = new DataResponseResourceForPrintJob
                {
                    ResourcePropertyDetailId = resourceForPrintJob.ResourcePropertyDetailId,
                    PrintJobId = resourceForPrintJob.PrintJobId,
                };

                return new ResponseObject<DataResponseResourceForPrintJob>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Resources for print job created successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseResourceForPrintJob>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponseResourceForPrintJob>> UsingResourceForPrintJob(Request_CreateResourceForPrintJob request)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User;

                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponseObject<DataResponseResourceForPrintJob>
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

                var printJob = await _printJobRepository.GetByIdAsync(request.PrintJobId);
                var design = await _designRepository.GetByIdAsync(printJob.DesignId);
                var project = await _projectRepository.GetByIdAsync(design.ProjectId);

                var leaderId = Convert.ToInt64(userId);
                var leader = await _userRepository.GetUserById(leaderId);

                if (!leaderId.Equals(project.EmployeeId))
                {
                    return new ResponseObject<DataResponseResourceForPrintJob>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "Only project leader",
                        Data = null
                    };
                }
                var resourceForPrintJob = new ResourceForPrintJob
                {
                    ResourcePropertyDetailId = request.ResourcePropertyDetailId,
                    PrintJobId = request.PrintJobId,
                };

                await _resourceForPrintJobRepository.CreateAsync(resourceForPrintJob);

                if (resourceForPrintJob == null)
                {
                    return new ResponseObject<DataResponseResourceForPrintJob>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Resource for print job not found.",
                        Data = null
                    };
                }

                var resourcePropertyDetail = await _resourcePropertyDetailRepository.GetByIdAsync(resourceForPrintJob.ResourcePropertyDetailId);
                var resourceProperty = await _resourcePropertyRepository.GetByIdAsync(resourcePropertyDetail.PropertyId);
                var resources = await _resourceRepository.GetByIdAsync(resourceProperty.ResourceId);

                if (resourcePropertyDetail == null || resourceProperty == null || resources == null)
                {
                    return new ResponseObject<DataResponseResourceForPrintJob>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Resources not found.",
                        Data = null
                    };
                }

                if(resources.ResourceType == ResourceType.MayIn)
                {
                    if(resources.ResourceStatus == ResourceStatus.CanBaoTri)
                    {
                        return new ResponseObject<DataResponseResourceForPrintJob>
                        {
                            Status = StatusCodes.Status451UnavailableForLegalReasons,
                            Message = "Resources need to be maintained.",
                            Data = null
                        };
                    }
                    else if (resources.ResourceStatus == ResourceStatus.CanNhapThem)
                    {
                        return new ResponseObject<DataResponseResourceForPrintJob>
                        {
                            Status = StatusCodes.Status409Conflict,
                            Message = "This resource status is not for this resource type.",
                            Data = new DataResponseResourceForPrintJob
                            {
                                ResourcePropertyDetailId = resourceForPrintJob.ResourcePropertyDetailId,
                                PrintJobId = resourceForPrintJob.PrintJobId,
                            }
                        };
                    }
                    else 
                    {
                        return new ResponseObject<DataResponseResourceForPrintJob>
                        {
                            Status = StatusCodes.Status202Accepted,
                            Message = "Resources are used.",
                            Data = null
                        };
                    }
                }
                else
                {
                    if (resources.ResourceStatus == ResourceStatus.CanBaoTri)
                    {
                        return new ResponseObject<DataResponseResourceForPrintJob>
                        {                      
                            Status = StatusCodes.Status409Conflict,
                            Message = "This resource status is not for this resource type.",
                            Data = null
                        };
                    }
                    else if (resources.ResourceStatus == ResourceStatus.CanNhapThem)
                    {
                        return new ResponseObject<DataResponseResourceForPrintJob>
                        {
                            Status = StatusCodes.Status451UnavailableForLegalReasons,
                            Message = "Resources need to be supplied.",
                            Data = null
                        };
                    }
                    else
                    {
                        if(resources.AvailableQuantity < resourcePropertyDetail.Quantity)
                        {
                            return new ResponseObject<DataResponseResourceForPrintJob>
                            {
                                Status = StatusCodes.Status451UnavailableForLegalReasons,
                                Message = "Resources need to be supplied.",
                                Data = null
                            };
                        }
                        else if (resources.AvailableQuantity == resourcePropertyDetail.Quantity)
                        {
                            resources.ResourceStatus = ResourceStatus.CanNhapThem;
                            resources.AvailableQuantity = 0;
                            await _resourceRepository.UpdateAsync(resources);

                            return new ResponseObject<DataResponseResourceForPrintJob>
                            {
                                Status = StatusCodes.Status202Accepted,
                                Message = "Resources are used.",
                                Data = new DataResponseResourceForPrintJob
                                {
                                    ResourcePropertyDetailId = resourceForPrintJob.ResourcePropertyDetailId,
                                    PrintJobId = resourceForPrintJob.PrintJobId,
                                }
                            };
                        }
                        else
                        {
                            resources.AvailableQuantity = resources.AvailableQuantity - resourcePropertyDetail.Quantity;
                            resourceProperty.Quantity = resourceProperty.Quantity - resourcePropertyDetail.Quantity;
                            await _resourceRepository.UpdateAsync(resources);
                            await _resourcePropertyRepository.UpdateAsync(resourceProperty);

                            return new ResponseObject<DataResponseResourceForPrintJob>
                            {
                                Status = StatusCodes.Status202Accepted,
                                Message = "Resources are used.",
                                Data = new DataResponseResourceForPrintJob
                                {
                                    ResourcePropertyDetailId = resourceForPrintJob.ResourcePropertyDetailId,
                                    PrintJobId = resourceForPrintJob.PrintJobId,
                                }
                            };
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseResourceForPrintJob>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
