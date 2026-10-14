namespace LimitedStockOrdering.Api.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int AdminLifetimeMinutes { get; set; } = 480;
    public int GuestLifetimeMinutes { get; set; } = 43200;
}
