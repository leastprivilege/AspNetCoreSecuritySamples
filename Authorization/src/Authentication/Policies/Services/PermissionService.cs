namespace Authentication.Policies
{
    public interface IPermissionService
    {
        bool IsDiscountAllowed(string subject, int customerId, int amount);
    }

    public class PermissionService : IPermissionService
    {
        public bool IsDiscountAllowed(string subject, int customerId, int amount)
        {
            return (amount < 10);
        }
    }
}