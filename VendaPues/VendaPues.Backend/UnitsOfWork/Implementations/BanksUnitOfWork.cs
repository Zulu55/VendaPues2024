using VendaPues.Backend.Repositories.Interfaces;
using VendaPues.Backend.UnitsOfWork.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.UnitsOfWork.Implementations
{
    public class BanksUnitOfWork : GenericUnitOfWork<Bank>, IBanksUnitOfWork
    {
        private readonly IBanksRepository _banksRepository;

        public BanksUnitOfWork(IGenericRepository<Bank> repository, IBanksRepository banksRepository) : base(repository)
        {
            _banksRepository = banksRepository;
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination) => await _banksRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<IEnumerable<Bank>>> GetAsync(PaginationDTO pagination) => await _banksRepository.GetAsync(pagination);

        public async Task<IEnumerable<Bank>> GetComboAsync() => await _banksRepository.GetComboAsync();
    }
}