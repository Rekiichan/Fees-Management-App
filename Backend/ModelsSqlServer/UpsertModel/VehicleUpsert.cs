using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeeCollectorApplication.ModelsSqlServer
{
    public class VehicleUpsert
    {
        [Required]
        [StringLength(20)]
        public string license_plate_number { get; set; }
        [StringLength(200)]
        public string image_url{ get; set; }
        [Required]
        [StringLength(30)]
        public string vehicle_type { get; set; }
        [Required]
        public string location { get; set; }
    }
}
