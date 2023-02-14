using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeeCollectorApplication.Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }
        [StringLength(20)]
        public string LicensePlate { get; set; }
        public float Price { get; set; } = 0;
    }
}
