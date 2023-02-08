using Microsoft.AspNetCore.Mvc;
using FeeCollectorApplication.Models;
using Microsoft.EntityFrameworkCore;
using FeeCollectorApplication.DataAccess;
using FeeCollectorApplication.Models.IModels;

namespace MongoTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        //private readonly FeeCollectorService _feeCollectorService;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        public CategoryController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }
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
        //        string time = "12:00";
        //        var timeCvt = DateTime.ParseExact(time, "H:mm", null, System.Globalization.DateTimeStyles.None);
        //        timeCvt1 = timeCvt1 + timeCvt1;
        //    }
        //}
        [HttpGet("GetCarById")]
        public async Task<ActionResult<ICars>> GetCarById(int Id)
        {
            ICars car = await ApplicationDbContext.cars.FirstOrDefaultAsync(s => s.CarId == Id);
            if (User == null)
            {
                return NotFound();
            }
            else
            {
                return User;
            }
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetDataById(string id)
        //{
        //    var obj = await _db.GetAsync(id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(obj);
        //}
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(string id, Category cat)
        //{
        //    var obj = await _db.GetAsync(id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    cat._id = obj._id;
        //    await _db.UpdateAsync(id, cat);
        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(string id)
        //{
        //    var obj = await _db.GetAsync(id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    await _feeCollectorService.RemoveAsync(id);
        //    return Ok("Delete success!");
        //}
    }
}
