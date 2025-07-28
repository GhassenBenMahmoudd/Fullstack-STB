using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using stb_backend.Data;
using stb_backend.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "STB API",
        Version = "v1"
    });
});

// Ajout du DbContext
builder.Services.AddDbContext<StbDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

app.UseAuthorization();

app.MapRazorPages();

app.Run();
