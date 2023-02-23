using Microsoft.AspNetCore.Identity;

namespace FeeCollectorApplication.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        
    }
}
