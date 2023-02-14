namespace FeeCollectorApplication.Models
{
    public class BillUpsert
    {
        public string Location { get; set; }
        public string LicensePlate { get; set; }
        public int VehicleTypeId { get; set; }
        public string ImageUrl { get; set; }
    }
}
