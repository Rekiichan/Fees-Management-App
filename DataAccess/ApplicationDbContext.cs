using Microsoft.EntityFrameworkCore;
using FeeCollectorApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using FeeCollectorApplication.Models;

namespace FeeCollectorApplication.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<ApplicationUser> User { get; set; }
        public DbSet<EmployeeRequest> EmployeeRequests { get; set; }
    }
}
