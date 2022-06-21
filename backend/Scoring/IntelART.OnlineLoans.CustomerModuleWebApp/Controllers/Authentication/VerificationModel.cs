using System;
using System.ComponentModel.DataAnnotations;

namespace IntelART.OnlineLoans.CustomerModuleWebApp.Controllers.Authentication
{
    public class VerificationModel
    {
        public Guid RegistrationProcessId { get; set; }
        public String Phone { get; set; }
        [Required(ErrorMessage = "Նույնականացման կոդը պետք է մուտքագրված լինի")]
        [RegularExpression("^(\\d){4}$", ErrorMessage = "Նույնականացման կոդը պետք է պարունակի չորս թվանշան")]
        public string VerificationCode { get; set; }

        public bool IsActive { get; set; }
    }
}
