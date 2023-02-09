namespace FeeCollectorApplication.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IVehicleRepository Vehicle { get; }
        IPaymentRepository Payment { get; }
        IBillRepository Bill { get; }
        void Save();

    }
}
