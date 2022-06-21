using System;

namespace IntelART.CLRServices
{
    public class NORQResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Gender { get; set; }
        public string District { get; set; }
        public string Community { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Apartment { get; set; }
        public DocumentData NonBiometricPassport { get; set; }
        public DocumentData BiometricPassport { get; set; }
        public DocumentData IDCard { get; set; }
        public string SocialCardNumber { get; set; }
        public decimal Salary { get; set; }
    }
}
