using FeeCollectorApplication.ModelsSqlServer;

namespace FeeCollectorApplication.Repository.IRepository
{
    public interface IBillHistoryRepository : IRepository<BillHistory>
    {
        void Update(BillHistory billHistory);
    }
}
