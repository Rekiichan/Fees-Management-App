using FeeCollectorApplication.Models;

namespace FeeCollectorApplication.Repository.IRepository
{
    public interface IBillRepository : IRepository<Bill>
    {
        void Update(Bill bill);
    }
}
