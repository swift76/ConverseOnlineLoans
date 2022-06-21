using System.Collections.Generic;

namespace IntelART.CLRServices
{
    public class ACRAResponse
    {
        public string FicoScore { get; set; }
        public List<ACRAQueryResultDetails> Details { get; set; }
        public List<ACRAQueryResultQueries> Queries { get; set; }

        public ACRAResponse()
        {
            FicoScore = string.Empty;
            Details = new List<ACRAQueryResultDetails>();
            Queries = new List<ACRAQueryResultQueries>();
        }
    }
}
