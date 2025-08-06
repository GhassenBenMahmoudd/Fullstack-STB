using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using stb_backend.Data;
using stb_backend.DTOs;
using stb_backend.Domain;
using Microsoft.AspNetCore.Authorization;

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
        if (string.IsNullOrEmpty(loginDto.Password) ||
            (string.IsNullOrEmpty(loginDto.Email) && string.IsNullOrEmpty(loginDto.Matricule)))
        {
            return BadRequest(new { message = "Veuillez fournir un mot de passe et un email ou matricule." });
        }

        User? baseUser = null;

        if (!string.IsNullOrEmpty(loginDto.Email))
        {
            baseUser = _context.Users
                .SingleOrDefault(u =>
                    u.Email.ToLower().Trim() == loginDto.Email.ToLower().Trim() &&
                    u.Password.Trim() == loginDto.Password.Trim()
                );
        }
        else if (!string.IsNullOrEmpty(loginDto.Matricule))
        {
            baseUser = _context.Users
                .OfType<Employe>()
                .SingleOrDefault(u =>
                    u.Matricule != null &&
                    u.Matricule.ToLower().Trim() == loginDto.Matricule.ToLower().Trim() &&
                    u.Password.Trim() == loginDto.Password.Trim()
                );
        }

        if (baseUser == null)
        {
            return Unauthorized(new { message = "Identifiants invalides." });
        }

        Employe? user = baseUser as Employe;
        string userRole = "User";
        string roleDescription = "Utilisateur standard";

        if (user is Manager)
        {
            userRole = "Manager";
            roleDescription = "Manager - Accès complet à toutes les fonctionnalités";
        }
        else if (user != null)
        {
            userRole = "Employe";
            roleDescription = "Employé - Peut déclarer des cadeaux et consulter ses déclarations";
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, baseUser.IdUser.ToString()),
        new Claim(ClaimTypes.Name, $"{baseUser.Prenom} {baseUser.Nom}"),
        new Claim("prenom", baseUser.Prenom),
        new Claim("nom", baseUser.Nom),
        new Claim("email", baseUser.Email),
        new Claim(ClaimTypes.Role, userRole),
        new Claim("role", userRole)
    };

        if (user?.Matricule != null)
        {
            claims.Add(new Claim("matricule", user.Matricule));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = tokenString,
            user = new
            {
                id = baseUser.IdUser,
                prenom = baseUser.Prenom,
                nom = baseUser.Nom,
                email = baseUser.Email,
                matricule = user?.Matricule,
                role = userRole,
                roleDescription = roleDescription,
                permissions = GetUserPermissions(userRole)
            },
            message = $"Connexion réussie. Rôle: {userRole}"
        });
    }

    private object GetUserPermissions(string role)
    {
        return role switch
        {
            "Manager" => new
            {
                canDeclareGifts = true,
                canViewAllDeclarations = true,
                canUpdateStatus = true,
                canArchive = true,
                canDelete = true,
                canViewReports = true
            },
            "Employe" => new
            {
                canDeclareGifts = true,
                canViewAllDeclarations = false,
                canUpdateStatus = false,
                canArchive = false,
                canDelete = false,
                canViewReports = false
            },
            _ => new
            {     
                canDeclareGifts = false,
                canViewAllDeclarations = false,
                canUpdateStatus = false,
                canArchive = false,
                canDelete = false,
                canViewReports = false
            }
        };
    }
}