using System.Net;
using Xunit;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Whoami.Api.Data;
using Whoami.Api.DTOs;

namespace Whoami.Api.Tests;

public class BlogpostTests
{
    [Fact]
    public async Task GetBlogposts_Returns200OK()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/blogposts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetBlogpost_NonExistingId_Returns404NotFound()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        // Act: blogpost opvragen die niet bestaat
        var response = await client.GetAsync("/api/blogposts/999999");

        // Assert: blogpost niet gevonden
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateBlogpost_WithoutLogin_Returns401Unauthorized()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        // Act: blogpost aanmaken zonder in te loggen
        var response = await client.PostAsJsonAsync(
            "/api/blogposts",
            new
            {
                Titel = "Testblogpost",
                Inhoud = "Deze blogpost mag niet worden aangemaakt.",
                Publicatiedatum = DateTime.UtcNow
            });

        // Assert: toegang geweigerd
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }

    [Fact]
    public async Task CreateBlogpost_AsGast_Returns403Forbidden()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        var email = $"gast-blog-{Guid.NewGuid()}@test.nl";
        var password = "Test123!";

        try
        {
            // Arrange: gebruiker registreren als Gast
            var registerResponse = await client.PostAsJsonAsync(
                "/api/auth/register",
                new { Email = email, Password = password });

            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            // Inloggen als Gast
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login",
                new { Email = email, Password = password });

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            // Act: proberen een blogpost aan te maken
            var response = await client.PostAsJsonAsync(
                "/api/blogposts",
                new
                {
                    Titel = "Blogpost van Gast",
                    Inhoud = "Deze blogpost mag niet worden aangemaakt.",
                    Publicatiedatum = DateTime.UtcNow
                });

            // Assert: Gast heeft onvoldoende rechten
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            // Testgebruiker opruimen
            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                Assert.True(result.Succeeded);
            }
        }
    }

    [Fact]
    public async Task CreateBlogpost_AsEditor_Returns201Created()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var email = $"editor-blog-{Guid.NewGuid()}@test.nl";
        var password = "Test123!";

        IdentityUser? user = null;
        int? blogpostId = null;

        try
        {
            // Arrange: gebruiker registreren
            var registerResponse = await client.PostAsJsonAsync(
                "/api/auth/register",
                new { Email = email, Password = password });

            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            // Rol wijzigen van Gast naar Editor
            user = await userManager.FindByEmailAsync(email);
            Assert.NotNull(user);

            var removeResult = await userManager.RemoveFromRoleAsync(user, "Gast");
            Assert.True(removeResult.Succeeded);

            var addResult = await userManager.AddToRoleAsync(user, "Editor");
            Assert.True(addResult.Succeeded);

            // Inloggen als Editor
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login",
                new { Email = email, Password = password });

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            // Act: blogpost aanmaken
            var response = await client.PostAsJsonAsync(
                "/api/blogposts",
                new
                {
                    Titel = "Automatische blogposttest",
                    Inhoud = "Blogpost aangemaakt door xUnit.",
                    Publicatiedatum = DateTime.UtcNow
                });

            // Assert: blogpost is aangemaakt
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var blogpost = await response.Content
                .ReadFromJsonAsync<BlogpostResponseDto>();

            Assert.NotNull(blogpost);
            Assert.True(blogpost.Id > 0);
            Assert.Equal("Automatische blogposttest", blogpost.Titel);

            blogpostId = blogpost.Id;
        }
        finally
        {
            // Testblogpost opruimen
            if (blogpostId.HasValue)
            {
                var blogpost = await db.Blogposts.FindAsync(blogpostId.Value);

                if (blogpost != null)
                {
                    db.Blogposts.Remove(blogpost);
                    await db.SaveChangesAsync();
                }
            }

            // Testgebruiker opruimen
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                Assert.True(result.Succeeded);
            }
        }
    }

    [Fact]
    public async Task UpdateBlogpost_AsEditor_Returns204NoContent()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var email = $"editor-update-{Guid.NewGuid()}@test.nl";
        var password = "Test123!";

        IdentityUser? user = null;
        int? blogpostId = null;

        try
        {
            // Arrange: gebruiker registreren
            var registerResponse = await client.PostAsJsonAsync(
                "/api/auth/register",
                new { Email = email, Password = password });

            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            user = await userManager.FindByEmailAsync(email);
            Assert.NotNull(user);

            // Gebruiker Editor maken
            var removeResult = await userManager.RemoveFromRoleAsync(user, "Gast");
            Assert.True(removeResult.Succeeded);

            var addResult = await userManager.AddToRoleAsync(user, "Editor");
            Assert.True(addResult.Succeeded);

            // Inloggen
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login",
                new { Email = email, Password = password });

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            // Blogpost aanmaken
            var createResponse = await client.PostAsJsonAsync(
                "/api/blogposts",
                new
                {
                    Titel = "Oude titel",
                    Inhoud = "Oude inhoud",
                    Publicatiedatum = DateTime.UtcNow
                });

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var blogpost = await createResponse.Content
                .ReadFromJsonAsync<BlogpostResponseDto>();

            Assert.NotNull(blogpost);
            blogpostId = blogpost.Id;

            // Act: blogpost wijzigen
            var updateResponse = await client.PutAsJsonAsync(
                $"/api/blogposts/{blogpostId}",
                new
                {
                    Titel = "Nieuwe titel",
                    Inhoud = "Nieuwe inhoud",
                    Publicatiedatum = DateTime.UtcNow
                });

            // Assert: wijzigen is gelukt
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            // Controleren of wijzigingen zijn opgeslagen
            var getResponse = await client.GetAsync(
                $"/api/blogposts/{blogpostId}");

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var updatedBlogpost = await getResponse.Content
                .ReadFromJsonAsync<BlogpostResponseDto>();

            Assert.NotNull(updatedBlogpost);
            Assert.Equal("Nieuwe titel", updatedBlogpost.Titel);
            Assert.Equal("Nieuwe inhoud", updatedBlogpost.Inhoud);
        }
        finally
        {
            // Testblogpost verwijderen
            if (blogpostId.HasValue)
            {
                var blogpost = await db.Blogposts.FindAsync(blogpostId.Value);

                if (blogpost != null)
                {
                    db.Blogposts.Remove(blogpost);
                    await db.SaveChangesAsync();
                }
            }

            // Testgebruiker verwijderen
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                Assert.True(result.Succeeded);
            }
        }
    }

    [Fact]
    public async Task DeleteBlogpost_AsEditor_Returns204NoContent()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var email = $"editor-delete-{Guid.NewGuid()}@test.nl";
        var password = "Test123!";

        IdentityUser? user = null;
        int? blogpostId = null;

        try
        {
            // Arrange: gebruiker registreren
            var registerResponse = await client.PostAsJsonAsync(
                "/api/auth/register",
                new { Email = email, Password = password });

            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            user = await userManager.FindByEmailAsync(email);
            Assert.NotNull(user);

            // Rol wijzigen naar Editor
            var removeResult = await userManager.RemoveFromRoleAsync(user, "Gast");
            Assert.True(removeResult.Succeeded);

            var addResult = await userManager.AddToRoleAsync(user, "Editor");
            Assert.True(addResult.Succeeded);

            // Inloggen als Editor
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login",
                new { Email = email, Password = password });

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            // Blogpost aanmaken
            var createResponse = await client.PostAsJsonAsync(
                "/api/blogposts",
                new
                {
                    Titel = "Blogpost om te verwijderen",
                    Inhoud = "Automatische DELETE-test",
                    Publicatiedatum = DateTime.UtcNow
                });

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var blogpost = await createResponse.Content
                .ReadFromJsonAsync<BlogpostResponseDto>();

            Assert.NotNull(blogpost);
            blogpostId = blogpost.Id;

            // Act: blogpost verwijderen
            var deleteResponse = await client.DeleteAsync(
                $"/api/blogposts/{blogpostId}");

            // Assert: verwijderen is gelukt
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Controleren of blogpost echt verwijderd is
            var getResponse = await client.GetAsync(
                $"/api/blogposts/{blogpostId}");

            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
        finally
        {
            // Eventuele overgebleven testblogpost opruimen
            if (blogpostId.HasValue)
            {
                var blogpost = await db.Blogposts.FindAsync(blogpostId.Value);

                if (blogpost != null)
                {
                    db.Blogposts.Remove(blogpost);
                    await db.SaveChangesAsync();
                }
            }

            // Testgebruiker opruimen
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                Assert.True(result.Succeeded);
            }
        }
    }

}