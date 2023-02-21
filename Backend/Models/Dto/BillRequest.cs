namespace FeeCollectorApplication.Models.Dto
{
    public class BillRequest
    {
        public string Location { get; set; }
        public string LicensePlate { get; set; }
        public string VehicleTypeId { get; set; }
        public string ImageUrl { get; set; }

    }
}
