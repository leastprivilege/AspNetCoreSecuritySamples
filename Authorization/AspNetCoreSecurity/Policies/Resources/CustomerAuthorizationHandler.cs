using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Threading.Tasks;

namespace AspNetCoreAuthentication.Policies
{
    public class CustomerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Customer>
    {
        private readonly IPermissionService _permissions;

        public CustomerAuthorizationHandler(IPermissionService permissions)
        {
            _permissions = permissions;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Customer resource)
        {
            var nop = Task.CompletedTask;

            // user must be in sales
            if (!context.User.HasClaim("department", "sales"))
            {
                return nop;
            }

            // ...and responsible for customer region
            if (!context.User.HasClaim("region", resource.Region))
            {
                return nop;
            }

            // if customer is fortune 500 - sales rep must be senior
            if (resource.Fortune500)
            {
                if (!context.User.HasClaim("status", "senior"))
                {
                    return nop;
                }
            }

            if (requirement.Name == "GiveDiscount")
            {
                HandleDiscountOperation(context, requirement, resource);
                return nop;
            }

            context.Succeed(requirement);
            return nop;
        }

        private void HandleDiscountOperation(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Customer resource)
        {
            var discountOperation = requirement as DiscountOperationAuthorizationRequirement;
            var salesRep = context.User.FindFirst("sub").Value;

            var result = _permissions.IsDiscountAllowed(
                salesRep,
                resource.Id,
                discountOperation.Amount);

            if (result)
            {
                context.Succeed(requirement);
            }
        }
    }
}