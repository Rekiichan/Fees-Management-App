using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using FeeCollectorApplication.Models;
using FeeCollectorApplication.Service;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.ModelsSqlServer;
using System.Globalization;
using Microsoft.Extensions.Hosting;

namespace FeeCollectorApplication.Controllers
{
    [ApiController]
    [Route("api/vehicle")]
    public class VehicleController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public VehicleController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        [HttpGet]
        public IActionResult GetAllData()
        {
            var unit = _unitOfWork.Vehicle.GetAll();
            return Ok(unit);
        }
        [HttpGet("{id}")]
        public IActionResult GetDataById(int id)
        {
            var obj = _unitOfWork.Vehicle.GetFirstOrDefault(u => u.Vehicle_id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }
        [HttpPost]
        public IActionResult RequestVehicle([FromForm]VehicleUpsert obj, [FromForm]IFormFile file)
        {
            if (obj == null || file == null)
            {
                return BadRequest();
            }
            DateTime timeStart = DateTime.Now;
            DateTime timeEnd = timeStart.AddHours(2);
            var model = _unitOfWork.Vehicle.GetAll(u => u.license_plate_number == obj.license_plate_number).ToArray();
            for (int i = model.Count() - 1; i >= 0; i--)
            {
                if (model[i].license_plate_number == obj.license_plate_number)
                {
                    if (model[i].time_end.CompareTo(timeStart) >= 0)
                    {
                        return NoContent();
                    }
                }
                if (model[i].time_end.CompareTo(timeStart) <= 0)
                {
                    break;
                }
            }

            string imgUrl = AddImageOfVehicle(file, obj.license_plate_number, timeStart);
            var temp = new Vehicle()
            {
                license_plate_number = obj.license_plate_number,
                time_start = timeStart,
                time_end = timeEnd,
                image_url = imgUrl,
                vehicle_type = obj.vehicle_type,
                location = obj.location
            };

            _unitOfWork.Vehicle.Add(temp);
            float priceOfVehicleType = _unitOfWork.VehicleType.GetFirstOrDefault(u => u.vehicle_type == obj.vehicle_type).price;

            // check that obj have already had ?
            Bill billCheck = _unitOfWork.Bill.GetFirstOrDefault(u => u.license_plate_number == obj.license_plate_number);

            // Trigger when vehicle need to update
            if (billCheck != null)
            {
                billCheck.price += priceOfVehicleType;
                BillTriggerUpdate(billCheck);
            }
            else
            {
                BillTriggerInsert(obj.license_plate_number, priceOfVehicleType);
            }
            BillHistoryTrigger(new BillHistory()
            {
                price = priceOfVehicleType,
                license_plate_number = obj.license_plate_number,
                Bill_datetime = DateTime.Now
            });
            // Save scoped
            _unitOfWork.Save();
            return Ok("Created");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteById(int id)
        {
            var obj = _unitOfWork.Vehicle.GetFirstOrDefault(u => u.Vehicle_id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Vehicle.Remove(obj);
            return Ok("Deleted");
        }

        #region function_trigger
        private void BillTriggerUpdate(Bill obj)
        {
            _unitOfWork.Bill.Update(obj);
        }
        private void BillTriggerInsert(string lpn, float price)
        {
            var billModel = new Bill()
            {
                license_plate_number = lpn,
                price = price
            };
            _unitOfWork.Bill.Add(billModel);
        }

        private void BillHistoryTrigger(BillHistory obj)
        {
            _unitOfWork.BillHistory.Add(obj);
        }
        #endregion

        #region function_process

        private string AddImageOfVehicle([FromBody] IFormFile file, string lpn, DateTime time_start)
        {
            string? imagePath = "";
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = lpn + '-' + time_start.ToString("yyyy-MM-dd-HH-mm");
                var uploads = Path.Combine(wwwRootPath, @"images\");
                var extension = Path.GetExtension(file.FileName);

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                string ImageUrl = @"\images\" + fileName + extension;
                imagePath = ImageUrl;
            }
            return imagePath;
        }

        #endregion
    }
}
