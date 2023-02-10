using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using FeeCollectorApplication.Models;
using FeeCollectorApplication.Service;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.ModelsSqlServer;
using System.Globalization;

namespace FeeCollectorApplication.Controllers
{
    [ApiController]
    [Route("api/vehicle")]
    public class VehicleController : ControllerBase
    {
        private readonly FeeCollectorService _feeCollectorService;
        private readonly IUnitOfWork _unitOfWork;
        public VehicleController(FeeCollectorService feeCollectorService, IUnitOfWork unitOfWork)
        {
            _feeCollectorService = feeCollectorService;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public IActionResult GetAllData()
        {
            var model = _feeCollectorService.GetData();
            if (model.Count() > 0)
            {
                updateDatabase(model);
            }
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
        public IActionResult RequestVehicle(VehicleUpsert obj)
        {
            if (obj == null)
            {
                return BadRequest();
            }
            DateTime timeStart = DateTime.Now;
            DateTime timeEnd = timeStart.AddHours(2);
            var model = _unitOfWork.Vehicle.GetAll().ToArray();
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
            var temp = new Vehicle()
            {
                license_plate_number = obj.license_plate_number,
                time_start = timeStart,
                time_end = timeEnd,
                image_url = obj.image_url,
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

        #endregion

        #region function_process

        //public string timeProccess(string time1, string time2)
        //{
        //    bool checkPM1 = false;
        //    bool checkPM2 = false;

        //    if (time1.Contains("PM"))
        //    {
        //        checkPM1 = true;
        //    }
        //    if (time2.Contains("PM"))
        //    {
        //        checkPM2 = true;
        //    }

        //    time1.Remove(5);
        //    time2.Remove(5);

        //    var timeCvt1 = DateTime.ParseExact(time1, "H:mm", null, System.Globalization.DateTimeStyles.None);
        //    var timeCvt2 = DateTime.ParseExact(time2, "H:mm", null, System.Globalization.DateTimeStyles.None);

        //    if (checkPM1)
        //    {
        //        string timeStart = "12:00";
        //        var timeCvt = DateTime.ParseExact(timeStart, "H:mm", null, System.Globalization.DateTimeStyles.None);
        //        timeCvt1 = timeCvt1 + timeCvt1;
        //    }
        //}

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
            _unitOfWork.Save();
        }
        #endregion
    }
}
