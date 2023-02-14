using FeeCollectorApplication.Models;

namespace FeeCollectorApplication.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        void Update(ApplicationUser applicationUser);
    }
}
