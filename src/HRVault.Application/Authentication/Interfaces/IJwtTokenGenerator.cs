using HRVault.Application.Authentication.DTOs;

namespace HRVault.Application.Authentication.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(JwtUser user);
}