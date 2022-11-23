using Microsoft.AspNetCore.Mvc;
using FeeCollectorApplication.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using FeeCollectorApplication.Service;

namespace FeeCollectorApplication.Controllers
{
    public class PieChartController : Controller
    {
        private readonly FeeCollectorService _feeCollectorService;
        public PieChartController(FeeCollectorService feeCollectorService)
        {
            _feeCollectorService = feeCollectorService;
        }
        // GET: Home
        public ActionResult Index()
        {

            List<Category> category = _feeCollectorService.GetData();
            double countYes = 0;
            double countNo = 0;
            foreach (var item in category)
            {
                if (item.pendingStatus == true)
                {
                    countYes++;
                }
                else
                {
                    countNo++;
                }
            }
            double sum = 0;
            sum = countNo + countYes;
            countYes = countYes / sum * 100;
            countNo = countNo / sum * 100;

            List<PieChart> dataPoints = new List<PieChart>
            {
                new PieChart("Paid", countYes),
                new PieChart("Not Paid", countNo)
            };

            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            return View();
        }
    }
}