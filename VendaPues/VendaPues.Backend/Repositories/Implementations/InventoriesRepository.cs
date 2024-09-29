using Microsoft.EntityFrameworkCore;
using VendaPues.Backend.Data;
using VendaPues.Backend.Helpers;
using VendaPues.Backend.Repositories.Interfaces;
using VendaPues.Backend.UnitsOfWork.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Enums;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Repositories.Implementations;

public class InventoriesRepository : GenericRepository<Inventory>, IInventoriesRepository
{
    private readonly DataContext _context;
    private readonly IKardexUnitOfWork _kardexUnitOfWork;

    public InventoriesRepository(DataContext context, IKardexUnitOfWork kardexUnitOfWork) : base(context)
    {
        _context = context;
        _kardexUnitOfWork = kardexUnitOfWork;
    }

    public async Task<ActionResponse<bool>> FinishCount1Async(int id)
    {
        var inventory = await _context.Inventories.FindAsync(id);
        if (inventory == null)
        {
            return new ActionResponse<bool>
            {
                WasSuccess = false,
                Message = "Inventario no existe"
            };
        }

        inventory.Count1Finish = true;
        _context.Update(inventory);
        await _context.SaveChangesAsync();
        return new ActionResponse<bool> { WasSuccess = true };
    }

    public async Task<ActionResponse<bool>> FinishCount2Async(int id)
    {
        var inventory = await _context.Inventories.FindAsync(id);
        if (inventory == null)
        {
            return new ActionResponse<bool>
            {
                WasSuccess = false,
                Message = "Inventario no existe"
            };
        }

        inventory.Count2Finish = true;
        _context.Update(inventory);
        await _context.SaveChangesAsync();
        return new ActionResponse<bool> { WasSuccess = true };
    }

    public async Task<ActionResponse<bool>> FinishCount3Async(int id)
    {
        var inventory = await _context.Inventories
            .Include(x => x.InventoryDetails)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (inventory == null)
        {
            return new ActionResponse<bool>
            {
                WasSuccess = false,
                Message = "Inventario no existe"
            };
        }

        foreach (var inventoryDetail in inventory.InventoryDetails!)
        {
            if (!(inventoryDetail.Stock == inventoryDetail.Count1 || inventoryDetail.Stock == inventoryDetail.Count2))
            {
                if (inventoryDetail.Count1 == inventoryDetail.Count2)
                {
                    if (inventoryDetail.Stock > inventoryDetail.Count1)
                    {
                        inventoryDetail.Adjustment = (inventoryDetail.Stock - inventoryDetail.Count1) * -1;
                    }
                    else
                    {
                        inventoryDetail.Adjustment = inventoryDetail.Count1 - inventoryDetail.Stock;
                    }
                }
                else
                {
                    if (inventoryDetail.Stock > inventoryDetail.Count3)
                    {
                        inventoryDetail.Adjustment = (inventoryDetail.Stock - inventoryDetail.Count3) * -1;
                    }
                    else
                    {
                        inventoryDetail.Adjustment = inventoryDetail.Count3 - inventoryDetail.Stock;
                    }
                }

                var kardexDTO = new KardexDTO
                {
                    Date = inventory.Date,
                    ProductId = inventoryDetail.ProductId,
                    KardexType = KardexType.Inventory,
                    Cost = inventoryDetail.Cost,
                    Quantity = inventoryDetail.Adjustment
                };

                await _kardexUnitOfWork.AddAsync(kardexDTO);
            }
        }

        inventory.Count3Finish = true;
        await _context.SaveChangesAsync();
        return new ActionResponse<bool> { WasSuccess = true };
    }

    public override async Task<ActionResponse<Inventory>> GetAsync(int id)
    {
        var inventory = await _context.Inventories
             .Include(x => x.InventoryDetails!)
             .ThenInclude(x => x.Product!)
             .FirstOrDefaultAsync(x => x.Id == id);

        if (inventory == null)
        {
            return new ActionResponse<Inventory>
            {
                WasSuccess = false,
                Message = "Inventario no existe"
            };
        }

        return new ActionResponse<Inventory>
        {
            WasSuccess = true,
            Result = inventory
        };
    }

    public override async Task<ActionResponse<Inventory>> AddAsync(Inventory inventory)
    {
        inventory.InventoryDetails = [];
        inventory.Date = inventory.Date.ToUniversalTime();
        var products = await _context.Products.ToListAsync();
        foreach (var product in products)
        {
            inventory.InventoryDetails.Add(new InventoryDetail
            {
                Cost = product.Cost,
                ProductId = product.Id,
                Stock = product.Stock,
            });
        }

        await base.AddAsync(inventory);
        return new ActionResponse<Inventory>
        {
            WasSuccess = true,
            Result = inventory,
        };
    }

    public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
    {
        var queryable = _context.Inventories.AsQueryable();
        int recordsNumber = await queryable.CountAsync();

        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = recordsNumber
        };
    }

    public override async Task<ActionResponse<IEnumerable<Inventory>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Inventories.AsQueryable();

        return new ActionResponse<IEnumerable<Inventory>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderByDescending(x => x.Date)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public async Task<IEnumerable<Inventory>> GetComboAsync()
    {
        return await _context.Inventories
           .OrderBy(c => c.Name)
           .ToListAsync();
    }
}