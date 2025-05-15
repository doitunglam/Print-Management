using Microsoft.AspNetCore.Http;
using PM.Application.Payloads.RequestModels.ProjectRequests;
using PM.Application.Payloads.ResponseModels.DataProjects;
using PM.Application.Payloads.Responses;
using PM.Domain.Entities;
using PM.Domain.InterfaceRepositories;
using Step = PM.Domain.Entities.Step;


public class OrderService : IOrderService
{
    private readonly IBaseRepository<Order> _orderRepository;
    private readonly IBaseRepository<Step> _stepRepository;
    private readonly IBaseRepository<FlowTemplate> _flowTemplateRepository;
    private readonly IBaseRepository<StepFlowTemplate> _stepFlowTemplateRepository;

    public OrderService(IBaseRepository<Order> orderRepository, IBaseRepository<Step> stepRepository, IBaseRepository<FlowTemplate> flowtemplateRepository, IBaseRepository<StepFlowTemplate> stepFlowTemplateRepository)
    {
        _orderRepository = orderRepository;
        _stepRepository = stepRepository;
        _flowTemplateRepository = flowtemplateRepository;
        _stepFlowTemplateRepository = stepFlowTemplateRepository;
    }

    public async Task<ResponseObject<DataResponseOrder>> CreateOrderAsync(Request_CreateOrder request)
    {
        try
        {
            var order = new Order
            {
                ProductID = request.ProductID,
                DesignID = request.DesignID,
                OrderDate = request.OrderDate,
                Status = request.Status,
                DeliveryAddress = request.DeliveryAddress,
                PhoneNumber = request.PhoneNumber,
                CustomerID = request.CustomerID,
                CustomerName = request.CustomerName,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            var createdOrder = await _orderRepository.CreateAsync(order);

            var response = new DataResponseOrder
            {
                Id = createdOrder.Id,
                ProductID = createdOrder.ProductID,
                DesignID = createdOrder.DesignID,
                OrderDate = createdOrder.OrderDate,
                Status = createdOrder.Status,
                DeliveryAddress = createdOrder.DeliveryAddress,
                PhoneNumber = createdOrder.PhoneNumber,
                CustomerID = createdOrder.CustomerID,
                CustomerName = createdOrder.CustomerName,
                CreatedAt = createdOrder.CreatedAt,
                UpdatedAt = createdOrder.UpdatedAt ?? DateTime.MinValue
            };

            var flowTemplate = await _flowTemplateRepository.GetAsync(flow => flow.ProductId == createdOrder.ProductID);
            var stepFlowTemplates = await _stepFlowTemplateRepository.GetAllAsync((step => step.FlowTemplateId == flowTemplate.Id));
            var steps = stepFlowTemplates.Select(template => new Step
            {
                Order = createdOrder,
                Name = template.Name,
                Position = template.Position
            }).ToList();

            // Save all steps
            await _stepRepository.CreateAsync(steps);

            return new ResponseObject<DataResponseOrder>
            {
                Status = StatusCodes.Status201Created,
                Message = "Order created successfully",
                Data = response
            };
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine(ex);

            return new ResponseObject<DataResponseOrder>
            {
                Status = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseObject<DataResponseOrder>> EditOrderByIdAsync(long id, Request_CreateOrder request)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);

            order.Rating = request.Rating;

            await _orderRepository.UpdateAsync(order);

            return new ResponseObject<DataResponseOrder>
            {
                Status = StatusCodes.Status202Accepted,
                Message = "Order edited successfully",
                Data = null
            };
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine(ex);

            return new ResponseObject<DataResponseOrder>
            {
                Status = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseObject<List<DataResponseOrder>>> GetAllOrdersAsync(long? customerId)
    {
        try
        {
            Task<IQueryable<Order>> ordersQuery;
            if (customerId != null)
            {
                ordersQuery = _orderRepository.GetAllAsync(o => o.CustomerID == customerId);
            }
            else
            {
                ordersQuery = _orderRepository.GetAllAsync();
            }
            var orders = await ordersQuery;

            if (orders == null || !orders.Any())
            {
                return new ResponseObject<List<DataResponseOrder>>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "No orders found.",
                    Data = null
                };
            }

            var responseData = orders.Select(o => new DataResponseOrder
            {
                Id = o.Id,
                ProductID = o.ProductID,
                DesignID = o.DesignID,
                OrderDate = o.OrderDate,
                Status = o.Status,
                DeliveryAddress = o.DeliveryAddress,
                PhoneNumber = o.PhoneNumber,
                CustomerID = o.CustomerID,
                CustomerName = o.CustomerName,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt ?? DateTime.MinValue
            }).ToList();

            return new ResponseObject<List<DataResponseOrder>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Orders fetched successfully.",
                Data = responseData
            };
        }
        catch (Exception ex)
        {
            return new ResponseObject<List<DataResponseOrder>>
            {
                Status = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseObject<DataResponseOrder>> GetOrderByIdAsync(long id)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return new ResponseObject<DataResponseOrder>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Product not found.",
                    Data = null
                };
            }

            var steps = await _stepRepository.GetAllAsync(step => step.Order == order);

            foreach (var step in steps)
            {
                step.Order = null;
            }

            var response = new DataResponseOrder
            {
                Id = order.Id,
                ProductID = order.ProductID,
                DesignID = order.DesignID,
                OrderDate = order.OrderDate,
                Status = order.Status,
                DeliveryAddress = order.DeliveryAddress,
                PhoneNumber = order.PhoneNumber,
                CustomerID = order.CustomerID,
                CustomerName = order.CustomerName,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt ?? DateTime.MinValue,
                Steps = [.. steps],
                Rating = order.Rating
            };

            return new ResponseObject<DataResponseOrder>
            {
                Status = StatusCodes.Status200OK,
                Message = "Product retrieved successfully.",
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new ResponseObject<DataResponseOrder>
            {
                Status = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }
}
