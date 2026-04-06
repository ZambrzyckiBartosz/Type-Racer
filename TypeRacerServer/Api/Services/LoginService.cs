using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TypeRacerServer.Api.Requests;

namespace TypeRacerServer.Api.Services;

public class LoginService(AppDbContext _context, IConfiguration _configuration)
{
    public async Task<string> LoginHandler(LoginRequest loginRequest)
    {
        Console.WriteLine("Login Service Active");
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginRequest.Username);

        if(user == null){
            throw new Exception("User not found");
        }
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password,user.PasswordHash);
        if(!isPasswordValid){
            throw new Exception("Wrong password");
        }

        var TokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "ojapidifido;suf833238fkjadsal");
        var tokenDescriptor = new SecurityTokenDescriptor{
            Subject = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, user.Username)}),
            Expires = DateTime.UtcNow.AddDays(3),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = TokenHandler.CreateToken(tokenDescriptor);
        var tokenString = TokenHandler.WriteToken(token);
        Console.WriteLine("Login service deactive");
        return tokenString;
    }
}