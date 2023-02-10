using System.ComponentModel.DataAnnotations;

namespace FeeCollectorApplication.ModelsSqlServer
{
    public class VehicleType
    {
        [Key]
        public int Id { get; set; }
        public string vehicle_type { get; set; }
        public float price { get; set; }
    }
}
