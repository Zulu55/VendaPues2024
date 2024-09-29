using Microsoft.EntityFrameworkCore;
using VendaPues.Backend.Data;
using VendaPues.Backend.Helpers;
using VendaPues.Backend.Repositories.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Repositories.Implementations;

public class PurchaseDetailRepository : GenericRepository<PurchaseDetail>, IPurchaseDetailRepository
{
    private readonly DataContext _context;

    public PurchaseDetailRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
    {
        var queryable = _context.PurchaseDetails.AsQueryable();

        if (pagination.Id != 0)
        {
            queryable = queryable.Where(x => x.PurchaseId == pagination.Id);
        }

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.Contains(pagination.Filter, StringComparison.CurrentCultureIgnoreCase));
        }

        int recordsNumber = await queryable.CountAsync();

        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = recordsNumber
        };
    }

    public override async Task<ActionResponse<IEnumerable<PurchaseDetail>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.PurchaseDetails.AsQueryable();

        if (pagination.Id != 0)
        {
            queryable = queryable.Where(x => x.PurchaseId == pagination.Id);
        }

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.Contains(pagination.Filter, StringComparison.CurrentCultureIgnoreCase));
        }

        return new ActionResponse<IEnumerable<PurchaseDetail>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination)
                .ToListAsync()
        };
    }
}