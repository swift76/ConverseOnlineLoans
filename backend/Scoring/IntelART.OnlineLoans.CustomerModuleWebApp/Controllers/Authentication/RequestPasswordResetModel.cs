using System.ComponentModel.DataAnnotations;

namespace IntelART.OnlineLoans.CustomerModuleWebApp.Controllers.Authentication
{
    public class RequestPasswordResetModel
    {
        [Required(ErrorMessage = "Հեռախոսը պարտադիր է")]
        [RegularExpression("^[ ]*(\\d\\s*){8}$", ErrorMessage = "Հեռախոսահամարը պետք է պարունակի ութ թվանշան")]
        public string Phone { get; set; }
    }
}
