using VendaPues.Backend.Repositories.Interfaces;
using VendaPues.Backend.UnitsOfWork.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.UnitsOfWork.Implementations
{
    public class NewsUnitOfWork : GenericUnitOfWork<NewsArticle>, INewsUnitOfWork
    {
        private readonly INewsRepository _newsRepository;

        public NewsUnitOfWork(IGenericRepository<NewsArticle> repository, INewsRepository newsRepository) : base(repository)
        {
            _newsRepository = newsRepository;
        }

        public override async Task<ActionResponse<NewsArticle>> UpdateAsync(NewsArticle model) => await _newsRepository.UpdateAsync(model);

        public override async Task<ActionResponse<NewsArticle>> AddAsync(NewsArticle model) => await _newsRepository.AddAsync(model);

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination) => await _newsRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<IEnumerable<NewsArticle>>> GetAsync(PaginationDTO pagination) => await _newsRepository.GetAsync(pagination);
    }
}