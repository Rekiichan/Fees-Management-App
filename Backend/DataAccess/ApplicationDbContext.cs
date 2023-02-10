using Microsoft.EntityFrameworkCore;
using FeeCollectorApplication.ModelsSqlServer;

namespace FeeCollectorApplication.Service
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Payment> Payments{ get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<BillHistory> BillHistories { get; set; }
    }
}
