using Microsoft.IdentityModel.Tokens;
using System.Text;
// ReSharper disable InconsistentNaming

namespace IdentityServer0;

public static class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // издатель токена
    // ReSharper disable once InconsistentNaming
    public const string AUDIENCE = "MyAuthClient"; // потребитель токена
    private const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
    public const int LIFETIME = 60; // время жизни токена - 60 минута
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
    }
}