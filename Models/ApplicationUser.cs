using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FeeCollectorApplication.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string citizenIdentification { get; set; }
        [StringLength(100)]
        public string Avatar { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        public ICollection<Bill> Bills { get; set; }
    }
}
