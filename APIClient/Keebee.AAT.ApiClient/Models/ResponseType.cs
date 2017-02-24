using System.Collections.Generic;

namespace Keebee.AAT.ApiClient.Models
{
    public class ResponseType
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
        public ResponseTypeCategory ResponseTypeCategory { get; set; }
        public InteractiveActivityType InteractiveActivityType { get; set; }
    }

    public class ResponseTypeList
    {
        public IEnumerable<ResponseType> ResponseTypes;
    }
}
