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
            var VehicleViewModel = await _unit.Vehicle.GetAllAsync();
            if (VehicleViewModel == null)
            {
                return NotFound();
            }
            return Ok(VehicleViewModel);
        }
    }
}
