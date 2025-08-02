using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using stb_backend.Data; // Adaptez le using pour votre DbContext
using stb_backend.DTOs;
using stb_backend.Domain;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly StbDbContext _context;

    public AuthController(IConfiguration configuration, StbDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        // IMPORTANT : En production, ne jamais stocker les mots de passe en clair.
        // Utilisez un système de hash (ex: ASP.NET Identity).
        var user = _context.Employes.SingleOrDefault(u =>
                    u.Matricule.ToLower() == loginDto.Matricule.ToLower() &&
                    u.Password == loginDto.Password
                );

        if (user == null)
        {
            return Unauthorized(new { message = "Identifiant ou mot de passe invalide." });
        }
        string userRole;
        if (user is Manager)
        {
            userRole = "Manager";
        }
        else
        {
            // Puisque l'objet vient de _context.Employes et n'est pas un Manager,
            // c'est un Employe "standard".
            userRole = "Employe";
        }
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
            new Claim("matricule", user.Matricule), // Ajout de la matricule dans les claims
            new Claim(ClaimTypes.Name, $"{user.Prenom} {user.Nom}"),
            new Claim(ClaimTypes.Role, userRole)

            // Ajoutez le rôle de l'utilisateur. C'est crucial !
            // new Claim(ClaimTypes.Role, user.Role) 
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1), // Le jeton est valide pour 1 jour
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { token = tokenString, userName = user.Prenom, userRole = userRole  });
    }
}
