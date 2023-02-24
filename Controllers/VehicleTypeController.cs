using FeeCollectorApplication.Models;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [AllowAnonymous]
        public async Task<IActionResult> GetAllVehicleType()
        {
            var model = await _unitOfWork.VehicleType.GetAllAsync();
            await Console.Out.WriteLineAsync("GET ALL VEHICLE TYPE SUCCESS!!!");
            return Ok(model);
        }
        //[AllowAnonymous]
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public async Task<IActionResult> AddVehicleType(VehicleTypeUpsert obj)
        {
            if (obj == null)
            {
                await Console.Out.WriteLineAsync("OBJECT IS NULL!!");
                return BadRequest();
            }
            await Console.Out.WriteLineAsync("OBJ IS NOT NULL!!!");
            var temp = new VehicleType()
            {
                VehicleTypeName = obj.VehicleTypeName,
                Price = obj.Price
            };
            await _unitOfWork.VehicleType.Add(temp);
            await Console.Out.WriteLineAsync("ADD VEHICLE TYPE SUCCESS!!!");
            await _unitOfWork.Save();
            await Console.Out.WriteLineAsync("SAVE VEHICLE TYPE SUCCESS!!!");
            return Ok("Created " + obj.VehicleTypeName);
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditVehicleType(int id, VehicleTypeUpsert obj)
        {
            if (obj == null)
            {
                await Console.Out.WriteLineAsync("REQUEST OBJ IS NULL!!");
                return BadRequest();
            }
            await Console.Out.WriteLineAsync("---------------------------OBJ IS NOT NULL-------------------------------");
            var model = await _unitOfWork.VehicleType.GetFirstOrDefaultAsync(u => u.Id == id);
            if (model == null)
            {
                await Console.Out.WriteLineAsync("---------------------------CANNOT FIND THAT VEHICLE TYPE ID-------------------------------");
                return BadRequest();
            }
            await Console.Out.WriteLineAsync("---------------------------FOUND-------------------------------");
            model.Price = obj.Price;
            model.VehicleTypeName = obj.VehicleTypeName;
            _unitOfWork.VehicleType.Update(model);
            await Console.Out.WriteLineAsync($"---------------------------UPDATED VEHICLE TYPE {id}-------------------------------");
            await _unitOfWork.Save();
            await Console.Out.WriteLineAsync($"---------------------------SAVED-------------------------------");
            return Ok("Updated");
        }
    }
}
