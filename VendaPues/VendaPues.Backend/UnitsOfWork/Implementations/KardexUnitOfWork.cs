using VendaPues.Backend.Repositories.Interfaces;
using VendaPues.Backend.UnitsOfWork.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.UnitsOfWork.Implementations
{
    public class KardexUnitOfWork : GenericUnitOfWork<Kardex>, IKardexUnitOfWork
    {
        private readonly IKardexRepository _kardexRepository;

        public KardexUnitOfWork(IGenericRepository<Kardex> repository, IKardexRepository kardexRepository) : base(repository)
        {
            _kardexRepository = kardexRepository;
        }

        public async Task<ActionResponse<bool>> AddAsync(KardexDTO kardexDTO) => await _kardexRepository.AddAsync(kardexDTO);

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination) => await _kardexRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<IEnumerable<Kardex>>> GetAsync(PaginationDTO pagination) => await _kardexRepository.GetAsync(pagination);
    }
}