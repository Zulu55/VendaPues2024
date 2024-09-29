using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Repositories.Interfaces
{
    public interface IInventoryDetailsRepository
    {
        Task<ActionResponse<InventoryDetail>> UpdateAsync(InventoryDetail inventoryDetail);

        Task<ActionResponse<int>> GetRecordsNumberCount1Async(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<InventoryDetail>>> GetCount1Async(PaginationDTO pagination);

        Task<ActionResponse<int>> GetRecordsNumberCount2Async(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<InventoryDetail>>> GetCount2Async(PaginationDTO pagination);

        Task<ActionResponse<int>> GetRecordsNumberCount3Async(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<InventoryDetail>>> GetCount3Async(PaginationDTO pagination);
    }
}