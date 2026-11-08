using Whoami.Api.DTOs;
using Whoami.Api.Services;

namespace Whoami.Api.Endpoints;

public static class BlogpostEndpoints
{
    public static void MapBlogpostEndpoints(this WebApplication app)
    {
        app.MapGet("/api/blogposts", async (BlogpostService service) =>
        {
            var blogposts = await service.GetAllAsync();

            var result = blogposts.Select(blogpost => new BlogpostResponseDto
            {
                Id = blogpost.Id,
                Titel = blogpost.Titel,
                Inhoud = blogpost.Inhoud,
                Publicatiedatum = blogpost.Publicatiedatum
            }).ToList();

            return Results.Ok(result);
        });

        app.MapPost("/api/blogposts", async (
            BlogpostCreateDto dto,
            BlogpostService service) =>
        {
            var blogpost = await service.CreateAsync(dto);

            var result = new BlogpostResponseDto
            {
                Id = blogpost.Id,
                Titel = blogpost.Titel,
                Inhoud = blogpost.Inhoud,
                Publicatiedatum = blogpost.Publicatiedatum
            };

            return Results.Created($"/api/blogposts/{blogpost.Id}", result);
        });

        app.MapGet("/api/blogposts/{id:int}", async (
            int id,
            BlogpostService service) =>
        {
            var blogpost = await service.GetByIdAsync(id);

            if (blogpost == null)
            {
                return Results.NotFound();
            }

            var result = new BlogpostResponseDto
            {
                Id = blogpost.Id,
                Titel = blogpost.Titel,
                Inhoud = blogpost.Inhoud,
                Publicatiedatum = blogpost.Publicatiedatum
            };

            return Results.Ok(result);
        });

        app.MapPut("/api/blogposts/{id:int}", async (
            int id,
            BlogpostUpdateDto dto,
            BlogpostService service) =>
        {
            var updated = await service.UpdateAsync(id, dto);

            if (!updated)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        });

        app.MapDelete("/api/blogposts/{id:int}", async (
            int id,
            BlogpostService service) =>
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