using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using FeeCollectorApplication.Models;
using FeeCollectorApplication.Service;
using System.Collections.Specialized;

namespace MongoTest.Controllers
{
    public class CategoryController : Controller
    {
        private readonly FeeCollectorService _feeCollectorService;
        public CategoryController(FeeCollectorService feeCollectorService)
        {
            _feeCollectorService = feeCollectorService;
        }
        public IActionResult Index()
        {
            var model = _feeCollectorService.GetData();
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
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
                ViewBag.Message = "Employee updated successfully!";
                return RedirectToAction("Index");
            }
            ViewBag.Message = "Error while updating Employee!";
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
