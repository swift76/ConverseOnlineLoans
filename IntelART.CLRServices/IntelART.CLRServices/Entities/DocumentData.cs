using System;

namespace IntelART.CLRServices
{
    public class DocumentData
    {
        public string Number { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string IssuedBy { get; set; }

        public bool IsDocumentValid()
        {
            return (!string.IsNullOrEmpty(Number) && ExpiryDate >= DateTime.Now.Date);
        }
    }
}
