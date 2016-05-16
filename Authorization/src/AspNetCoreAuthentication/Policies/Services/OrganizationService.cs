using System.Security.Claims;

namespace AspNetCoreAuthentication.Policies
{
    public interface IOrganizationService
    {
        JobLevel GetJobLevel(ClaimsPrincipal user);
    }

    public class OrganizationService : IOrganizationService
    {
        public JobLevel GetJobLevel(ClaimsPrincipal user)
        {
            return JobLevel.Developer;
        }
    }
}