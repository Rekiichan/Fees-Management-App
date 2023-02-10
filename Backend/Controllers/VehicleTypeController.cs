using FeeCollectorApplication.ModelsSqlServer;
using FeeCollectorApplication.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace FeeCollectorApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleTypeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public VehicleTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public IActionResult GetAllVehicleType()
        {
            var model = _unitOfWork.VehicleType.GetAll();
            return Ok(model);
        }
        [HttpPost]
        public IActionResult AddCarType(VehicleTypeUpsert obj)
        {
            if (obj == null)
            {
                return BadRequest();
            }
            var temp = new VehicleType()
            {
                vehicle_type = obj.vehicle_type,
                price = obj.price
            };
            _unitOfWork.VehicleType.Add(temp);
            _unitOfWork.Save();
            return Ok("Created");
        }
        [HttpPut("{id:int}")]
        public IActionResult EditVehicleType(int id, VehicleTypeUpsert obj)
        {
            if (obj == null)
            {
                return BadRequest();
            }
            var model = _unitOfWork.VehicleType.GetFirstOrDefault(u => u.Id == id);
            if (model == null)
            {
                return BadRequest();
            }
            model.price = obj.price;
            model.vehicle_type = obj.vehicle_type;

            _unitOfWork.VehicleType.Update(model);
            _unitOfWork.Save();
            return Ok("Updated");
        }
    }
}
