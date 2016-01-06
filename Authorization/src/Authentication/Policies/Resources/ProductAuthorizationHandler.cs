using Authentication.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Authorization.Infrastructure;

namespace Authentication.Policies
{
    public class ProductAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Product>
    {
        protected override void Handle(AuthorizationContext context, OperationAuthorizationRequirement requirement, Product resource)
        {
            context.Succeed(requirement);
        }
    }
}