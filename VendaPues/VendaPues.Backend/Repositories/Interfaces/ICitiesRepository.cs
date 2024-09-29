using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Repositories.Interfaces
{
    public interface ICitiesRepository
    {
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        
        Task<ActionResponse<IEnumerable<City>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<IEnumerable<City>> GetComboAsync(int stateId);
    }
}
