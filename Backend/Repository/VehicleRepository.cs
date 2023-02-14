using FeeCollectorApplication.Models;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.DataAccess;

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
