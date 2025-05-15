using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Application.ImplementService;
using PM.Application.InterfaceService;
using PM.Application.Payloads.RequestModels.ProjectRequests;
using PM.Application.Payloads.ResponseModels.DataProjects;
using PM.Application.Payloads.Responses;

namespace Print_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IDesignService _designService;
        private readonly IPrintJobService _printJobService;
        private readonly IResourceManagementService _resourceManagementService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IStepTemplateService _stepTemplateService;
        private readonly IFlowTemplateService _flowTemplateService;
        private readonly IStepService _stepService;

        public ProjectController(IProjectService projectService, IDesignService designService, IPrintJobService printJobService
            , IResourceManagementService resourceManagementService, IProductService productService, IOrderService orderService, IStepTemplateService stepTemplateService, IFlowTemplateService flowTemplateService, IStepService stepService)
        {
            _projectService = projectService;
            _designService = designService;
            _printJobService = printJobService;
            _resourceManagementService = resourceManagementService;
            _productService = productService;
            _orderService = orderService;
            _stepTemplateService = stepTemplateService;
            _flowTemplateService = flowTemplateService;
            _stepService = stepService;
        }

        [HttpPost("CreateProject")]
        public async Task<IActionResult> CreateProject([FromBody] Request_CreateProject request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _projectService.CreateProjectAsync(request);

            if (response.Status == StatusCodes.Status201Created)
            {
                return CreatedAtAction(nameof(CreateProject), new { id = response.Data.Id }, response.Data);
            }

            return StatusCode(response.Status, response.Message);
        }

        [HttpGet("all-projects")]
        public async Task<IActionResult> GetAllProjects()
        {
            var result = await _projectService.GetAllProjectsAsync();
            if (result.Status != StatusCodes.Status200OK)
            {
                return StatusCode(result.Status, result.Message);
            }
            return Ok(result.Data);
        }

        [HttpPut("UpdateProject/{id}")]
        public async Task<IActionResult> UpdateProject(long id, [FromBody] Request_CreateProject request)
        {
            try
            {
                var response = await _projectService.UpdateProjectAsync(id, request);
                if (response.Status == StatusCodes.Status400BadRequest)
                {
                    return BadRequest(response.Message); // Return specific error message
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpDelete("DeleteProject/{projectId}")]
        public async Task<IActionResult> DeleteProjectAsync(long projectId)
        {
            var result = await _projectService.DeleteProjectAsync(projectId);

            return StatusCode(result.Status, result);
        }

        [HttpPost("AddDesign")]
        public async Task<IActionResult> AddDesign([FromBody] Request_CreateDesign request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _designService.CreateDesignAsync(request);

            if (response.Status == StatusCodes.Status201Created)
            {
                return CreatedAtAction(nameof(AddDesign), new { id = response.Data.Id }, response.Data);
            }

            return StatusCode(response.Status, response.Message);
        }

        [HttpGet("all-designs")]
        public async Task<IActionResult> GetAllDesignsForAllProjects()
        {
            var response = await _designService.GetAllDesignsForAllProjectsAsync();

            //if (response.Status == StatusCodes.Status401Unauthorized)
            //{
            //    return Unauthorized(new { response.Message });
            //}

            if (response.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(new { response.Message });
            }

            if (response.Status == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { response.Message });
            }

            return Ok(response.Data);
        }


        [HttpGet("all-designs-belong-to-project/{projectId}")]
        public async Task<IActionResult> GetAllDesigns(long projectId)
        {
            var response = await _designService.GetAllDesignAsync(projectId);

            if (response.Status == StatusCodes.Status401Unauthorized)
            {
                return Unauthorized(new { response.Message });
            }

            if (response.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(new { response.Message });
            }

            if (response.Status == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { response.Message });
            }

            return Ok(response.Data);
        }


        [HttpPost("ApproveDesign/{designId}")]
        public async Task<IActionResult> ApproveDesign(long designId)
        {
            var response = await _designService.ApproveDesignAsync(designId);

            if (response.Status == StatusCodes.Status200OK)
            {
                return Ok(response);
            }

            return StatusCode(response.Status, response.Message);
        }

        [HttpPost("RejectDesign/{designId}")]
        public async Task<IActionResult> RejectDesign(long designId)
        {
            var response = await _designService.RejectDesignAsync(designId);

            if (response.Status == StatusCodes.Status200OK)
            {
                return Ok(response);
            }

            return StatusCode(response.Status, response.Message);
        }

        [HttpPut("UpdateDesign/{designId}")]
        public async Task<IActionResult> UpdateDesign(long designId, [FromBody] Request_CreateDesign request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return bad request if model state is invalid
            }

            try
            {
                var response = await _designService.UpdateDesignAsync(designId, request); // Service method to handle design update

                if (response.Status == StatusCodes.Status404NotFound)
                {
                    return NotFound(new { response.Message }); // Return not found if the design doesn't exist
                }

                if (response.Status == StatusCodes.Status200OK)
                {
                    return Ok(response.Data); // Return updated design data if success
                }

                return StatusCode(response.Status, response.Message); // Return other status if needed
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return internal server error if an exception is caught
            }
        }

        [HttpDelete("DeleteDesign/{designId}")]
        public async Task<IActionResult> DeleteDesign(long designId)
        {
            try
            {
                var response = await _designService.DeleteDesignAsync(designId); // Service method to delete the design

                if (response.Status == StatusCodes.Status404NotFound)
                {
                    return NotFound(new { response.Message }); // Return not found if the design to delete is not found
                }

                if (response.Status == StatusCodes.Status200OK)
                {
                    return Ok(response.Message); // Return success message if design was deleted
                }

                return StatusCode(response.Status, response.Message); // Return other status if necessary
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return internal server error if an exception occurs
            }
        }

        // Method to test Create Product
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] Request_CreateProduct request)
        {
            var response = await _productService.CreateProductAsync(request);
            if (response.Status == StatusCodes.Status201Created)
            {
                return CreatedAtAction(nameof(GetProductById), new { id = response.Data.Id }, response);
            }
            return StatusCode(response.Status, response);
        }

        // Method to test Get All Products
        [HttpGet("getAllProduct")]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _productService.GetAllProductsAsync();
            return StatusCode(response.Status, response);
        }

        // Method to test Get Product by ID
        [HttpGet("getProductBy/{id}")]
        public async Task<IActionResult> GetProductById(long id)
        {
            var response = await _productService.GetProductByIdAsync(id);
            return StatusCode(response.Status, response);
        }

        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] Request_CreateOrder request)
        {
            var response = await _orderService.CreateOrderAsync(request);
            return StatusCode(response.Status, response);
        }


        [HttpGet]
        [Route("Order/{id}")]
        public async Task<IActionResult> GetOrder(long id)
        {
            var response = await _orderService.GetOrderByIdAsync(id);
            return StatusCode(response.Status, response);
        }

        [HttpPost]
        [Route("Order/{id}")]
        public async Task<IActionResult> EditOrder(long id, [FromBody] Request_CreateOrder request)
        {
            var response = await _orderService.EditOrderByIdAsync(id, request);
            return StatusCode(response.Status, response);
        }

        [HttpGet("getAllOrder")]
        public async Task<IActionResult> GetAllOrders(long? customerId)
        {
            var response = await _orderService.GetAllOrdersAsync(customerId);
            return StatusCode(response.Status, response);
        }




        [HttpPost("ConfirmDesign-for-printing")]
        public async Task<IActionResult> ConfirmDesignForPrintingAsync([FromBody] Request_CreatePrintJob request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _printJobService.ConfirmDesignForPrintingAsync(request);

            if (response.Status == StatusCodes.Status201Created)
            {
                return Ok(response.Data);
            }

            return StatusCode(response.Status, response.Message);
        }

        [HttpPut("UpdatePrintJob/{printJobId}")]
        public async Task<IActionResult> UpdatePrintJobAsync(long printJobId, [FromBody] Request_CreatePrintJob updateRequest)
        {
            try
            {
                var response = await _printJobService.UpdatePrintJobAsync(printJobId, updateRequest);

                if (response.Status == StatusCodes.Status404NotFound)
                {
                    return NotFound(new { message = response.Message });
                }
                if (response.Status == StatusCodes.Status500InternalServerError)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = response.Message });
                }

                return Ok(new { message = response.Message, data = response.Data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpDelete("DeletePrintJob/{printJobId}")]
        public async Task<IActionResult> DeletePrintJobAsync(long printJobId)
        {
            try
            {
                var response = await _printJobService.DeletePrintJobAsync(printJobId);

                if (response.Status == StatusCodes.Status404NotFound)
                {
                    return NotFound(new { message = response.Message });
                }

                if (response.Status == StatusCodes.Status500InternalServerError)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = response.Message });
                }

                return Ok(new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost("CreateResources")]
        public async Task<IActionResult> CreateResources([FromBody] Request_CreateResource request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _resourceManagementService.CreateResourceAsync(request);

            if (response.Status == StatusCodes.Status201Created)
            {
                return CreatedAtAction(nameof(CreateResources), new { id = response.Data.Id }, response.Data);
            }

            return StatusCode(response.Status, response.Message);
        }

        [HttpGet("get-all-resources")]
        public async Task<IActionResult> GetAllResources()
        {

            var result = await _resourceManagementService.GetAllResourcesAsync();

            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }

            return StatusCode(result.Status, result);
        }

        [HttpPost("CreateResource-property")]
        public async Task<IActionResult> CreateResourceProperty([FromBody] Request_CreateResourceProperty request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _resourceManagementService.CreateResourcePropertyAsync(request);

            if (response.Status == StatusCodes.Status201Created)
            {
                return CreatedAtAction(nameof(CreateResourceProperty), new { id = response.Data.Id }, response.Data);
            }

            return StatusCode(response.Status, response.Message);
        }

        [HttpGet("GetAllResourceProperties")]
        public async Task<ActionResult<ResponseObject<List<DataResponseResourceProperty>>>> GetAllResourcePropertiesAsync()
        {
            try
            {
                var result = await _resourceManagementService.GetAllResourcePropertiesAsync();
                if (result.Status == StatusCodes.Status404NotFound)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseObject<List<DataResponseResourceProperty>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPost("CreateResource-property-detail")]
        public async Task<IActionResult> CreateResourcePropertyDetail([FromBody] Request_CreateResourcePropertyDetail request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _resourceManagementService.CreateResourcePropertyDetailAsync(request);

            if (response.Status == StatusCodes.Status201Created)
            {
                return CreatedAtAction(nameof(CreateResourcePropertyDetail), new { id = response.Data.Id }, response.Data);
            }

            return StatusCode(response.Status, response.Message);
        }

        [HttpGet("GetAllResourcePropertyDetails")]
        public async Task<ActionResult<ResponseObject<List<DataResponseResourcePropertyDetail>>>> GetAllResourcePropertyDetailsAsync()
        {
            try
            {
                var result = await _resourceManagementService.GetAllResourcePropertyDetailsAsync();

                if (result.Status == StatusCodes.Status404NotFound)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseObject<List<DataResponseResourcePropertyDetail>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                });
            }
        }


        //[HttpPost("CreateResource-for-print-job")]
        //[Authorize]
        //public async Task<IActionResult> CreateResourceForPrintJob([FromBody] Request_CreateResourceForPrintJob request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var response = await _resourceManagementService.CreateResourceForPrintJobAsync(request);

        //    if (response.Status == StatusCodes.Status201Created)
        //    {
        //        return CreatedAtAction(nameof(CreateResourceForPrintJob), new { id = response.Data.Id }, response.Data);
        //    }

        //    return StatusCode(response.Status, response.Message);
        //}

        [HttpPost("UsingResource-for-print-job")]
        public async Task<IActionResult> UsingResourceForPrintJob([FromBody] Request_CreateResourceForPrintJob request)
        {
            var response = await _resourceManagementService.UsingResourceForPrintJob(request);

            if (response.Status == StatusCodes.Status202Accepted)
            {
                return Accepted(response.Data);
            }

            return StatusCode(response.Status, response.Message);
        }

        [HttpGet("print-jobs")]
        public async Task<IActionResult> GetAllPrintJobs()
        {
            var result = await _printJobService.GetAllPrintJobsAsync();

            if (result.Status != StatusCodes.Status200OK)
            {
                return StatusCode(result.Status, result.Message);
            }
            return Ok(result.Data);
        }

        [HttpPost("ConfirmFinishing-project")]
        public async Task<IActionResult> ConfirmFinishingProject([FromBody] Request_FinishingProject request)
        {
            var result = await _projectService.ConfirmFinishingProjectAsync(request);

            return StatusCode(result.Status, new { result.Message, result.Data });
        }

        [HttpGet]
        [Route("all-customers")]
        public async Task<IActionResult> GetAllCustomersAsync()
        {
            var response = await _projectService.GetAllCustomersAsync();

            if (response.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(response);
            }
            else if (response.Status == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("StepTemplate")]
        public async Task<IActionResult> CreateStepTemplate([FromBody] Request_CreateStepTemplate request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _stepTemplateService.CreateStepTemplateAsync(request);

            if (response.Status == StatusCodes.Status201Created)
            {
                return CreatedAtAction(nameof(CreateProject), new { id = response.Data.Id }, response.Data);
            }

            return StatusCode(response.Status, response.Message);
        }

        [HttpGet]
        [Route("StepTemplate")]
        public async Task<IActionResult> GetAllStepTemplate()
        {
            var result = await _stepTemplateService.GetAllStepTemplateAsync();
            if (result.Status != StatusCodes.Status200OK)
            {
                return StatusCode(result.Status, result.Message);
            }
            return Ok(result.Data);
        }

        [HttpPut]
        [Route("StepTemplate/{id}")]
        public async Task<IActionResult> UpdateStepTemplateAsync(long id, [FromBody] Request_CreateStepTemplate request)
        {
            try
            {
                var response = await _stepTemplateService.UpdateStepTemplateAsync(id, request);
                if (response.Status == StatusCodes.Status400BadRequest)
                {
                    return BadRequest(response.Message); // Return specific error message
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Route("StepTemplate/{id}")]
        public async Task<IActionResult> DeleteStepAsync(long id)
        {
            var result = await _stepTemplateService.DeleteStepTemplateAsync(id);

            return StatusCode(result.Status, result);
        }

        [HttpPost]
        [Route("FlowTemplate")]
        public async Task<IActionResult> CreateFlowTemplate([FromBody] Request_CreateFlowTemplate request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _flowTemplateService.CreateFlowTemplateService(request);

            if (response.Status == StatusCodes.Status201Created)
            {
                return CreatedAtAction(nameof(CreateProject), new { id = response.Data.Id }, response.Data);
            }

            return StatusCode(response.Status, response.Message);
        }

        [HttpGet]
        [Route("FlowTemplate")]
        public async Task<IActionResult> GetAllFlowTemplate()
        {
            var result = await _flowTemplateService.GetAllFlowTemplateAsync();
            if (result.Status != StatusCodes.Status200OK)
            {
                return StatusCode(result.Status, result.Message);
            }
            return Ok(result.Data);
        }

        [HttpPut]
        [Route("FlowTemplate/{id}")]
        public async Task<IActionResult> UpdateFlowTemplateAsync(long id, [FromBody] Request_CreateFlowTemplate request)
        {
            try
            {
                var response = await _flowTemplateService.UpdateFlowTemplateAsync(id, request);
                if (response.Status == StatusCodes.Status400BadRequest)
                {
                    return BadRequest(response.Message); // Return specific error message
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Route("FlowTemplate/{id}")]
        public async Task<IActionResult> DeleteFlowAsync(long id)
        {
            var result = await _flowTemplateService.DeleteFlowTemplateAsync(id);

            return StatusCode(result.Status, result);
        }


        [HttpPut]
        [Route("Step/{id}")]
        public async Task<IActionResult> UpdateStepAsync(long id, [FromBody] Request_CreateStep request)
        {
            try
            {
                var response = await _stepService.UpdateStepAsync(id, request);
                if (response.Status == StatusCodes.Status400BadRequest)
                {
                    return BadRequest(response.Message); // Return specific error message
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpPut]
        [Route("Steps")]
        public async Task<IActionResult> UpdateStepsAsync([FromBody] Request_UpdateSteps request)
        {
            try
            {
                var response = await _stepService.UpdateStepsAsync(request);
                if (response.Status == StatusCodes.Status400BadRequest)
                {
                    return BadRequest(response.Message); // Return specific error message
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
