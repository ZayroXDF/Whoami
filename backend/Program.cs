using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Whoami.Api.Data;
using Whoami.Api.Endpoints;
using Whoami.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Swagger registreren
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database registreren via Dependency Injection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Identity registreren voor gebruikers en rollen
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});


// Autorisatie registreren
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EditorOrAdmin", policy =>
        policy.RequireRole("Editor", "Admin"));
});


// Service registreren via Dependency Injection
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<BlogpostService>();

var app = builder.Build();


 // Rollen aanmaken als ze nog niet bestaan
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    string[] rollen = { "Gast", "Editor", "Admin" };

    foreach (var rol in rollen)
    {
        if (!await roleManager.RoleExistsAsync(rol))
        {
            var resultaat = await roleManager.CreateAsync(
                new IdentityRole(rol)
            );

            if (!resultaat.Succeeded)
            {
                throw new Exception(
                    $"Aanmaken van rol {rol} is mislukt."
                );
            }
        }
    }
}


// Authenticatie en autorisatie inschakelen
app.UseAuthentication();
app.UseAuthorization();

// Swagger inschakelen
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoints registreren
app.MapProjectEndpoints();
app.MapBlogpostEndpoints();
app.MapAuthEndpoints();

app.Run();

public partial class Program { }