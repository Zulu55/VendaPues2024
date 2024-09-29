using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.UnitsOfWork.Interfaces
{
    public interface IBanksUnitOfWork
    {
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Bank>>> GetAsync(PaginationDTO pagination);

        Task<IEnumerable<Bank>> GetComboAsync();
    }
}
