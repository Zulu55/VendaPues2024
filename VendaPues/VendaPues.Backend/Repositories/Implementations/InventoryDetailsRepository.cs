using Microsoft.EntityFrameworkCore;
using VendaPues.Backend.Data;
using VendaPues.Backend.Helpers;
using VendaPues.Backend.Repositories.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Repositories.Implementations
{
    public class InventoryDetailsRepository : GenericRepository<InventoryDetail>, IInventoryDetailsRepository
    {
        private readonly DataContext _context;

        public InventoryDetailsRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ActionResponse<InventoryDetail>> UpdateAsync(InventoryDetail inventoryDetail)
        {
            var currentInventoryDetail = await _context.InventoryDetails.FindAsync(inventoryDetail.Id);
            if (currentInventoryDetail == null)
            {
                return new ActionResponse<InventoryDetail>
                {
                    WasSuccess = false,
                    Message = "Detalle de inventario no existe."
                };
            }

            currentInventoryDetail.Cost = inventoryDetail.Cost;
            currentInventoryDetail.Count1 = inventoryDetail.Count1;
            currentInventoryDetail.Count2 = inventoryDetail.Count2;
            currentInventoryDetail.Count3 = inventoryDetail.Count3;
            currentInventoryDetail.Adjustment = inventoryDetail.Adjustment;

            _context.Update(currentInventoryDetail);
            await _context.SaveChangesAsync();

            return new ActionResponse<InventoryDetail>
            {
                WasSuccess = true,
                Result = currentInventoryDetail
            };
        }

        public async Task<ActionResponse<int>> GetRecordsNumberCount1Async(PaginationDTO pagination)
        {
            var queryable = _context.InventoryDetails.AsQueryable();
            if (pagination.Id != 0)
            {
                queryable = queryable.Where(x => x.InventoryId == pagination.Id);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Product!.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            int recordsNumber = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }

        public async Task<ActionResponse<int>> GetRecordsNumberCount2Async(PaginationDTO pagination)
        {
            var queryable = _context.InventoryDetails
                .Where(x => x.Stock != x.Count1)
                .AsQueryable();
            if (pagination.Id != 0)
            {
                queryable = queryable.Where(x => x.InventoryId == pagination.Id);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Product!.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            int recordsNumber = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }

        public async Task<ActionResponse<int>> GetRecordsNumberCount3Async(PaginationDTO pagination)
        {
            var queryable = _context.InventoryDetails
                .Where(x => x.Stock != x.Count1 && x.Stock != x.Count2 && x.Count1 != x.Count2)
                .AsQueryable();
            if (pagination.Id != 0)
            {
                queryable = queryable.Where(x => x.InventoryId == pagination.Id);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Product!.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            int recordsNumber = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }

        public async Task<ActionResponse<IEnumerable<InventoryDetail>>> GetCount1Async(PaginationDTO pagination)
        {
            var queryable = _context.InventoryDetails.AsQueryable();
            if (pagination.Id != 0)
            {
                queryable = queryable.Where(x => x.InventoryId == pagination.Id);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Product!.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<InventoryDetail>>
            {
                WasSuccess = true,
                Result = await queryable
                    .Include(x => x.Product)
                    .OrderBy(x => x.Product!.Name)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<InventoryDetail>>> GetCount2Async(PaginationDTO pagination)
        {
            var queryable = _context.InventoryDetails
                .Where(x => x.Stock != x.Count1)
                .AsQueryable();
            if (pagination.Id != 0)
            {
                queryable = queryable.Where(x => x.InventoryId == pagination.Id);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Product!.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<InventoryDetail>>
            {
                WasSuccess = true,
                Result = await queryable
                    .Include(x => x.Product)
                    .OrderBy(x => x.Product!.Name)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<InventoryDetail>>> GetCount3Async(PaginationDTO pagination)
        {
            var queryable = _context.InventoryDetails
                .Where(x => x.Stock != x.Count1 && x.Stock != x.Count2 && x.Count1 != x.Count2)
                .AsQueryable();
            if (pagination.Id != 0)
            {
                queryable = queryable.Where(x => x.InventoryId == pagination.Id);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Product!.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<InventoryDetail>>
            {
                WasSuccess = true,
                Result = await queryable
                    .Include(x => x.Product)
                    .OrderBy(x => x.Product!.Name)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }
    }
}