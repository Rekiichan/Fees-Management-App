namespace FeeCollectorApplication.Models.DtoModel
{
    public class LoginResponseDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        //public string citizenIdentification { get; set; }
    }
}
