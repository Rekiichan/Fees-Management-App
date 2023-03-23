using FeeCollectorApplication.Models;

namespace FeeCollectorApplication.Repository.IRepository
{
    public interface IEmployeeRequestRepository : IRepository<EmployeeRequest>
    {
        void Update(EmployeeRequest emp);
    }
}
