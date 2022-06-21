using System;
using System.ComponentModel.DataAnnotations;

namespace IntelART.OnlineLoans.CustomerModuleWebApp.Controllers.Authentication
{
    public class PasswordResetModel
    {
        public Guid RegistrationProcessId { get; set; }
        public String Phone { get; set; }
        [Required(ErrorMessage = "Նույնականացման կոդը պետք է մուտքագրված լինի")]
        [RegularExpression("^(\\d){4}$", ErrorMessage = "Նույնականացման կոդը պետք է պարունակի չորս թվանշան")]
        public string VerificationCode { get; set; }
        [Required(ErrorMessage = "Գաղտնաբառը պարտադիր է")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[\\da-zA-Z]).{8,}$", ErrorMessage = "Գաղտնաբառը պետք է լինի լատինատառ, պարունակի առնվազն ութ նիշ, մեկ մեծատառ, մեկ փոքրատառ և մեկ թվանշան")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Գաղտնաբառի կրկնությունը պարտադիր է")]
        [Compare("Password", ErrorMessage = "Մուտքագրված գաղտնաբառերի անհամապատասխանություն")]
        public string ConfirmPassword { get; set; }
    }
}
