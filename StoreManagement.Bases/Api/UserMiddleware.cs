using StoreManagement.Bases.Enums;
using StoreManagement.Bases.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StoreManagement.Bases.Api;

public class UserMiddleware
{
    private readonly RequestDelegate next;

    public UserMiddleware(RequestDelegate next)
    {
        this.next = next;
    }
    public async Task Invoke(HttpContext context)
    {
        if (context.User is not null)
        {
            var value = context.User.FindFirst(ClaimTypes.NameIdentifier);
            UserState.GetCurrentUserId = delegate ()
            {

                return Guid.Parse(value?.Value ?? Guid.Empty.ToString());

            };

            UserState.UserId = Guid.Parse(value?.Value ?? Guid.Empty.ToString());

            UserState.GetCurrentUserCountry = delegate ()
            {
                var value = context.User.FindFirst(CustomClaims.Country.ToString());

                return Guid.Parse(value?.Value ?? Guid.Empty.ToString());
            };
        }
        await next(context);
    }
}

