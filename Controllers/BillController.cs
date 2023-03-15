using Azure.Identity;
using FeeCollectorApplication.Models;
using FeeCollectorApplication.Models.Dto;
using FeeCollectorApplication.Repository;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FeeCollectorApplication.Controllers
{
    [Route("api/bill")]
    [Authorize]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IConfiguration _configuration;
        public BillController(IUnitOfWork unit, IConfiguration configuration)
        {
            _unit = unit;
            _configuration= configuration;
        }
        //[Authorize(Roles = SD.Role_Admin)]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllBills()
        {
            var model = await _unit.Bill.GetAllAsync();
            //await Console.Out.WriteLineAsync("-------------------------------- GET ALL BILLS ----------------------------");
            return Ok(model);
        }

        //[Authorize(Roles = SD.Role_Admin)]
        //[AllowAnonymous]
        //[HttpGet("{id:int}")]
        //public IActionResult GetBillById(int id)
        //{
        //    var model = _unit.Bill.GetFirstOrDefault(u => u.Id == id);
        //    if (model == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(model);
        //}

        [AllowAnonymous]
        [HttpGet("search/{lp}")]
        public async Task<IActionResult> GetAllBillsByLp(string lp)
        {
            var model = await _unit.Bill.GetAllAsync(u => u.LicensePlate == lp);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }
        [AllowAnonymous]
        //[Authorize("admin")]
        [HttpPost]
        public async Task<IActionResult> AddBill(BillUpsert obj)
        {
            var updateModel = await _unit.Vehicle.GetFirstOrDefaultAsync(u => u.LicensePlate == obj.LicensePlate);
            var vehicleTypeModel = await _unit.VehicleType.GetFirstOrDefaultAsync(u => u.Id == obj.VehicleTypeId);
            if (updateModel == null)
            {
                Vehicle newModel = new Vehicle()
                {
                    LicensePlate = obj.LicensePlate,
                    Price = vehicleTypeModel.Price,
                    ImagePath = obj.ImageUrl,
                    LastModified = DateTime.UtcNow.AddHours(7)
                };
                await _unit.Vehicle.Add(newModel);
                await _unit.Save();
            }
            else
            {
                updateModel.Price += vehicleTypeModel.Price;
                updateModel.ImagePath = obj.ImageUrl;
                _unit.Vehicle.Update(updateModel);
            }

            var vehicle = await _unit.Vehicle.GetFirstOrDefaultAsync(u => u.LicensePlate == obj.LicensePlate);

            DateTime timeStart = DateTime.UtcNow.AddHours(7);
            DateTime timeEnd = timeStart.AddHours(2);

            var newBillModel = new Bill()
            {
                CreatedTime = timeStart,
                EndTime = timeEnd,
                Fee = vehicleTypeModel.Price,
                Location = obj.Location,
                LicensePlate = obj.LicensePlate,
                ImageUrl = obj.ImageUrl,
                VehicleId = vehicle.Id,
                VehicleTypeId = obj.VehicleTypeId,
                Longtitude = obj.Longtitude,
                Latitude = obj.Latitude
            };

            await _unit.Bill.Add(newBillModel);
            await _unit.Save();
            string response = _configuration.GetValue<string>("DomainName:Domain") + "/api/information/" + newBillModel.LicensePlate;
            return Ok(response);
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBill(int id, BillUpsert obj)
        {
            var model = await _unit.Bill.GetFirstOrDefaultAsync(u => u.Id == id);
            if (model == null)
            {
                return BadRequest();
            }
            
            var newFee = await _unit.VehicleType.GetFirstOrDefaultAsync(u => u.Id == obj.VehicleTypeId);
            
            model.Location = obj.Location;
            model.ImageUrl = obj.ImageUrl;
            model.LicensePlate = obj.LicensePlate;
            model.VehicleTypeId = obj.VehicleTypeId;
            model.Fee = newFee.Price;
            _unit.Bill.Update(model);
            await _unit.Save();

            var updateVehicleModel = await _unit.Vehicle.GetFirstOrDefaultAsync(u => u.LicensePlate == obj.LicensePlate);
            var listBillOfVehicle = await _unit.Bill.GetAllAsync(u => u.LicensePlate == updateVehicleModel.LicensePlate);
            float totalFee = 0;
            foreach (var item in listBillOfVehicle)
            {
                totalFee += item.Fee;
            }
            updateVehicleModel.Price = totalFee;
            _unit.Vehicle.Update(updateVehicleModel);
            await _unit.Save();
            return Ok("Bill Updated");
        }
    }
}
