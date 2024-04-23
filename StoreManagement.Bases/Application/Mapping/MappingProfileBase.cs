using AutoMapper;
using StoreManagement.Bases.Infrastructure;
using System;

namespace StoreManagement.Bases;

public class MappingProfileBase : Profile
{
    public MappingProfileBase()
    {
        SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        ReplaceMemberName("_", "");
    }
    public static string GetCurrentLanguage()
    {
        return System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
    }
    public bool IsArabic()
    {
        return System.Threading.Thread.CurrentThread.CurrentCulture.ToString() == "ar"; //Language.Arabic;
    }
    public Guid UserId => UserState.GetCurrentUserId();
}

