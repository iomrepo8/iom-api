using System.Collections.Generic;

namespace IOM.Models.ApiControllerModels
{
    public class ChangeAccountModel
    {
        public int UserDetailsId { get; set; }
        public IList<int> TeamIds { get; set; }
        public IList<int> AccountIds { get; set; }
    }
}