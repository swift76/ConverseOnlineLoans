using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using IntelART.IdentityManagement;
using System.Linq;

namespace IntelART.IdentityServer
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private IUserStore userStore;

        public ResourceOwnerPasswordValidator(IUserStore userStore)
        {
            this.userStore = userStore;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            string ipAddress;
            if (context.Request.Raw.AllKeys.Contains("X-OL-IP"))
                ipAddress = context.Request.Raw["X-OL-IP"];
            else
                ipAddress = null;

            if (!this.userStore.ValidatePassword(context.UserName, context.Password, ipAddress))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.UnauthorizedClient, "Լրացված տվյալներով օգտագործող գրանցված չէ. խնդրում ենք մուտքագրել ճիշտ տվյալներ կամ գրանցվել որպես օգտագործող։");
            }
            else
            {
                UserInfo user = this.userStore.GetUserByUsername(context.UserName);
                context.Result = new GrantValidationResult(user.Id.ToString(), "custom");
            }
        }
    }
}
