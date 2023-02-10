using FeeCollectorApplication.Models;
using FeeCollectorApplication.ModelsSqlServer;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Service;

namespace FeeCollectorApplication.Repository
{
    public class BillHistoryRepository : Repository<BillHistory>, IBillHistoryRepository
    {
        private ApplicationDbContext _db;
        public BillHistoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(BillHistory billHistory)
        {
            _db.BillHistories.Update(billHistory);
        }
    }
}
