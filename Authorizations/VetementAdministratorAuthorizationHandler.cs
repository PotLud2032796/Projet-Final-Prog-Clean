using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualBasic;
using Atelier.Models;

namespace Atelier.Authorizations
{
    public class VetementAdministratorAuthorizationHandler
                    : AuthorizationHandler<OperationAuthorizationRequirement, Vetement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Vetement resource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            // Les administrateurs ont tous les droits
            if (context.User.IsInRole(AuthorizationConstants.VetementAdministratorsRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}