namespace FeeCollectorApplication.Models.DtoModel
{
    public class CustomerRegisterRequestDTO
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Avatar { get; set; }
        public string Address { get; set; }
    }
}
