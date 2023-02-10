using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeeCollectorApplication.ModelsSqlServer
{
    public class BillUpsert
    {
        [Required]
        [StringLength(20)]
        public string license_plate_number { get; set; }
        [Required]
        public float price { get; set; }
    }
}
