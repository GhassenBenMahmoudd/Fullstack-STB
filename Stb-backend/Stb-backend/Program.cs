using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using stb_backend.Data;
using stb_backend.Interfaces;
using stb_backend.Middleware;
using stb_backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Force la conversion des enums en chaînes de caractères.
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        // Parfois nécessaire, indique comment gérer les noms de propriétés.
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
// Enregistrement de ton service ici ⬇
builder.Services.AddScoped<IDeclarationCadeauService, DeclarationCadeauService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "STB API",
        Version = "v1"
    });

    // 1. Définir le schéma de sécurité (Security Scheme)
    //    On explique à Swagger comment l'authentification fonctionne.
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        // Description pour les développeurs qui utiliseront l'API
        Description = @"Veuillez entrer 'Bearer' [espace] puis votre token dans le champ ci-dessous.
                      Exemple: 'Bearer 12345abcdef'",
        Name = "Authorization", // Le nom du header HTTP
        In = ParameterLocation.Header, // L'emplacement du header (dans l'en-tête de la requête)
        Type = SecuritySchemeType.ApiKey, // Le type de schéma
        Scheme = "Bearer" // Le nom du schéma
    });

    // 2. Appliquer ce schéma de sécurité à toutes les opérations qui le nécessitent
    //    On dit à Swagger d'ajouter le header "Authorization" à ses requêtes.
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // Doit correspondre à l'ID défini dans AddSecurityDefinition
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Ajout du DbContext
builder.Services.AddDbContext<StbDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DÉBUT DE LA CONFIGURATION CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200") // L'URL de votre app Angular
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
// --- DÉBUT DE LA CONFIGURATION JWT ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
// --- FIN DE LA CONFIGURATION JWT ---


// Build après l'ajout de tous les services
var app = builder.Build();

// Migration automatique de la base
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StbDbContext>();
    context.Database.Migrate();
}

// Swagger en développement uniquement
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "STB API v1");
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseErrorHandlingMiddleware();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// ON APPLIQUE LA POLITIQUE CORS
app.UseCors("AllowAngularApp");
app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers(); // ✅ AJOUT


app.Run();