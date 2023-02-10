﻿using FeeCollectorApplication.Models;
using FeeCollectorApplication.ModelsSqlServer;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Service;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FeeCollectorApplication.Controllers
{
    [ApiController]
    [Route("api/Payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FeeCollectorService _feeCollectorService;
        public PaymentController(IUnitOfWork unitOfWork, FeeCollectorService feeCollectorService)
        {
            _unitOfWork = unitOfWork;
            _feeCollectorService = feeCollectorService;
        }
        [HttpGet("{lpn}")]
        public IActionResult GetLpn(string lpn)
        {
            var dataFromMongoDb = _feeCollectorService.GetData();
            if (dataFromMongoDb.Count > 0)
            {
                updateDatabase(dataFromMongoDb);
            }
            var model = _unitOfWork.Bill.GetFirstOrDefault(u => u.license_plate_number == lpn);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }
        [HttpPost]
        public IActionResult RequestPayment(PaymentUpsert payment)
        {
            if (payment == null)
            {
                return BadRequest();
            }
            var obj = _unitOfWork.Bill.GetFirstOrDefault(u => u.license_plate_number == payment.license_plate_number);
            if (obj == null)
            {
                return Ok("This vehicle has paid the full fee");
            }
            var model = new Payment()
            {
                license_plate_number = payment.license_plate_number,
                paid_price = payment.paid_price,
                paid_time = DateTime.Now
            };
            var billModel = _unitOfWork.Bill.GetFirstOrDefault(u => u.license_plate_number == payment.license_plate_number);
            if (billModel != null)
            {
                if (billModel.price > 0)
                {
                    _unitOfWork.Payment.Add(model);
                    billModel.price -= payment.paid_price;
                    if (billModel.price == 0)
                    {
                        _unitOfWork.Bill.Remove(billModel);
                        _unitOfWork.Save();
                    }
                    else if (billModel.price > 0)
                    {
                        _unitOfWork.Bill.Update(billModel);
                    }
                    else
                    {
                        return Ok("The paid fee is greater than the required fee");
                    }
                }
                else
                {
                    return NotFound("This license plate paid");
                }
            }
            else
            {
                return NotFound("cannot find that license plate in bill");
            }
            _unitOfWork.Save();
            return Ok("Paid " + payment.paid_price.ToString());
        }

        #region process_function
        private void updateDatabase(List<Category> model)
        {
            int n = model.Count();
            for (int i = 0; i < n; i++)
            {
                string timeStart = model[i].date + ' ' + model[i].time;
                DateTime time_start = DateTime.ParseExact(timeStart, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                DateTime time_end = time_start.AddHours(2);

                _unitOfWork.Vehicle.Add(new Vehicle
                {
                    image_url = model[i].image,
                    license_plate_number = model[i].idCar,
                    vehicle_type = model[i].type,
                    time_start = time_start,
                    time_end = time_end,
                    location = model[i].location
                });
            }
            _feeCollectorService.RemoveAll();
            _unitOfWork.Save();
        }

        #endregion
    }
}
