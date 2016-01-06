using Microsoft.AspNet.Authorization.Infrastructure;

namespace Authentication.Policies
{
    public static class CustomerOperations
    {
        public static OperationAuthorizationRequirement Manage = 
            new OperationAuthorizationRequirement { Name = "Manage" };
        public static OperationAuthorizationRequirement SendMail =
            new OperationAuthorizationRequirement { Name = "SendMail" };

        public static OperationAuthorizationRequirement GiveDiscount(int amount)
        {
            return new DiscountOperationAuthorizationRequirement
            {
                Name = "GiveDiscount",
                Amount = amount
            };
        }
    }

    public class DiscountOperationAuthorizationRequirement : OperationAuthorizationRequirement
    {
        public int Amount { get; set; }
    }
}