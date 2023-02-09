using FeeCollectorApplication.Models;
using FeeCollectorApplication.ModelsSqlServer;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Service;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FeeCollectorApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FeeCollectorService _feeCollectorService;
        public PaymentController(IUnitOfWork unitOfWork, FeeCollectorService feeCollectorService)
        {
            _unitOfWork = unitOfWork;
            _feeCollectorService = feeCollectorService;
        }
        [HttpGet("{license_plate_number}")]
        public IActionResult GetLpn(string license_plate_number)
        {
            var dataFromMongoDb = _feeCollectorService.GetData();
            if (dataFromMongoDb.Count > 0)
            {
                updateDatabase(dataFromMongoDb);
            }
            var model = _unitOfWork.Bill.GetFirstOrDefault(u => u.license_plate_number == license_plate_number);
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
            _unitOfWork.Payment.Add(model);
            try
            {
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return NoContent();
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
