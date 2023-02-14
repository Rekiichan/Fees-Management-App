using Microsoft.AspNetCore.Mvc;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Models;
using Microsoft.AspNetCore.Authorization;
using FeeCollectorApplication.Utility;

namespace FeeCollectorApplication.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/vehicle")]
    public class VehicleController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public VehicleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAllData()
        {
            var unit = _unitOfWork.Vehicle.GetAll();
            return Ok(unit);
        }
        //[AllowAnonymous]
        //[HttpGet("{id:int}")]
        //public IActionResult GetDataById(int id)
        //{
        //    var obj = _unitOfWork.Vehicle.GetFirstOrDefault(u => u.Id == id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(obj);
        //}
        [AllowAnonymous]
        [HttpGet("lp:string")]
        public IActionResult GetDataByLicensePlate(string lp)
        {
            var obj = _unitOfWork.Vehicle.GetFirstOrDefault(u => u.LicensePlate == lp);
            return Ok(obj);
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPut("{id:int}")]
        public IActionResult UpdateVehicle(int id, VehicleUpsert obj)
        {
            var model = _unitOfWork.Vehicle.GetFirstOrDefault(u => u.Id == id);
            if (model == null)
            {
                return NotFound();
            }
            model.Price = obj.Price;
            model.LicensePlate= obj.LicensePlate;
            _unitOfWork.Vehicle.Update(model);
            _unitOfWork.Save();
            return Ok("Updated");
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpDelete("{id}")]
        public IActionResult DeleteById(int id)
        {
            var obj = _unitOfWork.Vehicle.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Vehicle.Remove(obj);
            _unitOfWork.Save();
            return Ok("Deleted");
        }
    }
}
