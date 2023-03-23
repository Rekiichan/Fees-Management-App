using FeeCollectorApplication.Models;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace FeeCollectorApplication.Controllers
{
    [Route("api/bill")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        public BillController(IUnitOfWork unit, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _unit = unit;
            _configuration = configuration;
            _userManager = userManager;
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllBills()
        {
            var model = await _unit.Bill.GetAllAsync();
            return Ok(model);
        }

        //[AllowAnonymous]
        [Authorize(Roles = SD.Role_Admin)]
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

        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [HttpGet("get-bill-based-on-emp")]
        public async Task<IActionResult> GetAllBillByEmp()
        {
            // Get claim from User that get from ControlerBase
            var claimIdentities = User.Claims.ToList();

            // Get id from claim
            var EmployeeId = claimIdentities[1].Value;

            // get all bills of employee
            var model = await _unit.Bill.GetAllAsync(u => u.UserId == EmployeeId);

            return Ok(model);
        }
        //[AllowAnonymous]
        //[Authorize(Roles = SD.Role_Employee)]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
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

            // Get claim from User that get from ControlerBase
            var claimIdentities = User.Claims.ToList();

            // Get id from claim
            var EmployeeId = claimIdentities[1].Value;

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
                Latitude = obj.Latitude,
                UserId = EmployeeId
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
