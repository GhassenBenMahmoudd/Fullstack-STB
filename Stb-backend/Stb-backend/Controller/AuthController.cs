using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using stb_backend.Data;
using stb_backend.Domain;
using stb_backend.DTOs;
using stb_backend.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace stb_backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly StbDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public AuthController(StbDbContext context, IConfiguration configuration, EmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        // ====================== REGISTER ======================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var emailLower = registerDto.Email.ToLower().Trim();
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == emailLower);
            if (existingUser != null)
                return BadRequest("Un compte avec cet e-mail existe déjà.");

            var tempPassword = GenerateTemporaryPassword();
            var user = new User
            {
                Nom = registerDto.Nom.Trim(),
                Prenom = registerDto.Prenom.Trim(),
                Email = emailLower,
                Password = BCrypt.Net.BCrypt.HashPassword(tempPassword),
                NumeroTelephone = registerDto.NumeroTelephone,
                IsVerified = false,
                VerificationToken = Guid.NewGuid().ToString(),
                VerificationTokenExpiry = DateTime.UtcNow.AddHours(24)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var verificationUrl = $"http://localhost:4200/verify-email?token={user.VerificationToken}";
            var loginUrl = "http://localhost:4200/login";

            var emailBody = $@"
        <h3>Bonjour {user.Prenom},</h3>
        <p>Merci pour votre inscription.</p>
        <p>Voici votre mot de passe temporaire (valable pour votre première connexion) :</p>
        <p><strong>{tempPassword}</strong></p>
        <p>Veuillez vérifier votre compte en cliquant ici :</p>
        <a href='{verificationUrl}'>Activer mon compte</a>
        <p>Ensuite, connectez-vous ici et changez votre mot de passe :</p>
        <a href='{loginUrl}'>Se connecter à l'application</a>
        <p>Ce lien de vérification expirera dans 24 heures.</p>";

            await _emailService.SendEmailAsync(user.Email, "Vérification et mot de passe temporaire", emailBody);

            return Ok(new { message = "Inscription réussie. Vérifiez votre e-mail." });
        }



        // ====================== VERIFY EMAIL ======================
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token manquant.");

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.VerificationToken == token && !u.IsVerified);

            if (user == null || (user.VerificationTokenExpiry.HasValue && user.VerificationTokenExpiry < DateTime.UtcNow))
                return BadRequest("Token invalide ou expiré.");

            user.IsVerified = true;
            user.VerificationToken = null;
            user.VerificationTokenExpiry = null;
            await _context.SaveChangesAsync();

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

            var redirectUrl = $"http://localhost:4200/verified?token={tokenString}";
            return Redirect(redirectUrl);
        }

        // ====================== LOGIN ======================
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Password) ||
                (string.IsNullOrWhiteSpace(loginDto.Email) && string.IsNullOrWhiteSpace(loginDto.Matricule)))
            {
                return BadRequest(new { message = "Veuillez fournir un mot de passe et un email ou matricule." });
            }

            User? baseUser = null;

            if (!string.IsNullOrEmpty(loginDto.Email))
            {
                var emailLower = loginDto.Email.ToLower().Trim();
                baseUser = _context.Users
                    .SingleOrDefault(u => u.Email.ToLower().Trim() == emailLower);
            }
            else if (!string.IsNullOrEmpty(loginDto.Matricule))
            {
                baseUser = _context.Users
                    .OfType<Employe>()
                    .SingleOrDefault(u =>
                        u.Matricule != null &&
                        u.Matricule.ToLower().Trim() == loginDto.Matricule.ToLower().Trim()
                    );
            }

             if (baseUser == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, baseUser.Password))
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

        // ====================== PERMISSIONS ======================
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
        private string GenerateTemporaryPassword(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@$!%*?&";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }

}
