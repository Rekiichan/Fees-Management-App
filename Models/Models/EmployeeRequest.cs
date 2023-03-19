using System.ComponentModel.DataAnnotations;

namespace FeeCollectorApplication.Models
{
    public class EmployeeRequest
    {
        public int Id { get; set; }

        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AvatarLink { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string citizenIdentification { get; set; } //REQUIRED

        public DateTime RequestAt { get; set; } = DateTime.UtcNow.AddHours(7);
    }
}
