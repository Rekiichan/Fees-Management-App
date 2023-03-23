using FeeCollectorApplication.Models;

namespace FeeCollectorApplication.Repository.IRepository
{
    public interface IVehicleTypeRepository : IRepository<VehicleType>
    {
        void Update(VehicleType vehicleType);
        Task AddRange(List<VehicleType> vehicleType);
    }
}
