using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreAuthentication.Policies
{
    public class JobLevelRequirementHandler : AuthorizationHandler<JobLevelRequirement>
    {
        private readonly IOrganizationService _service;

        public JobLevelRequirementHandler(IOrganizationService service)
        {
            _service = service;
        }
        
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, JobLevelRequirement requirement)
        {
            var currentLevel = _service.GetJobLevel(context.User);

            if (currentLevel == requirement.Level)
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}