using Microsoft.EntityFrameworkCore;
using VendaPues.Backend.Data;
using VendaPues.Backend.Helpers;
using VendaPues.Backend.Repositories.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Repositories.Implementations
{
    public class NewsRepository : GenericRepository<NewsArticle>, INewsRepository
    {
        private readonly DataContext _context;
        private readonly IFileStorage _fileStorage;

        public NewsRepository(DataContext context, IFileStorage fileStorage) : base(context)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        public override async Task<ActionResponse<NewsArticle>> UpdateAsync(NewsArticle newsArticle)
        {
            if (!newsArticle.ImageUrl.StartsWith("https://"))
            {
                var photoProduct = Convert.FromBase64String(newsArticle.ImageUrl);
                newsArticle.ImageUrl = await _fileStorage.SaveFileAsync(photoProduct, ".jpg", "promos");
            }
            return await base.UpdateAsync(newsArticle);
        }

        public override async Task<ActionResponse<NewsArticle>> AddAsync(NewsArticle newsArticle)
        {
            var photoProduct = Convert.FromBase64String(newsArticle.ImageUrl);
            newsArticle.ImageUrl = await _fileStorage.SaveFileAsync(photoProduct, ".jpg", "promos");
            return await base.AddAsync(newsArticle);
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.NewsArticles.AsQueryable();

            if (pagination.Id != 0)
            {
                queryable = queryable.Where(x => x.Active);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Title.ToLower().Contains(pagination.Filter.ToLower()));
            }

            int recordsNumber = await queryable.CountAsync();

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }

        public override async Task<ActionResponse<IEnumerable<NewsArticle>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.NewsArticles.AsQueryable();

            if (pagination.Id != 0)
            {
                queryable = queryable.Where(x => x.Active);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Title.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<NewsArticle>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.Title)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }
    }
}