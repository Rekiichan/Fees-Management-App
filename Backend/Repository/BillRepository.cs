using FeeCollectorApplication.ModelsSqlServer;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Service;

namespace FeeCollectorApplication.Repository
{
    public class BillRepository : Repository<Bill>, IBillRepository
    {
        private ApplicationDbContext _db;
        public BillRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Bill bill)
        {
            _db.Bills.Update(bill);
        }
    }
}
