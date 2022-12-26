//using FeeCollectorApplication.Service;
//using Microsoft.AspNetCore.Mvc;

//namespace FeeCollectorApplication.Controllers
//{
//    [Route("uploadimage")]
//    [ApiController]
//    public class ApiController : ControllerBase
//    {
//        private readonly FeeCollectorService _feeCollectorService;
//        private readonly IWebHostEnvironment _hostEnvironment;
//        public ApiController(FeeCollectorService feeCollectorService, IWebHostEnvironment hostEnvironment)
//        {
//            _feeCollectorService = feeCollectorService;
//            _hostEnvironment = hostEnvironment;
//        }
//        [HttpPost]
//        [ProducesResponseType(StatusCodes.Status201Created)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public string AddImageByIdCar([FromForm]IFormFile file, [FromForm] string idCar, [FromForm] string time)
//        {
//            string? imagePath = "";
//            Console.WriteLine("start");
//            string wwwRootPath = _hostEnvironment.WebRootPath;
//            if (file != null)
//            {
//                Console.WriteLine("start");
//                string fileName = idCar + '-' + time;
//                var uploads = Path.Combine(wwwRootPath, @"images\");
//                var extension = Path.GetExtension(file.FileName);

//                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
//                {
//                    Console.WriteLine("start");
//                    file.CopyTo(fileStreams);
//                }
//                string ImageUrl = @"\images\" + fileName + extension;
//                imagePath = ImageUrl;
//                //_feeCollectorService.GetData().Where(u =>
//                //{
//                //    Console.WriteLine("start");
//                //    if (u.idCar == idCar)
//                //    {
//                //        Console.WriteLine(u.idCar);
//                //        u.image = ImageUrl;
//                //    }
//                //    return true;
//                //});
//            }
//            return imagePath;
//        }
//    }
//}
