﻿using Azure.Identity;
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
        [Authorize("Admin")]
        //[AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllBills()
        {
            var model = await _unit.Bill.GetAllAsync();
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
        [HttpGet("{lp}")]
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
        //[Authorize("Admin")]
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
                    LastModified = DateTime.Now
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
            
            DateTime timeStart = DateTime.Now;
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
                VehicleTypeId = obj.VehicleTypeId
            };

            await _unit.Bill.Add(newBillModel);
            await _unit.Save();
            // TODO: UPDATE LINK FROM FRONT END
            //return Ok($"{domainName}/api/vehicle/" + newBillModel.LicensePlate);
            string response = _configuration.GetValue<string>("DomainName:Domain") + "/api/information/" + newBillModel.LicensePlate;
            return Ok(response);
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBill(int id, BillUpsert obj)
        {
            var model = await _unit.Bill.GetFirstOrDefaultAsync(u => u.Id == id);
            if (model == null)
            {
                return BadRequest();
            }
            model.Location = obj.Location;
            model.ImageUrl = obj.ImageUrl;
            model.LicensePlate = obj.LicensePlate;
            model.VehicleTypeId = obj.VehicleTypeId;
            _unit.Bill.Update(model);
            await _unit.Save();
            return Ok("Bill Updated");
        }
    }
}
