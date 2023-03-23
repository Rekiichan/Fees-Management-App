using System.ComponentModel.DataAnnotations;

namespace FeeCollectorApplication.Models.Dto
{
    public class EmployeeResponse
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AvatarLink { get; set; }
        public string Address { get; set; }
        [Required]
        public string citizenIdentification { get; set; } //REQUIRED
    }
}
