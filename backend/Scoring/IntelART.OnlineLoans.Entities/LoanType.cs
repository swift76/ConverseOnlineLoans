﻿namespace IntelART.OnlineLoans.Entities
{
    public class LoanType : DirectoryEntity
    {
        public string STATE { get; set; }
        public bool IS_OVERDRAFT { get; set; }
        public bool IS_CARD_ACCOUNT { get; set; }
    }
}
