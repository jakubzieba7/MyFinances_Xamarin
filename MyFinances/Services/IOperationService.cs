using MyFinances.Core.Dtos;
using MyFinances.Core.Response;
using MyFinances.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFinances.Services
{
    public interface IOperationService
    {
        Task<DataResponse<int>> AddAsync(OperationDto operation);
        Task<Response> UpdateAsync(OperationDto operation);
        Task<Response> DeleteAsync(int id);
        Task<DataResponse<OperationDto>> GetAsync(int id);
        Task<DataResponse<IEnumerable<OperationDto>>> GetAsync();
        Task<DataResponse<IEnumerable<OperationDto>>> GetAsync(PaginationFilter paginationFilter);
    }
}
