using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Whoami.Api.Data;

namespace Whoami.Api.Tests;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Bestaande databaseconfiguratie verwijderen
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            // Aparte testdatabase gebruiken
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    "Server=.\\SQLEXPRESS;" +
                    "Database=WhoamiTestDb;" +
                    "Trusted_Connection=True;" +
                    "TrustServerCertificate=True;"
                ));
        });
    }
}