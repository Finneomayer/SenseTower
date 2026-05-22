using IdentityServer0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityServer0.Controllers;

public class AccountController:Controller
{
    // тестовые данные вместо использования базы данных
    private readonly List<Person> _people = new()
    {
        new Person {Login="admin", Password="12345" },
        new Person { Login="user", Password="55555"}
    };

    [HttpPost("stub/authstub")]
    public IActionResult Token([FromBody]LoginDto loginDto)
    {


        var identity = GetIdentity(loginDto.Login, loginDto.Password);
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (identity == null)
        {
            return Unauthorized();
        }

        var now = DateTime.UtcNow;
        // создаем JWT-токен
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            

        return Ok(encodedJwt);
    }

    private ClaimsIdentity GetIdentity(string username, string password)
    {
#pragma warning disable CS8600
        var person = _people.FirstOrDefault(x => x.Login == username && x.Password == password);
#pragma warning restore CS8600
        // ReSharper disable once InvertIf
        if (person != null)
        {
            var claims = new List<Claim>
            {
                new (ClaimsIdentity.DefaultNameClaimType, person.Login)         
            };
            var claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

        // если пользователя не найдено
#pragma warning disable CS8603
        return null;
#pragma warning restore CS8603
    }
    public class LoginDto
    {
#pragma warning disable CS8618
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Login { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Password { get; set; }
#pragma warning restore CS8618
    }
}