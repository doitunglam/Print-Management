using PM.Application.Payloads.RequestModels.ProjectRequests;
using PM.Application.Payloads.ResponseModels.DataProjects;
using PM.Application.Payloads.Responses;
using PM.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IProductService
{
    Task<ResponseObject<DataResponseProduct>> CreateProductAsync(Request_CreateProduct request);
    Task<ResponseObject<List<DataResponseProduct>>> GetAllProductsAsync();
    Task<ResponseObject<DataResponseProduct>> GetProductByIdAsync(long id);
}
