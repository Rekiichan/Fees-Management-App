using Azure.Identity;
using FeeCollectorApplication.Models;
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
        //[Authorize("Admin")]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAllBills()
        {
            var model = _unit.Bill.GetAll();
            return Ok(model);
        }
        //[Authorize(Roles = SD.Role_Admin)]
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
        [HttpGet("{lp}")]
        public IActionResult GetBillByLp(string lp)
        {
            var model = _unit.Bill.GetAll(u => u.LicensePlate == lp);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult AddBill(BillUpsert obj)
        {
            var updateModel = _unit.Vehicle.GetFirstOrDefault(u => u.LicensePlate == obj.LicensePlate);
            float fee = _unit.VehicleType.GetFirstOrDefault(u => u.Id == obj.VehicleTypeId).Price;
            if (updateModel == null)
            {
                Vehicle newModel = new Vehicle()
                {
                    LicensePlate = obj.LicensePlate,
                    Price = fee
                };
                _unit.Vehicle.Add(newModel);
                _unit.Save();
            }
            else
            {
                updateModel.Price += fee;
                _unit.Vehicle.Update(updateModel);
            }

            var vehicleId = _unit.Vehicle.GetFirstOrDefault(u => u.LicensePlate == obj.LicensePlate).Id;
            DateTime timeStart = DateTime.Now;
            DateTime timeEnd = timeStart.AddHours(2);
            var newBillModel = new Bill()
            {
                TimeStart = timeStart,
                TimeEnd = timeEnd,
                Fee = fee,
                Location = obj.Location,
                LicensePlate = obj.LicensePlate,
                ImageUrl = obj.ImageUrl,
                VehicleId = vehicleId,
                VehicleTypeId = obj.VehicleTypeId
            };
            _unit.Bill.Add(newBillModel);
            _unit.Save();
            // TODO: UPDATE LINK FROM FRONT END
            //return Ok($"{domainName}/api/vehicle/" + newBillModel.LicensePlate);
            string response = _configuration.GetValue<string>("DomainName:Domain") + "/api/information/" + newBillModel.LicensePlate;
            return Ok(response);
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPut("{id:int}")]
        public IActionResult UpdateBill(int id, BillUpsert obj)
        {
            var model = _unit.Bill.GetFirstOrDefault(u => u.Id == id);
            if (model == null)
            {
                return BadRequest();
            }
            model.Location = obj.Location;
            model.ImageUrl = obj.ImageUrl;
            model.LicensePlate = obj.LicensePlate;
            model.VehicleTypeId = obj.VehicleTypeId;
            _unit.Bill.Update(model);
            _unit.Save();
            return Ok("Bill Updated");
        }
    }
}
