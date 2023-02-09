using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeeCollectorApplication.ModelsSqlServer
{
    public class PaymentUpsert
    {
        public string license_plate_number { get; set; }
        public float paid_price { get; set; }
    }
}
