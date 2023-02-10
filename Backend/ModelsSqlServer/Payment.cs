using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeeCollectorApplication.ModelsSqlServer
{
    public class Payment
    {
        [Key]
        public int pay_id { get; set; }
        [Required]
        [StringLength(20)]
        public string license_plate_number { get; set; }
        [Required]
        public float paid_price { get; set; }
        [Required]
        public DateTime paid_time { get; set; }
    }
}
