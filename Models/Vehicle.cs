using System.ComponentModel.DataAnnotations;

namespace FeeCollectorApplication.Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }
        [StringLength(20)]
        public string LicensePlate { get; set; }
        public float Price { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime LastModified { get; set; } = DateTime.UtcNow.AddHours(7);
        public string ImagePath { get; set; }
    }
}
