using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DealRealState.API.Authorization
{
    public class SellerOrSelfHandler
        : AuthorizationHandler<SellerOrSelfRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SellerOrSelfRequirement requirement)
        {

            var userIdClaim =
                context.User.FindFirst(
                    ClaimTypes.NameIdentifier
                );


            var roleClaim =
                context.User.FindFirst(
                    ClaimTypes.Role
                );


            if (userIdClaim == null)
                return Task.CompletedTask;



            // Seller can access all users
            if (roleClaim?.Value == "seller")
            {
                context.Succeed(requirement);

                return Task.CompletedTask;
            }



            // Get id from route
            if (context.Resource is HttpContext httpContext)
            {
                var requestedUserId =
                    httpContext.Request.RouteValues["id"]
                    ?.ToString();


                if (requestedUserId ==
                    userIdClaim.Value)
                {
                    context.Succeed(requirement);
                }
            }


            return Task.CompletedTask;
        }
    }
}