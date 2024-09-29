using Microsoft.EntityFrameworkCore;
using VendaPues.Backend.Data;
using VendaPues.Backend.Helpers;
using VendaPues.Backend.Repositories.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Enums;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Repositories.Implementations
{
    public class OrdersRepository : GenericRepository<Order>, IOrdersRepository
    {
        private readonly DataContext _context;
        private readonly IUsersRepository _usersRepository;
        private readonly IKardexRepository _kardexRepository;

        public OrdersRepository(DataContext context, IUsersRepository usersRepository, IKardexRepository kardexRepository) : base(context)
        {
            _context = context;
            _usersRepository = usersRepository;
            _kardexRepository = kardexRepository;
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.Orders.AsQueryable();

            int recordsNumber = await queryable.CountAsync();

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }

        public async Task<ActionResponse<IEnumerable<Order>>> GetAsync(string email, PaginationDTO pagination)
        {
            var user = await _usersRepository.GetUserAsync(email);
            if (user == null)
            {
                return new ActionResponse<IEnumerable<Order>>
                {
                    WasSuccess = false,
                    Message = "Usuario no válido",
                };
            }

            var queryable = _context.Orders
                .Include(s => s.User!)
                .Include(s => s.OrderDetails!)
                .ThenInclude(sd => sd.Product)
                .AsQueryable();

            var isAdmin = await _usersRepository.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin)
            {
                queryable = queryable.Where(s => s.User!.Email == email);
            }

            var orders = await queryable
                    .OrderByDescending(x => x.Date)
                    .Paginate(pagination)
                    .ToListAsync();
            return new ActionResponse<IEnumerable<Order>>
            {
                WasSuccess = true,
                Result = orders
            };
        }

        public async Task<ActionResponse<int>> GetTotalPagesAsync(string email, PaginationDTO pagination)
        {
            var user = await _usersRepository.GetUserAsync(email);
            if (user == null)
            {
                return new ActionResponse<int>
                {
                    WasSuccess = false,
                    Message = "Usuario no válido",
                };
            }

            var queryable = _context.Orders.AsQueryable();

            var isAdmin = await _usersRepository.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin)
            {
                queryable = queryable.Where(s => s.User!.Email == email);
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = (int)totalPages
            };
        }

        public override async Task<ActionResponse<Order>> GetAsync(int id)
        {
            var order = await _context.Orders
                .Include(s => s.User!)
                .ThenInclude(u => u.City!)
                .ThenInclude(c => c.State!)
                .ThenInclude(s => s.Country)
                .Include(s => s.OrderDetails!)
                .ThenInclude(sd => sd.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (order == null)
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = "Pedido no existe"
                };
            }

            return new ActionResponse<Order>
            {
                WasSuccess = true,
                Result = order
            };
        }

        public async Task<ActionResponse<Order>> UpdateFullAsync(string email, OrderDTO orderDTO)
        {
            var user = await _usersRepository.GetUserAsync(email);
            if (user == null)
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = "Usuario no existe"
                };
            }

            var isAdmin = await _usersRepository.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin && orderDTO.OrderStatus != OrderStatus.Cancelled)
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = "Solo permitido para administradores."
                };
            }

            var order = await _context.Orders
                .Include(s => s.OrderDetails)
                .FirstOrDefaultAsync(s => s.Id == orderDTO.Id);
            if (order == null)
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = "Pedido no existe"
                };
            }

            if (orderDTO.OrderStatus == OrderStatus.Cancelled)
            {
                await ReturnStockAsync(order);
            }

            order.OrderStatus = orderDTO.OrderStatus;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return new ActionResponse<Order>
            {
                WasSuccess = true,
                Result = order
            };
        }

        private async Task ReturnStockAsync(Order order)
        {
            foreach (var orderDetail in order.OrderDetails!)
            {
                var product = await _context.Products.FindAsync(orderDetail.ProductId);
                if (product != null)
                {
                    var kardexDTO = new KardexDTO
                    {
                        Cost = product.Cost,
                        Date = DateTime.UtcNow,
                        KardexType = KardexType.CancelOrder,
                        ProductId = orderDetail.ProductId,
                        Quantity = orderDetail.Quantity,
                    };

                    await _kardexRepository.AddAsync(kardexDTO);
                }
            }
        }

        public async Task<ActionResponse<IEnumerable<Order>>> GetReportAsync(DatesDTO datesDTO)
        {
            var queryable = _context.Orders
                .Where(x => x.OrderStatus != OrderStatus.Cancelled && x.Date >= datesDTO.InitialDate && x.Date <= datesDTO.FinalDate)
                .Include(s => s.User!)
                .Include(s => s.OrderDetails!)
                .ThenInclude(sd => sd.Product)
                .AsQueryable();

            var orders = await queryable
                    .OrderBy(x => x.Date)
                    .ToListAsync();

            return new ActionResponse<IEnumerable<Order>>
            {
                WasSuccess = true,
                Result = orders
            };
        }
    }
}