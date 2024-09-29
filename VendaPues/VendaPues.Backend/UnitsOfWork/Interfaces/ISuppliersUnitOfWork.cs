using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.UnitsOfWork.Interfaces
{
    public interface ISuppliersUnitOfWork
    {
        Task<ActionResponse<Supplier>> GetAsync(int id);

        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Supplier>>> GetAsync(PaginationDTO pagination);

        Task<IEnumerable<Supplier>> GetComboAsync();
    }
}
