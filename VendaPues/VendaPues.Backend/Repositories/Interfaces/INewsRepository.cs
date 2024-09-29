using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Repositories.Interfaces
{
    public interface INewsRepository
    {
        Task<ActionResponse<NewsArticle>> UpdateAsync(NewsArticle newsArticle);

        Task<ActionResponse<NewsArticle>> AddAsync(NewsArticle newsArticle);

        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<NewsArticle>>> GetAsync(PaginationDTO pagination);
    }
}