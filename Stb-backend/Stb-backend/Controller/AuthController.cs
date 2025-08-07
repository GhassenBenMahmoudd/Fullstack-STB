using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using stb_backend.Data;
using stb_backend.DTOs;
using stb_backend.Domain;
using Microsoft.AspNetCore.Authorization;
using stb_backend.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

[Route("api/[controller]")]
[ApiController]

public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly StbDbContext _context;

    public AuthController(IConfiguration configuration, StbDbContext context, EmailService emailService)
    {
        _configuration = configuration;
        _context = context;
        _emailService = emailService;

    }
    private readonly EmailService _emailService;

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

        if (user is Manager)
            userRole = "Manager";
        else if (user != null)
            userRole = "Employe";

        var claims = new List<Claim>
{
    new Claim("id", baseUser.IdUser.ToString()),
    new Claim("prenom", baseUser.Prenom),
    new Claim("nom", baseUser.Nom),
    new Claim("email", baseUser.Email),
    new Claim("role", userRole)
};


        if (user?.Matricule != null)
            claims.Add(new Claim("matricule", user.Matricule));

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
            userName = $"{baseUser.Prenom} {baseUser.Nom}",
            userRole
        });
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
        if (existingUser != null)
            return BadRequest("Un compte avec cet e-mail existe déjà.");

        var user = new User
        {
            Nom = registerDto.Nom,
            Prenom = registerDto.Prenom,
            Email = registerDto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            NumeroTelephone = registerDto.NumeroTelephone,
            IsVerified = false,
            VerificationToken = Guid.NewGuid().ToString(),
            VerificationTokenExpiry = DateTime.UtcNow.AddHours(24)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // ⚠️ Modifie ici vers ton vrai domaine frontend
        var verificationUrl = $"https://http://localhost:4200/verify-email?token={token}";

        var emailBody = $@"
        <h3>Bonjour {user.Prenom},</h3>
        <p>Merci pour votre inscription.</p>
        <p>Veuillez cliquer sur le lien ci-dessous pour activer votre compte :</p>
        <a href='{verificationUrl}'>Activer mon compte</a>
        <p>Ce lien expirera dans 24 heures.</p>";

        await _emailService.SendEmailAsync(user.Email, "Vérification de votre compte", emailBody);

        return Ok(new { message = "Inscription réussie. Vérifiez votre e-mail." });
    }
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail(string token)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.VerificationToken == token && !u.IsVerified);

        if (user == null || user.VerificationTokenExpiry < DateTime.UtcNow)
            return BadRequest("Token invalide ou expiré.");

        user.IsVerified = true;
        user.VerificationToken = null;
        user.VerificationTokenExpiry = null;
        await _context.SaveChangesAsync();

        // Génère le token JWT
        var claims = new List<Claim>
    {
        new Claim("id", user.IdUser.ToString()),
        new Claim("prenom", user.Prenom),
        new Claim("nom", user.Nom),
        new Claim("email", user.Email),
        new Claim("role", "User")
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        // 🔁 Redirige vers ton frontend avec le token JWT
        var redirectUrl = $"https://http://localhost:4200/verified?token={tokenString}";
        return Redirect(redirectUrl);
    }




}

