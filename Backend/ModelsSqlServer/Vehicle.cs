using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeeCollectorApplication.ModelsSqlServer
{
    public class Vehicle
    {
        [Key]
        public int Vehicle_id { get; set; }
        [Required]
        [StringLength(20)]
        public string license_plate_number { get; set; }
        [Required]
        [StringLength(200)]
        public string image_url { get; set; }
        [Required]
        public string vehicle_type { get; set; }
        [Required]
        [StringLength(20)]
        public DateTime time_start { get; set; }
        [StringLength(60)]
        public DateTime time_end { get; set; }
        [Required]
        public string location { get; set; }

    }
}
