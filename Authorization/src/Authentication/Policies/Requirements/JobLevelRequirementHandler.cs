using Microsoft.AspNet.Authorization;

namespace Authentication.Policies
{
    public class StatusRequirementHandler : AuthorizationHandler<JobLevelRequirement>
    {
        private readonly IOrganizationService _service;

        public StatusRequirementHandler(IOrganizationService service)
        {
            _service = service;
        }
        
        protected override void Handle(AuthorizationContext context, JobLevelRequirement requirement)
        {
            var currentLevel = _service.GetJobLevel(context.User);

            if (currentLevel == requirement.Level)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}