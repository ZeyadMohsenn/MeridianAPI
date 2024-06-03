namespace StoreManagement.Domain.Login_Token;

public class RegisterDto
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
}
public enum UserRole
{
    Cashier,
    Admin
}
