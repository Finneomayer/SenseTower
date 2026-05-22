using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using SC.SenseTower.Auth.Models;
using System.Security.Claims;

namespace SC.SenseTower.Auth.Validators
{
    public class AccessGrantedToTokenValidator : ICustomTokenRequestValidator
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AccessGrantedToTokenValidator(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            if (context.Result.ValidatedRequest.GrantType != "password")
                return;
            var principal = context.Result.ValidatedRequest.Subject;
            var userId = principal.FindFirstValue("sub");
            var user = await userManager.FindByIdAsync(userId);
            if (user.AccessGrantedTo != null && user.AccessGrantedTo.Value <= DateTime.UtcNow)
            {
                context.Result.IsError = true;
                context.Result.Error = "Действие билета закончилось, для постоянного доступа получите приглашение";
                context.Result.ErrorDescription = "Предоставленный срок доступа превышен";
            }
        }
    }
}
