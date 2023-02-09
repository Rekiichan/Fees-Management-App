using FeeCollectorApplication.ModelsSqlServer;

namespace FeeCollectorApplication.Repository.IRepository
{
    public interface IBillRepository : IRepository<Bill>
    {
        void Update(Bill bill);
    }
}
