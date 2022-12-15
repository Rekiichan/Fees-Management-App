using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using FeeCollectorApplication.Models;
using FeeCollectorApplication.Service;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace MongoTest.Controllers
{
    public class CategoryController : Controller
    {
        private readonly FeeCollectorService _feeCollectorService;
        public CategoryController(FeeCollectorService feeCollectorService)
        {
            _feeCollectorService = feeCollectorService;
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
        public IActionResult Index()
        {
            var model = _feeCollectorService.GetData();
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        public IActionResult Insert()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Insert(Category cat)
        {
            _feeCollectorService.Create(cat);
            ViewBag.Message = "Collector added successfully!";
            return View();
        }

        public IActionResult Update(string id)
        {
            var cat = _feeCollectorService.Get(id);
            return View(cat);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(string id, Category cat)
        {
            cat._id = new ObjectId(id).ToString();
            var filter = Builders<Category>.Filter.Eq("_id", cat._id);
            var updateDef = Builders<Category>.Update.Set("idCar", cat.idCar);
            updateDef = updateDef.Set("type", cat.type);
            updateDef = updateDef.Set("date", cat.date);
            updateDef = updateDef.Set("location", cat.location);
            updateDef = updateDef.Set("pendingStatus", cat.pendingStatus);
            var result = _feeCollectorService._categoryCollection.UpdateOne(filter, updateDef);
            if (result.IsAcknowledged)
            {
                return RedirectToAction("Index");
            }
            return View(cat);
        }
        public IActionResult ConfirmDelete(string id)
        {
            var cat = _feeCollectorService.Get(id);
            return View(cat);
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            ObjectId oId = new ObjectId(id);
            var result = _feeCollectorService._categoryCollection.DeleteOne<Category>(e => e._id == oId.ToString());

            if (result.IsAcknowledged)
            {
                TempData["Message"] = "Employee deleted successfully!";
            }
            else
            {
                TempData["Message"] = "Error while deleting Employee!";
            }
            return RedirectToAction("Index");
        }
    }
}
