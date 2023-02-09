using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Service;

namespace FeeCollectorApplication.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Vehicle = new VehicleRepository(_db);
            Payment= new PaymentRepository(_db);
            Bill= new BillRepository(_db);
            VehicleType = new VehicleTypeRepository(_db);
        }
        public IVehicleRepository Vehicle { get; private set; }

        public IPaymentRepository Payment { get; private set; }

        public IBillRepository Bill { get; private set; }

        public IVehicleTypeRepository VehicleType { get; private set; }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}