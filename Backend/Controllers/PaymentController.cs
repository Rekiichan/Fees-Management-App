using FeeCollectorApplication.ModelsSqlServer;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Service;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FeeCollectorApplication.Controllers
{
    [ApiController]
    [Route("api/Payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("{lpn}")]
        public IActionResult GetPaymentByLpn(string lpn)
        {
            var model = _unitOfWork.Payment.GetAll(u => u.license_plate_number == lpn);
            if (model == null)
            {
                return BadRequest();
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
            var billModel = _unitOfWork.Bill.GetFirstOrDefault(u => u.license_plate_number == payment.license_plate_number);
            if (billModel != null)
            {
                if (billModel.price > 0)
                {
                    _unitOfWork.Payment.Add(model);
                    billModel.price -= payment.paid_price;
                    if (billModel.price == 0)
                    {
                        _unitOfWork.Bill.Remove(billModel);
                        _unitOfWork.Save();
                    }
                    else if (billModel.price > 0)
                    {
                        _unitOfWork.Bill.Update(billModel);
                    }
                    else
                    {
                        return Ok("The paid fee is greater than the required fee");
                    }
                }
                else
                {
                    return NotFound("This license plate paid");
                }
            }
            else
            {
                return NotFound("cannot find that license plate in bill");
            }
            _unitOfWork.Save();
            return Ok("Paid " + payment.paid_price.ToString());
        }
    }
}
