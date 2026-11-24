using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Whoami.Api.Tests;

public class AuthTests
{
    [Fact]
        public async Task Register_NewUser_Returns200OK()
        {
            using var factory = new CustomWebApplicationFactory();
            using var client = factory.CreateClient();
            using var scope = factory.Services.CreateScope();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            var email = $"register-{Guid.NewGuid()}@test.nl";
            var password = "Test123!";

            try
            {
                // Arrange & Act: nieuwe gebruiker registreren
                var response = await client.PostAsJsonAsync(
                    "/api/auth/register",
                    new
                    {
                        Email = email,
                        Password = password
                    });

                // Assert: registratie moet slagen
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                // Controleren of gebruiker in database staat
                var user = await userManager.FindByEmailAsync(email);

                Assert.NotNull(user);
                Assert.Equal(email, user.Email);

                // Controleren of gebruiker de Gast-rol heeft
                var isGast = await userManager.IsInRoleAsync(user, "Gast");

                Assert.True(isGast);
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
        public async Task Login_WithCorrectPassword_Returns200OK()
        {
            using var factory = new CustomWebApplicationFactory();
            using var client = factory.CreateClient();
            using var scope = factory.Services.CreateScope();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            var email = $"login-{Guid.NewGuid()}@test.nl";
            var password = "Test123!";

            try
            {
                // Arrange: gebruiker registreren
                var registerResponse = await client.PostAsJsonAsync(
                    "/api/auth/register",
                    new
                    {
                        Email = email,
                        Password = password
                    });

                Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

                // Act: inloggen met juiste gegevens
                var loginResponse = await client.PostAsJsonAsync(
                    "/api/auth/login",
                    new
                    {
                        Email = email,
                        Password = password
                    });

                // Assert: login moet slagen
                Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
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
        public async Task Login_WithWrongPassword_Returns401Unauthorized()
        {
            using var factory = new CustomWebApplicationFactory();
            using var client = factory.CreateClient();
            using var scope = factory.Services.CreateScope();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            var email = $"wrongpassword-{Guid.NewGuid()}@test.nl";
            var password = "Test123!";

            try
            {
                // Arrange: gebruiker registreren
                var registerResponse = await client.PostAsJsonAsync(
                    "/api/auth/register",
                    new
                    {
                        Email = email,
                        Password = password
                    });

                Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

                // Act: inloggen met verkeerd wachtwoord
                var loginResponse = await client.PostAsJsonAsync(
                    "/api/auth/login",
                    new
                    {
                        Email = email,
                        Password = "Verkeerd123!"
                    });

                // Assert: toegang moet worden geweigerd
                Assert.Equal(
                    HttpStatusCode.Unauthorized,
                    loginResponse.StatusCode
                );
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
        public async Task Register_DuplicateEmail_Returns400BadRequest()
        {
            using var factory = new CustomWebApplicationFactory();
            using var client = factory.CreateClient();
            using var scope = factory.Services.CreateScope();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            var email = $"duplicate-{Guid.NewGuid()}@test.nl";
            var password = "Test123!";

            try
            {
                // Arrange: eerste registratie
                var firstResponse = await client.PostAsJsonAsync(
                    "/api/auth/register",
                    new
                    {
                        Email = email,
                        Password = password
                    });

                Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

                // Act: opnieuw registreren met hetzelfde e-mailadres
                var secondResponse = await client.PostAsJsonAsync(
                    "/api/auth/register",
                    new
                    {
                        Email = email,
                        Password = password
                    });

                // Assert: dubbele registratie moet worden geweigerd
                Assert.Equal(
                    HttpStatusCode.BadRequest,
                    secondResponse.StatusCode
                );
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
}