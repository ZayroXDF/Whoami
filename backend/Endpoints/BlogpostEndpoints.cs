using Whoami.Api.Models;

namespace Whoami.Api.Endpoints;

public static class BlogpostEndpoints
{
    public static void MapBlogpostEndpoints(this WebApplication app)
    {
        app.MapGet("/api/blogposts", () =>
        {
            var blogposts = new List<Blogpost>
            {
                new Blogpost
                {
                    Id = 1,
                    Titel = "Mijn eerste blogpost",
                    Inhoud = "Dit is mijn eerste blogpost over mijn opleiding Software Development.",
                    Publicatiedatum = new DateTime(2026, 11, 6)
                }
            };

            return Results.Ok(blogposts);
        });
    }
}