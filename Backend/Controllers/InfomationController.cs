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
    [Route("api/[controller]")]
    public class InfomationController : ControllerBase
    {
        private readonly FeeCollectorService _feeCollectorService;
        private readonly IUnitOfWork _unitOfWork;
        public InfomationController(FeeCollectorService feeCollectorService, IUnitOfWork unitOfWork)
        {
            _feeCollectorService = feeCollectorService;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public IActionResult GetAllData()
        {
            var model = _feeCollectorService.GetData();
            var unit = _unitOfWork.Vehicle.GetAll();
            if (unit.Count() < model.Count())
            {
                updateDatabase(model, unit);
                unit = _unitOfWork.Vehicle.GetAll();
            }
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
        [HttpGet("{LicensePlateNumber}")]
        public IActionResult GetDataByLicensePlate(string lp)
        {
            var obj = _unitOfWork.Vehicle.GetAll().FirstOrDefault(u => u.license_plate_number == lp);
            if (obj == null)
            {
                var unit = _unitOfWork.Vehicle.GetAll();
                var model = _feeCollectorService.GetData();
                if (unit.Count() < model.Count())
                {
                    updateDatabase(model, unit);
                    unit = _unitOfWork.Vehicle.GetAll();
                }
                else
                {
                    return NotFound();
                }
            }
            return Ok(obj);

        }
        //[HttpPut("{id}")]
        //public IActionResult Update(int id, Vehicle vehicle)
        //{
        //    var obj = _unitOfWork.Vehicle.GetFirstOrDefault(u => u.Vehicle_id == id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
    
        //    _unitOfWork.Vehicle.Update(vehicle);
        //    _unitOfWork.Save();
        //    return NoContent();
        //}

        [HttpDelete("{id}")]
        public IActionResult DeleteById(int id)
        {
            var obj = _unitOfWork.Vehicle.GetFirstOrDefault(u => u.Vehicle_id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Vehicle.Remove(obj);
            return Ok("Delete success!");
        }
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
        //        string time_start = "12:00";
        //        var timeCvt = DateTime.ParseExact(time_start, "H:mm", null, System.Globalization.DateTimeStyles.None);
        //        timeCvt1 = timeCvt1 + timeCvt1;
        //    }
        //}

        private void updateDatabase(List<Category> model, IEnumerable<Vehicle> unit)
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
                    car_type = model[i].type,
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
