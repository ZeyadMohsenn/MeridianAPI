using System;

namespace StoreManagement.Bases.Infrastructure;


public class UserState
{
    /// <summary>
    /// Gets And Sets Current User Id
    /// Value Sets On User MiddleWare
    /// </summary>
    public static Func<Guid> GetCurrentUserId;

    public static Guid UserId;

    public static Func<Guid> GetCurrentUserCountry;
}

