using Microsoft.EntityFrameworkCore;

namespace GeekShopping.OrderAPI.Model.Context
{
    public class MySQLContext : DbContext
    {

        public MySQLContext()
        {

        }

        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options)
        {

        }

        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

    }
}
