using Microsoft.EntityFrameworkCore;
using VendaPues.Backend.Data;
using VendaPues.Backend.Helpers;
using VendaPues.Backend.Repositories.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Repositories.Implementations;

public class BanksRepository : GenericRepository<Bank>, IBanksRepository
{
    private readonly DataContext _context;

    public BanksRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
    {
        var queryable = _context.Banks.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        int recordsNumber = await queryable.CountAsync();

        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = recordsNumber
        };
    }

    public async Task<IEnumerable<Bank>> GetComboAsync()
    {
        return await _context.Banks
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<Bank>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Banks.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Bank>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination)
                .ToListAsync()
        };
    }
}