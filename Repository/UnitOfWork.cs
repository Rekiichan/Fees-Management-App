using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.DataAccess;

namespace FeeCollectorApplication.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Vehicle = new VehicleRepository(_db);
            Bill = new BillRepository(_db);
            VehicleType = new VehicleTypeRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
        }
        public IVehicleRepository Vehicle { get; private set; }

        public IBillRepository Bill { get; private set; }

        public IVehicleTypeRepository VehicleType { get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}