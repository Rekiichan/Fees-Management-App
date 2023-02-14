using FeeCollectorApplication.Models;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FeeCollectorApplication.Controllers
{
    [ApiController]
    [Route("api/vehicletype")]
    [Authorize]
    public class VehicleTypeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public VehicleTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult GetAllVehicleType()
        {
            var model = _unitOfWork.VehicleType.GetAll();
            return Ok(model);
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public IActionResult AddVehicleType(VehicleTypeUpsert obj)
        {
            if (obj == null)
            {
                return BadRequest();
            }
            var temp = new VehicleType()
            {
                VehicleTypeName = obj.VehicleTypeName,
                Price = obj.Price
            };
            _unitOfWork.VehicleType.Add(temp);
            _unitOfWork.Save();
            return Ok("Created");
        }
        [Authorize(Roles = SD.Role_Admin)]
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
            model.Price = obj.Price;
            model.VehicleTypeName = obj.VehicleTypeName;
            _unitOfWork.VehicleType.Update(model);
            _unitOfWork.Save();
            return Ok("Updated");
        }
    }
}
