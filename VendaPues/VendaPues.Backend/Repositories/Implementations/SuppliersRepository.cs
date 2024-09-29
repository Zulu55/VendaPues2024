using Microsoft.EntityFrameworkCore;
using VendaPues.Backend.Data;
using VendaPues.Backend.Helpers;
using VendaPues.Backend.Repositories.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Repositories.Implementations;

public class SuppliersRepository : GenericRepository<Supplier>, ISuppliersRepository
{
    private readonly DataContext _context;

    public SuppliersRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
    {
        var queryable = _context.Suppliers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.SupplierName.ToLower().Contains(pagination.Filter.ToLower()));
        }

        int recordsNumber = await queryable.CountAsync();

        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = recordsNumber
        };
    }

    public async Task<IEnumerable<Supplier>> GetComboAsync()
    {
        return await _context.Suppliers
            .OrderBy(c => c.SupplierName)
            .ToListAsync();
    }

    public override async Task<ActionResponse<Supplier>> GetAsync(int id)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.City!)
            .ThenInclude(c => c.State!)
            .ThenInclude(s => s.Country)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (supplier == null)
        {
            return new ActionResponse<Supplier>
            {
                WasSuccess = false,
                Message = "Registro no encontrado"
            };
        }

        return new ActionResponse<Supplier>
        {
            WasSuccess = true,
            Result = supplier
        };
    }

    public override async Task<ActionResponse<IEnumerable<Supplier>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Suppliers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.SupplierName.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Supplier>>
        {
            WasSuccess = true,
            Result = await queryable
                .Include(s => s.City!)
                .ThenInclude(c => c.State!)
                .ThenInclude(s => s.Country)
                .OrderBy(x => x.SupplierName)
                .Paginate(pagination)
                .ToListAsync()
        };
    }
}