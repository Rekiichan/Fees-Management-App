using FeeCollectorApplication.Models;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.DataAccess;

namespace FeeCollectorApplication.Repository
{
    public class EmployeeRequestRepository : Repository<EmployeeRequest>, IEmployeeRequestRepository
    {
        private ApplicationDbContext _db;
        public EmployeeRequestRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(EmployeeRequest empRequest)
        {
            _db.EmployeeRequests.Update(empRequest);
        }
    }
}
