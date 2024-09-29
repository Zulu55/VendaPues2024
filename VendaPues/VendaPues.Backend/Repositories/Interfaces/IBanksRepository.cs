using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Repositories.Interfaces
{
    public interface IBanksRepository
    {
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Bank>>> GetAsync(PaginationDTO pagination);

        Task<IEnumerable<Bank>> GetComboAsync();
    }
}