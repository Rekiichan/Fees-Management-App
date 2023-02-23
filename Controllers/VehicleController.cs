using Microsoft.AspNetCore.Mvc;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Models;
using Microsoft.AspNetCore.Authorization;
using FeeCollectorApplication.Utility;
using System.Net;

namespace FeeCollectorApplication.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/vehicle")]
    public class VehicleController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApiResponse response;
        public VehicleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize(Roles =SD.Role_Admin)]
        [HttpGet]
        public IActionResult GetAllData()
        {
            var unit = _unitOfWork.Vehicle.GetAllAsync();
            response.Result = unit;
            response.StatusCode = HttpStatusCode.OK;
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
        [HttpGet("lp")]
        public async Task<IActionResult> GetDataByLicensePlate(string lp)
        {
            var obj = await _unitOfWork.Vehicle.GetFirstOrDefaultAsync(u => u.LicensePlate == lp);
            return Ok(obj);
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, VehicleUpsert obj)
        {
            var model = await _unitOfWork.Vehicle.GetFirstOrDefaultAsync(u => u.Id == id);
            if (model == null)
            {
                return NotFound();
            }
            model.Price = obj.Price;
            model.LicensePlate= obj.LicensePlate;
            _unitOfWork.Vehicle.Update(model);
            await _unitOfWork.Save();
            return Ok("Updated");
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var obj = await _unitOfWork.Vehicle.GetFirstOrDefaultAsync(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Vehicle.Remove(obj);
            await _unitOfWork.Save();
            return Ok("Deleted");
        }
    }
}
