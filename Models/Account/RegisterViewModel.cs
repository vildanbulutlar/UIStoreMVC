using Domain.Enums;

public class RegisterViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    // Bunlar kalsın ama formdan gelmeyecek:
    public MembershipType MembershipType { get; set; } = MembershipType.Standard;
    public string? CompanyName { get; set; }
    public string? TaxNumber { get; set; }
}
