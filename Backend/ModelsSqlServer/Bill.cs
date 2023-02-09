using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeeCollectorApplication.ModelsSqlServer
{
    public class Bill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Bill_id { get; set; }
        [Required]
        [StringLength(20)]
        public string license_plate_number { get; set; }
        [Required]
        public float price { get; set; }
    }
}
