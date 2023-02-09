using FeeCollectorApplication.Models;
using FeeCollectorApplication.ModelsSqlServer;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Service;

namespace FeeCollectorApplication.Repository
{
    public class CarTypeRepository : Repository<VehicleType>, IVehicleTypeRepository
    {
        private ApplicationDbContext _db;
        public CarTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(VehicleType carType)
        {
            _db.VehicleTypes.Update(carType);
        }
    }
}
