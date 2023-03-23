using FeeCollectorApplication.Models;

namespace FeeCollectorApplication.Repository.IRepository
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        void Update(Vehicle vehicle);
    }
}
