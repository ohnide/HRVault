using System.Security.Cryptography;
using System.Text;
using HRVault.Application.Common.Interfaces;

namespace HRVault.Infrastructure.Security;

public class RefreshTokenService
    : IRefreshTokenService
{
    public string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }

    public string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);

        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}