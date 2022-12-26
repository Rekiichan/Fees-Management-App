//using Microsoft.AspNetCore.Mvc;
//using FeeCollectorApplication.Models;
//using Newtonsoft.Json;
//using FeeCollectorApplication.Service;
//using System.Runtime.Serialization;
//using MongoDB.Bson;

//namespace FeeCollectorApplication.Controllers
//{
//    [ApiController]
//    [Route("/api/[controller]")]
//    public class PieChartController : ControllerBase
//    {
//        private readonly FeeCollectorService _feeCollectorService;
//        public PieChartController(FeeCollectorService feeCollectorService)
//        {
//            _feeCollectorService = feeCollectorService;
//        }
//        // GET: Home
//        public ActionResult Index()
//        {

//            List<Category> category = _feeCollectorService.GetData();
//            double countYes = 0;
//            double countNo = 0;
//            foreach (var item in category)
//            {
//                if (item.pendingStatus == true)
//                {
//                    countYes++;
//                }
//                else
//                {
//                    countNo++;
//                }
//            }

//            double sum = 0;
//            sum = countNo + countYes;
//            countYes = countYes / sum * 100;
//            countNo = countNo / sum * 100;

//            List<PendingStatus> dataPoints_PendingStatus = new List<PendingStatus>
//            {
//                new PendingStatus("Paid", Math.Round(countYes,1)),
//                new PendingStatus("Not Paid", Math.Round(countNo,1))
//            };

//            return Ok(dataPoints_PendingStatus);
//        }

//        public ActionResult Index2()
//        {
//            List<Category> category = _feeCollectorService.GetData();
            
//            double check_16 = 0;
//            double check_30 = 0;
//            double check_16_30 = 0;

//            foreach (var item in category)
//            {
//                if (item.type.Contains("16"))
//                {
//                    if (item.type.Contains("30"))
//                    {
//                        check_16_30++;
//                    }
//                    else
//                    {
//                        check_16++;
//                    }
//                }
//                else
//                {
//                    check_30++;
//                }
//            }
//            double sum = (check_16 + check_16_30 + check_30);
//            check_16 = check_16 / sum * 100;
//            check_16_30 = check_16_30 / sum * 100;
//            check_30 = check_30 / sum * 100;

//            List<TypeCar> dataPoints_TypeCar = new List<TypeCar>
//            {
//                new TypeCar("Cars under 16 seats ", Math.Round(check_16,1)),
//                new TypeCar("Cars from 16 seats to 30 seats", Math.Round(check_16_30,1)),
//                new TypeCar("Cars above 30 seats", Math.Round(check_30,1))
//            };

//            return Ok(dataPoints_TypeCar);
//        }

//        [DataContract]
//        public class TypeCar
//        {
//            public TypeCar(string type, double value)
//            {
//                this.y = value;
//                this.label = type;
//            }

//            //Explicitly setting the name to be used while serializing to JSON.
//            [DataMember(Name = "label")]
//            public string label = "";

//            //Explicitly setting the name to be used while serializing to JSON.
//            [DataMember(Name = "y")]
//            public double y = 0;
//        }

//        [DataContract]
//        public class PendingStatus
//        {
//            public PendingStatus(string pendingStatus, double value)
//            {
//                this.y = value;
//                this.label = pendingStatus;
//            }

//            //Explicitly setting the name to be used while serializing to JSON.
//            [DataMember(Name = "label")]
//            public string label = "";

//            //Explicitly setting the name to be used while serializing to JSON.
//            [DataMember(Name = "y")]
//            public double y = 0;
//        }
//    }
//}