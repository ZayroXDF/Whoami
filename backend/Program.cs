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

// Service registreren via Dependency Injection
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<BlogpostService>();

var app = builder.Build();

// Swagger inschakelen
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoints registreren
app.MapProjectEndpoints();
app.MapBlogpostEndpoints();

app.Run();