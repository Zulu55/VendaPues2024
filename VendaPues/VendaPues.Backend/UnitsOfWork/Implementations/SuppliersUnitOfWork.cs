using VendaPues.Backend.Repositories.Implementations;
using VendaPues.Backend.Repositories.Interfaces;
using VendaPues.Backend.UnitsOfWork.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.UnitsOfWork.Implementations
{
    public class SuppliersUnitOfWork : GenericUnitOfWork<Supplier>, ISuppliersUnitOfWork
    {
        private readonly ISuppliersRepository _suppliersRepository;

        public SuppliersUnitOfWork(IGenericRepository<Supplier> repository, ISuppliersRepository suppliersRepository) : base(repository)
        {
            this._suppliersRepository = suppliersRepository;
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination) => await _suppliersRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<IEnumerable<Supplier>>> GetAsync(PaginationDTO pagination) => await _suppliersRepository.GetAsync(pagination);

        public async Task<IEnumerable<Supplier>> GetComboAsync() => await _suppliersRepository.GetComboAsync();

        public override async Task<ActionResponse<Supplier>> GetAsync(int id) => await _suppliersRepository.GetAsync(id);
    }
}
