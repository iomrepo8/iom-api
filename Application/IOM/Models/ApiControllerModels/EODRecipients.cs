using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{
    public class EODRecipients: EODReportModel
    {
        public IList<Recipient> Recipients { get; set; }
        public Confirmation ConfirmationURL { get; set; }

    }

    public class Recipient 
    {
        public string Name { get; set; }

        public string Email { get; set; }
        public bool? IsAllowed { get; set; }
    }

    public class Confirmation
    {
        public string ReturnURLWithParams { get; set; }
        public List<string> Params { get; set; }
    }
}