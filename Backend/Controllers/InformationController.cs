using FeeCollectorApplication.Models.ViewModel;
using FeeCollectorApplication.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FeeCollectorApplication.Controllers
{
    [Route("api/information")]
    [ApiController]
    public class InformationController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        public InformationController (IUnitOfWork unit)
        {
            _unit = unit;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllData()
        {
            var VehicleViewModel = await _unit.Vehicle.GetLimitAsync(null,20);
            if (VehicleViewModel == null)
            {
                return NotFound("No Vehicle Found");
            }
            return Ok(VehicleViewModel);
        }
        [HttpGet("{lp}")]
        public async Task<IActionResult> GetDataByLp(string lp)
        {
            var Vehicle = await _unit.Vehicle.GetFirstOrDefaultAsync(u => u.LicensePlate== lp);
            if (Vehicle == null)
            {
                return NotFound();
            }
            return Ok(Vehicle);
        }
    }
}
