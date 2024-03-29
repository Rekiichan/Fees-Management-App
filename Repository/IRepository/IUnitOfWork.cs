﻿namespace FeeCollectorApplication.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IVehicleRepository Vehicle { get; }
        IBillRepository Bill { get; }
        IVehicleTypeRepository VehicleType { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IEmployeeRequestRepository EmployeeRequest { get; }
        Task Save();
    }
}
