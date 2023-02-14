﻿using System.ComponentModel.DataAnnotations;

namespace FeeCollectorApplication.Models
{
    public class VehicleType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string VehicleTypeName { get; set; }
        [Required]
        public float Price { get; set; }
    }
}
