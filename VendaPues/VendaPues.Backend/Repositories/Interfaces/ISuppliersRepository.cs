using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Repositories.Interfaces
{
    public interface ISuppliersRepository
    {
        Task<ActionResponse<Supplier>> GetAsync(int id);

        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Supplier>>> GetAsync(PaginationDTO pagination);

        Task<IEnumerable<Supplier>> GetComboAsync();
    }
}
