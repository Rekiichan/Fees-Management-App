using System.ComponentModel.DataAnnotations;

namespace FeeCollectorApplication.Models
{
    public class VehicleTypeUpsert
    {
        public string VehicleTypeName { get; set; }
        public float Price { get; set; }
    }
}
