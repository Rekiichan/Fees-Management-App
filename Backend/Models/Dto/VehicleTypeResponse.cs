using System.ComponentModel.DataAnnotations;

namespace FeeCollectorApplication.Models.Dto
{
    public class VehicleTypeResponse
    {
        public string Id { get; set; }
        public string VehicleTypeName { get; set; }
        public float Price { get; set; }
    }
}
