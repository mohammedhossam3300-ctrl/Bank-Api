using Microsoft.AspNetCore.Mvc;

namespace Bank.Application.DTOs.Auth.TwoFactor;

public class VerifyTokenRequest
{
    [FromBody]
    public string Token { get; set; } = string.Empty;

    [FromHeader(Name = "User-Agent")]
    public string? UserAgent { get; set; }
}

