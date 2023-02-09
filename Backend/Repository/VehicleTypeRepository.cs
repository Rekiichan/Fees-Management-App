using FeeCollectorApplication.Models;
using FeeCollectorApplication.ModelsSqlServer;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Service;

namespace FeeCollectorApplication.Repository
{
    public class VehicleTypeRepository : Repository<VehicleType>, IVehicleTypeRepository
    {
        private ApplicationDbContext _db;
        public VehicleTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(VehicleType vehicleType)
        {
            _db.VehicleTypes.Update(vehicleType);
        }
    }
}
