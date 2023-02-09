using FeeCollectorApplication.ModelsSqlServer;

namespace FeeCollectorApplication.Repository.IRepository
{
    public interface IVehicleTypeRepository : IRepository<VehicleType>
    {
        void Update(VehicleType vehicleType);
    }
}
