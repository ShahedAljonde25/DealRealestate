using Microsoft.AspNetCore.Authorization;

namespace DealRealState.API.Authorization
{
    public class SellerOrSelfRequirement : IAuthorizationRequirement
    {
    }
}