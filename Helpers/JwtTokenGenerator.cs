/*
Developer: Abdullah Alshamrani
FirstBrick Project
VillaCapital
*/

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens; // for validating security tokens.
using System.Text; //helps in converting strings to byte arrays encoding.

namespace FirstBrickAPI.Helpers;

//static helper class responsible for generating JWT tokens.
public static class JwtTokenGenerator
{
    //creating the token.
    public static string GenerateToken(int userId, string username, string fullName, string role, string key, string issuer, string audience)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.ASCII.GetBytes(key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", userId.ToString()),
                new Claim("username", username),
                new Claim("fullName", fullName),
                new Claim(ClaimTypes.Role, role) // Add the role claim.
            }),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
        };

// GENERATE the JWT token.
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
