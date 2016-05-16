using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AspNetCoreAuthentication.Policies
{
    public class ProductAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Product>
    {
        protected override void Handle(AuthorizationContext context, OperationAuthorizationRequirement requirement, Product resource)
        {
            context.Succeed(requirement);
        }
    }
}