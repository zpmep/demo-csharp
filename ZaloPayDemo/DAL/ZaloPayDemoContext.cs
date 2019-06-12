using ZaloPayDemo.Models;
using System.Data.Entity;

namespace ZaloPayDemo.DAL
{
    public class ZaloPayDemoContext : DbContext
    {
        public ZaloPayDemoContext() : base("ZaloPayDemoSqlServer")
        {
            Configuration.ValidateOnSaveEnabled = false;
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Refund> Refunds { get; set; }
    }
}