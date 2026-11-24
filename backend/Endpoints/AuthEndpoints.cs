
using Microsoft.AspNetCore.Identity;
using Whoami.Api.DTOs;

namespace Whoami.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth");

        // REGISTRATIE
        group.MapPost("/register", async (
            RegisterDto dto,
            UserManager<IdentityUser> userManager) =>
        {
            var user = new IdentityUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await userManager.CreateAsync(
                user, dto.Password
            );

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            var roleResult = await userManager.AddToRoleAsync(
                user, "Gast"
            );

            if (!roleResult.Succeeded)
            {
                return Results.Problem(
                    "De Gast-rol kon niet worden toegekend."
                );
            }

            return Results.Ok(new
            {
                Message = "Gebruiker succesvol geregistreerd."
            });
        });

        // LOGIN - NIEUW TOEGEVOEGD
        group.MapPost("/login", async (
            LoginDto dto,
            SignInManager<IdentityUser> signInManager) =>
        {
            var result = await signInManager.PasswordSignInAsync(
                dto.Email,
                dto.Password,
                isPersistent: false,
                lockoutOnFailure: true
            );

            if (!result.Succeeded)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(new
            {
                Message = "Succesvol ingelogd."
            });
        });
    }
}
