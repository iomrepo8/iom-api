namespace IOM.Models.ApiControllerModels
{
    public class IPWhitelist
    {
        public int Id { get; set; }
        public string Alias { get; set; }
        public string IPAddress { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}