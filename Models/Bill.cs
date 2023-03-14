using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeeCollectorApplication.Models
{
    public class Bill
    {
        [Key]
        public int Id { get; set; }
        public float Fee { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        [Required]
        public string LicensePlate { get; set; }
        [StringLength(60)]
        public string ImageUrl { get; set; }
        public int VehicleId { get; set; }
        [ForeignKey("VehicleId")]
        public Vehicle Vehicle { get; set; }
        [Required]
        public int VehicleTypeId { get; set; }
        [ForeignKey("VehicleTypeId")]
        public VehicleType VehicleType { get; set; }
        public float Longtitude { get; set; }
        public float Latitude { get; set; }
        public string PaymentUrl { get; set; }
    }
}
