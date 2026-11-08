using Whoami.Api.DTOs;
using Whoami.Api.Services;

namespace Whoami.Api.Endpoints;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this WebApplication app)
    {
        app.MapGet("/api/projects", async (ProjectService service) =>
        {
            var projects = await service.GetAllAsync();

            var result = projects.Select(project => new ProjectResponseDto
            {
                Id = project.Id,
                Titel = project.Titel,
                Beschrijving = project.Beschrijving,
                Categorie = project.Categorie,
                GitHubUrl = project.GitHubUrl,
                Datum = project.Datum
            }).ToList();

            return Results.Ok(result);
        });

        app.MapPost("/api/projects", async (
            ProjectCreateDto dto,
            ProjectService service) =>
        {
            var project = await service.CreateAsync(dto);

            var result = new ProjectResponseDto
        {
            Id = project.Id,
            Titel = project.Titel,
            Beschrijving = project.Beschrijving,
            Categorie = project.Categorie,
            GitHubUrl = project.GitHubUrl,
            Datum = project.Datum
        };

            return Results.Created($"/api/projects/{project.Id}", result);
        });

        app.MapGet("/api/projects/{id:int}", async (
            int id,
            ProjectService service) =>
        {
            var project = await service.GetByIdAsync(id);

            if (project == null)
            {
                return Results.NotFound();
            }

            var result = new ProjectResponseDto
            {
                Id = project.Id,
                Titel = project.Titel,
                Beschrijving = project.Beschrijving,
                Categorie = project.Categorie,
                GitHubUrl = project.GitHubUrl,
                Datum = project.Datum
            };

            return Results.Ok(result);
        });

        app.MapPut("/api/projects/{id:int}", async (
            int id,
            ProjectUpdateDto dto,
            ProjectService service) =>
        {
            var updated = await service.UpdateAsync(id, dto);

            if (!updated)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        });

        app.MapDelete("/api/projects/{id:int}", async (
            int id,
            ProjectService service) =>
        {
            var deleted = await service.DeleteAsync(id);

            if (!deleted)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        });
    }
}