using Whoami.Api.Models;

namespace Whoami.Api.Endpoints;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this WebApplication app)
    {
        app.MapGet("/api/projects", () =>
        {
            var projecten = new List<Project>
            {
                new Project
                {
                    Id = 1,
                    Titel = "Whoami Portfolio",
                    Beschrijving = "Mijn portfolio website voor WPFW",
                    Categorie = "Web Development",
                    GitHubUrl = "https://github.com/ZayroXDF/Whoami",
                    Datum = new DateTime(2026, 10, 1)
                }
            };

            return Results.Ok(projecten);
        });
    }
}