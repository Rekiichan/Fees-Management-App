using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeeCollectorApplication.ModelsSqlServer
{
    public class BillHistory
    {
        [Key]
        public int Bill_id { get; set; }
        [Required]
        [StringLength(20)]
        public string license_plate_number { get; set; }
        public DateTime? Bill_datetime { get; set;}
        [Required]
        public float price { get; set; }
    }
}
