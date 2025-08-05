namespace ModelContextProtocol.AspNetCore.Stateless;

public sealed record UserIdClaim(string Type, string Value, string Issuer);
