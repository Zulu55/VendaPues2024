using VendaPues.Backend.Repositories.Implementations;
using VendaPues.Backend.Repositories.Interfaces;
using VendaPues.Backend.UnitsOfWork.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.UnitsOfWork.Implementations
{
    public class InventoriesUnitOfWork : GenericUnitOfWork<Inventory>, IInventoriesUnitOfWork
    {
        private readonly IInventoriesRepository _inventoriesRepository;

        public InventoriesUnitOfWork(IGenericRepository<Inventory> repository, IInventoriesRepository inventoriesRepository) : base(repository)
        {
            _inventoriesRepository = inventoriesRepository;
        }

        public async Task<IEnumerable<Inventory>> GetComboAsync() => await _inventoriesRepository.GetComboAsync();

        public async Task<ActionResponse<bool>> FinishCount1Async(int id) => await _inventoriesRepository.FinishCount1Async(id);

        public async Task<ActionResponse<bool>> FinishCount2Async(int id) => await _inventoriesRepository.FinishCount2Async(id);

        public async Task<ActionResponse<bool>> FinishCount3Async(int id) => await _inventoriesRepository.FinishCount3Async(id);

        public override async Task<ActionResponse<Inventory>> GetAsync(int id) => await _inventoriesRepository.GetAsync(id);

        public override async Task<ActionResponse<Inventory>> AddAsync(Inventory model) => await _inventoriesRepository.AddAsync(model);

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination) => await _inventoriesRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<IEnumerable<Inventory>>> GetAsync(PaginationDTO pagination) => await _inventoriesRepository.GetAsync(pagination);
    }
}