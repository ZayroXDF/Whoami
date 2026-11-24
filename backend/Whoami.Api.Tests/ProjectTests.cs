using System.Net;
using Xunit;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Whoami.Api.Data;
using Microsoft.EntityFrameworkCore;
using Whoami.Api.DTOs;

namespace Whoami.Api.Tests;

public class ProjectTests
{
    [Fact]
    public async Task GetProjects_Returns200OK()
    {
        // Arrange: API starten in de testomgeving
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        // Act: HTTP GET-verzoek uitvoeren
        var response = await client.GetAsync("/api/projects");

        // Assert: controleren of de statuscode 200 is
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProject_NonExistingId_Returns404NotFound()
    {
        // Arrange
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/projects/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateProject_WithoutLogin_Returns401Unauthorized()
    {
        // Arrange: API starten zonder in te loggen
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        // Act: proberen een project toe te voegen
        var response = await client.PostAsJsonAsync("/api/projects", new
        {
            Titel = "Testproject",
            Beschrijving = "Automatische beveiligingstest",
            Categorie = "Software",
            GitHubUrl = "https://github.com/test",
            Datum = DateTime.UtcNow
        });

        // Assert: gebruiker moet worden geweigerd
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateProject_AsGast_Returns403Forbidden()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var email = $"gast-{Guid.NewGuid()}@test.nl";
        var password = "Test123!";

        try
        {
            // Registreren als Gast
            var registerResponse = await client.PostAsJsonAsync(
                "/api/auth/register",
                new
                {
                    Email = email,
                    Password = password
                });

            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            // Inloggen
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    Email = email,
                    Password = password
                });

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            // Project proberen toe te voegen
            var response = await client.PostAsJsonAsync(
                "/api/projects",
                new
                {
                    Titel = "Testproject Gast",
                    Beschrijving = "Dit mag niet worden toegevoegd",
                    Categorie = "Software",
                    GitHubUrl = "https://github.com/test",
                    Datum = DateTime.UtcNow
                });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            // Testgebruiker altijd opruimen
            using var scope = factory.Services.CreateScope();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                Assert.True(result.Succeeded);
            }
        }
    }

    [Fact]
    public async Task CreateProject_AsEditor_Returns201Created()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var email = $"editor-{Guid.NewGuid()}@test.nl";
        var password = "Test123!";

        using var scope = factory.Services.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        IdentityUser? user = null;
        int? projectId = null;

        try
        {
            // Testgebruiker registreren
            var registerResponse = await client.PostAsJsonAsync(
                "/api/auth/register",
                new { Email = email, Password = password });

            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            // Gebruiker ophalen en Editor maken
            user = await userManager.FindByEmailAsync(email);
            Assert.NotNull(user);

            var removeResult = await userManager.RemoveFromRoleAsync(user, "Gast");
            Assert.True(removeResult.Succeeded);

            var addResult = await userManager.AddToRoleAsync(user, "Editor");
            Assert.True(addResult.Succeeded);

            // Inloggen met Editor-rechten
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login",
                new { Email = email, Password = password });

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            // Project aanmaken
            var response = await client.PostAsJsonAsync(
                "/api/projects",
                new
                {
                    Titel = "Automatische test",
                    Beschrijving = "Project aangemaakt door xUnit",
                    Categorie = "Software",
                    GitHubUrl = "https://github.com/test",
                    Datum = DateTime.UtcNow
                });

            // Controleren of de API 201 Created teruggeeft
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Het aangemaakte project uitlezen
            var project = await response.Content
                .ReadFromJsonAsync<ProjectResponseDto>();

            Assert.NotNull(project);
            Assert.True(project.Id > 0);
            Assert.Equal("Automatische test", project.Titel);

            projectId = project.Id;
        }
        finally
        {
            // Testproject opruimen
            if (projectId.HasValue)
            {
                var db = scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                var project = await db.Projects
                    .FindAsync(projectId.Value);

                if (project != null)
                {
                    db.Projects.Remove(project);
                    await db.SaveChangesAsync();
                }
            }

            // Testgebruiker opruimen
            if (user != null)
            {
                var deleteResult = await userManager.DeleteAsync(user);
                Assert.True(deleteResult.Succeeded);
            }
        }
    }

    [Fact]
    public async Task UpdateProject_AsEditor_Returns204NoContent()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        IdentityUser? user = null;
        int? projectId = null;

        try
        {
            // Arrange: inloggen als Editor
            user = await LoginAsEditorAsync(client, userManager);

            // Eerst een project aanmaken
            var createResponse = await client.PostAsJsonAsync(
                "/api/projects",
                new
                {
                    Titel = "Oude titel",
                    Beschrijving = "Oude beschrijving",
                    Categorie = "Software",
                    GitHubUrl = "https://github.com/test",
                    Datum = DateTime.UtcNow
                });

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var project = await createResponse.Content
                .ReadFromJsonAsync<ProjectResponseDto>();

            Assert.NotNull(project);
            projectId = project.Id;

            // Act: project wijzigen
            var updateResponse = await client.PutAsJsonAsync(
                $"/api/projects/{projectId}",
                new
                {
                    Titel = "Nieuwe titel",
                    Beschrijving = "Nieuwe beschrijving",
                    Categorie = "Webdevelopment",
                    GitHubUrl = "https://github.com/nieuw",
                    Datum = DateTime.UtcNow
                });

            // Assert: wijziging is gelukt
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            // Controleren of de gewijzigde gegevens zijn opgeslagen
            var getResponse = await client.GetAsync(
                $"/api/projects/{projectId}");

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var updatedProject = await getResponse.Content
                .ReadFromJsonAsync<ProjectResponseDto>();

            Assert.NotNull(updatedProject);
            Assert.Equal("Nieuwe titel", updatedProject.Titel);
            Assert.Equal("Nieuwe beschrijving", updatedProject.Beschrijving);
            Assert.Equal("Webdevelopment", updatedProject.Categorie);
        }
        finally
        {
            // Testproject opruimen
            if (projectId.HasValue)
            {
                var project = await db.Projects.FindAsync(projectId.Value);

                if (project != null)
                {
                    db.Projects.Remove(project);
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
        public async Task DeleteProject_AsEditor_Returns204NoContent()
        {
            using var factory = new CustomWebApplicationFactory();
            using var client = factory.CreateClient();
            using var scope = factory.Services.CreateScope();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            IdentityUser? user = null;
            int? projectId = null;

            try
            {
                // Arrange: inloggen als Editor
                user = await LoginAsEditorAsync(client, userManager);

                // Testproject aanmaken
                var createResponse = await client.PostAsJsonAsync(
                    "/api/projects",
                    new
                    {
                        Titel = "Project om te verwijderen",
                        Beschrijving = "Automatische DELETE-test",
                        Categorie = "Software",
                        GitHubUrl = "https://github.com/test",
                        Datum = DateTime.UtcNow
                    });

                Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

                var project = await createResponse.Content
                    .ReadFromJsonAsync<ProjectResponseDto>();

                Assert.NotNull(project);
                projectId = project.Id;

                // Act: project verwijderen via de API
                var deleteResponse = await client.DeleteAsync(
                    $"/api/projects/{projectId}");

                // Assert: verwijderen is gelukt
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

                // Controleren of het project niet meer bestaat
                var getResponse = await client.GetAsync(
                    $"/api/projects/{projectId}");

                Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
            }
            finally
            {
                // Project opruimen als de test eerder is mislukt
                if (projectId.HasValue)
                {
                    var project = await db.Projects.FindAsync(projectId.Value);

                    if (project != null)
                    {
                        db.Projects.Remove(project);
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

        private static async Task<IdentityUser> LoginAsEditorAsync(
        HttpClient client,
        UserManager<IdentityUser> userManager)
        {
        var email = $"editor-{Guid.NewGuid()}@test.nl";
        var password = "Test123!";

        // Nieuwe gebruiker registreren
        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new { Email = email, Password = password });

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        // Gebruiker ophalen
        var user = await userManager.FindByEmailAsync(email);
        Assert.NotNull(user);

        // Gast-rol vervangen door Editor
        var removeResult = await userManager.RemoveFromRoleAsync(user, "Gast");
        Assert.True(removeResult.Succeeded);

        var addResult = await userManager.AddToRoleAsync(user, "Editor");
        Assert.True(addResult.Succeeded);

        // Inloggen als Editor
        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new { Email = email, Password = password });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        return user;
    }
}