using System;

namespace StoreManagement.Bases;

public class UserModel
{
    public string FullNameAr { get; set; }
    public string FullNameEn { get; set; }
    public string UserName { get; set; }
    public Guid Id { get; set; }
    public string? UserTypeCode { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}
public class MobileUserModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
}

public class TokenSettings
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}

