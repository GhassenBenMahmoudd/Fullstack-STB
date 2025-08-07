using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;

namespace stb_backend.Middleware
{
    public class UserActivityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserActivityMiddleware> _logger;

        public UserActivityMiddleware(RequestDelegate next, ILogger<UserActivityMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log de la requête entrante
            var originalBodyStream = context.Response.Body;

            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            try
            {
                await _next(context);

                // Log après le traitement de la requête
                await LogUserActivity(context);
            }
            finally
            {
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(originalBodyStream);
            }
        }

        private async Task LogUserActivity(HttpContext context)
        {
            var user = context.User;
            var isAuthenticated = user.Identity?.IsAuthenticated ?? false;

            if (isAuthenticated)
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                var userName = user.FindFirstValue(ClaimTypes.Name);
                var userRole = user.FindFirstValue(ClaimTypes.Role);
                var userEmail = user.FindFirstValue("email");
                var userMatricule = user.FindFirstValue("matricule");

                var logData = new
                {
                    Timestamp = DateTime.UtcNow,
                    UserId = userId,
                    UserName = userName,
                    UserRole = userRole,
                    UserEmail = userEmail,
                    UserMatricule = userMatricule,
                    RequestPath = context.Request.Path,
                    RequestMethod = context.Request.Method,
                    ResponseStatusCode = context.Response.StatusCode,
                    IPAddress = context.Connection.RemoteIpAddress?.ToString()
                };

                var logMessage = JsonSerializer.Serialize(logData, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });

                _logger.LogInformation("Activité utilisateur: {LogData}", logMessage);

                // Afficher le rôle dans la console pour le développement
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    Console.WriteLine($"=== ACTIVITÉ UTILISATEUR ===");
                    Console.WriteLine($"Utilisateur: {userName}");
                    Console.WriteLine($"Rôle: {userRole}");
                    Console.WriteLine($"Email: {userEmail}");
                    Console.WriteLine($"Matricule: {userMatricule}");
                    Console.WriteLine($"Action: {context.Request.Method} {context.Request.Path}");
                    Console.WriteLine($"Statut: {context.Response.StatusCode}");
                    Console.WriteLine("=============================");
                }
            }
            else
            {
                _logger.LogInformation("Requête anonyme: {Method} {Path} - Statut: {StatusCode}", 
                    context.Request.Method, 
                    context.Request.Path, 
                    context.Response.StatusCode);
            }
        }
    }

    // Extension method pour faciliter l'utilisation du middleware
    public static class UserActivityMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserActivityLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserActivityMiddleware>();
        }
    }
} 