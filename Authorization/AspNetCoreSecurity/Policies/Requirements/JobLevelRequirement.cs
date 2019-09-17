using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreAuthentication.Policies
{
    public class JobLevelRequirement : IAuthorizationRequirement
    {
        public JobLevel Level { get; }

        public JobLevelRequirement(JobLevel level)
        {
            Level = level;
        }
    }

    public static class StatusPolicyBuilderExtensions
    {
        public static AuthorizationPolicyBuilder RequireJobLevel(this AuthorizationPolicyBuilder builder, JobLevel level)
        {
            builder.AddRequirements(new JobLevelRequirement(level));
            return builder;
        }
    }

    public enum JobLevel
    {
        Intern,
        Developer,
        Management,
        CxO
    }
}