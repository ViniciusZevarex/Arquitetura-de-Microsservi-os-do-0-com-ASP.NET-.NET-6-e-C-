using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {

        public readonly DbContextOptions<MySQLContext> _context;

        public OrderRepository(
            DbContextOptions<MySQLContext> dbContext
        )
        {
            _context = dbContext;
        }

        public async Task<bool> AddOrder(OrderHeader header)
        {
            if (header == null) return false;

            await using var _db = new MySQLContext( _context );
            _db.OrderHeaders.Add(header);
            _db.SaveChanges();
            return true;
        }

        public async Task UpdateOrderPaymentStatus(long orderHeaderId, bool status)
        {
            await using var _db = new MySQLContext(_context);
            var header = await _db.OrderHeaders.FirstOrDefaultAsync(o => o.Id== orderHeaderId);
            if (header != null)
            {
                header.PaymentStatus = status;
                _db.Update(header);
                await _db.SaveChangesAsync();
            }
            
        }
    }
}
