using VendaPues.Backend.Repositories.Interfaces;
using VendaPues.Backend.UnitsOfWork.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.UnitsOfWork.Implementations
{
    public class TemporalPurchasesUnitOfWork : GenericUnitOfWork<TemporalPurchase>, ITemporalPurchasesUnitOfWork
    {
        private readonly ITemporalPurchasesRepository _temporalPurchasesRepository;

        public TemporalPurchasesUnitOfWork(IGenericRepository<TemporalPurchase> repository, ITemporalPurchasesRepository temporalPurchasesRepository) : base(repository)
        {
            _temporalPurchasesRepository = temporalPurchasesRepository;
        }

        public async Task<ActionResponse<TemporalPurchase>> PutFullAsync(TemporalPurchaseDTO temporalPurchaseDTO) => await _temporalPurchasesRepository.PutFullAsync(temporalPurchaseDTO);

        public override async Task<ActionResponse<TemporalPurchase>> GetAsync(int id) => await _temporalPurchasesRepository.GetAsync(id);

        public async Task<ActionResponse<TemporalPurchaseDTO>> AddFullAsync(string email, TemporalPurchaseDTO temporalPurchaseDTO) => await _temporalPurchasesRepository.AddFullAsync(email, temporalPurchaseDTO);

        public async Task<ActionResponse<IEnumerable<TemporalPurchase>>> GetAsync(string email) => await _temporalPurchasesRepository.GetAsync(email);

        public async Task<ActionResponse<int>> GetCountAsync(string email) => await _temporalPurchasesRepository.GetCountAsync(email);

        public async Task<ActionResponse<bool>> DeleteAsync(string email) => await _temporalPurchasesRepository.DeleteAsync(email);
    }
}