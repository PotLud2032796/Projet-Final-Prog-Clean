using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Atelier.Authorizations
{
    public static class VetementOperations
    {
        public static OperationAuthorizationRequirement Create = new() { Name = AuthorizationConstants.CreateOperationName };
        public static OperationAuthorizationRequirement Read = new() { Name = AuthorizationConstants.ReadOperationName };
        public static OperationAuthorizationRequirement Update = new() { Name = AuthorizationConstants.UpdateOperationName };
        public static OperationAuthorizationRequirement Delete = new() { Name = AuthorizationConstants.DeleteOperationName };
    }
}