using FeeCollectorApplication.ModelsSqlServer;

namespace FeeCollectorApplication.Repository.IRepository
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        void Update(Payment payment);
    }
}
