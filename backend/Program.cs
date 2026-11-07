using Whoami.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Swagger registreren
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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