using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.UnitsOfWork.Interfaces
{
    public interface IKardexUnitOfWork
    {
        Task<ActionResponse<bool>> AddAsync(KardexDTO kardexDTO);

        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Kardex>>> GetAsync(PaginationDTO pagination);
    }
}
