namespace SHOP.CO.Infrastructure.Repositories
{
    public interface IOrderRepository : IBaseRepository<Order>
    {

        Task<Order?> GetOrderWithDetailsAsync(int orderId);
    }


    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(ShopCoDbContext context) : base(context) { }

        public IQueryable<Order> GetOrdersAsQueryable()
        {
            return _context.Orders.AsQueryable();
        }
        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}
    //public interface IOrderRepository
    //{
    //    IQueryable<Order> GetOrdersAsQueryable();
    //    Task<Order?> GetOrderWithDetailsAsync(int orderId);
    //    Task UpdateOrderAsync(Order order);
    //}
    //public class OrderRepository : IOrderRepository
    //{
    //    private readonly ShopCoDbContext _context;

    //    public OrderRepository(ShopCoDbContext context)
    //    {
    //        _context = context;
    //    }

    //    public IQueryable<Order> GetOrdersAsQueryable()
    //    {
    //        return _context.Orders.AsQueryable();
    //    }

    //    public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
    //    {
    //        return await _context.Orders
    //            .Include(o => o.OrderItems) // Lấy luôn chi tiết SP khách đã mua
    //            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    //    }

    //    public async Task UpdateOrderAsync(Order order)
    //    {
    //        _context.Orders.Update(order);
    //        await _context.SaveChangesAsync();
    //    }
    //}
//}
