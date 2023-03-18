using FeeCollectorApplication.Models;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.DataAccess;

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
        public async Task AddRange(List<VehicleType> vehicleType)
        {
            await _db.VehicleTypes.AddRangeAsync(vehicleType);
        }
    }
}
