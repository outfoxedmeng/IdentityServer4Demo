using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HybridMvcClient.Auth
{
    /// <summary>
    /// 若一个Handler成功，其他的Handler没有失败，则Requirement满足
    /// 若任何一个Handler失败，则Requirement失败
    /// 若没有Handler失败，且没有Handler成功，则Requirement失败
    /// 总结: Requirement满足的条件是：没有Handler失败，且至少有一个Handler成功
    /// </summary>
    public class SmithInSomewhereHandler : AuthorizationHandler<SmithInSomwhereRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SmithInSomwhereRequirement requirement)
        {
            var user = context.User;

            if (user.Identity.IsAuthenticated && user.HasClaim(JwtClaimTypes.FamilyName, "Smith") && user.HasClaim("location", "somewhere"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            //否则失败
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
