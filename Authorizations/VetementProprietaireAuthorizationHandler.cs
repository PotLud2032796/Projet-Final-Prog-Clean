using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Atelier.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace Atelier.Authorizations
{
    public class VetementProprietaireAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Vetement>
    {
        UserManager<IdentityUser> _userManager;

        public VetementProprietaireAuthorizationHandler(
            UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Vetement resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // Les actions possibles sont CRUD
            if (requirement.Name != AuthorizationConstants.CreateOperationName &&
                requirement.Name != AuthorizationConstants.ReadOperationName &&
                requirement.Name != AuthorizationConstants.UpdateOperationName &&
                requirement.Name != AuthorizationConstants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }

            if (resource.ProprietaireId == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}