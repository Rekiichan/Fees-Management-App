using FeeCollectorApplication.ModelsSqlServer;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Service;

namespace FeeCollectorApplication.Repository
{
    public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
    {
        private ApplicationDbContext _db;
        public VehicleRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Vehicle vehicle)
        {
            _db.Vehicles.Update(vehicle);
        }
    }
}
