namespace WebStore.ViewModels.VM;

public class JwtOptionsVm
{
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string SecretKey { get; set; } = default!;
    public int TokenExpirationMinutes { get; set; }
}