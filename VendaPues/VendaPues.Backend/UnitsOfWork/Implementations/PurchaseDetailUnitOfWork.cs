using VendaPues.Backend.Repositories.Interfaces;
using VendaPues.Backend.UnitsOfWork.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.UnitsOfWork.Implementations
{
    public class PurchaseDetailUnitOfWork : GenericUnitOfWork<PurchaseDetail>, IPurchaseDetailUnitOfWork
    {
        private readonly IPurchaseDetailRepository _purchaseDetailRepository;

        public PurchaseDetailUnitOfWork(IGenericRepository<PurchaseDetail> repository, IPurchaseDetailRepository purchaseDetailRepository) : base(repository)
        {
            _purchaseDetailRepository = purchaseDetailRepository;
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination) => await _purchaseDetailRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<IEnumerable<PurchaseDetail>>> GetAsync(PaginationDTO pagination) => await _purchaseDetailRepository.GetAsync(pagination);
    }
}