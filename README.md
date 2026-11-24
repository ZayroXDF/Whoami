# Whoami – Portfolio Backend

Dit project is de backend van mijn Whoami-portfolio, ontwikkeld voor WPFW Opdracht 3 en 4 van de opleiding HBO-ICT Software Development aan De Haagse Hogeschool.

De backend is gebouwd met ASP.NET Core Minimal API en ondersteunt CRUD-bewerkingen voor projecten en blogposts. Daarnaast heb ik authenticatie en autorisatie toegevoegd met ASP.NET Core Identity. Met rollen wordt bepaald welke gebruikers gegevens mogen toevoegen, wijzigen of verwijderen.

## 1. Gebruikte technieken

- C# en .NET 10
- ASP.NET Core Minimal API
- Entity Framework Core
- SQL Server Express
- SQL Server Management Studio (SSMS)
- Swagger UI (Swashbuckle)
- Visual Studio Code
- Git en GitHub
- ASP.NET Core Identity
- xUnit voor integratietests
- Microsoft.AspNetCore.Mvc.Testing (WebApplicationFactory)

## 2. Projectstructuur

De backend is opgedeeld in verschillende onderdelen:

| Map of bestand | Functie |
|---|---|
| Models | Bevat de entiteiten Project en Blogpost |
| DTOs | Bevat de Create-, Update- en Response-DTO's |
| Data | Bevat AppDbContext voor de databaseverbinding |
| Services | Bevat de CRUD-logica |
| Endpoints | Bevat de HTTP-routes voor projecten en blogposts |
| Migrations | Bevat de EF Core-databasemigraties |
| Program.cs | Configureert de database, services en API |
| AuthEndpoints.cs | Bevat de endpoints voor registreren en inloggen |
| Whoami.Api.Tests | Bevat de automatische integratietests |

De applicatie gebruikt een gescheiden lagenstructuur, waarbij endpoints HTTP-verzoeken verwerken, services de bewerkingen uitvoeren en Entity Framework Core de database benadert.

## 3. Database configureren

De backend gebruikt een lokale SQL Server Express-database met de naam `WhoamiDb`.

De connectionstring staat in `backend/appsettings.json`.

Voorbeeld:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=WhoamiDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

De SQL Server-instance moet beschikbaar zijn. Wanneer je een andere instance gebruikt, moet je de servernaam in de connectionstring aanpassen.

## 4. Database aanmaken

De database wordt aangemaakt met Entity Framework Core Migrations.

Open een terminal in de map `backend` en voer uit:

```powershell
dotnet restore
dotnet ef database update
```

Met `dotnet ef database update` worden de bestaande migraties toegepast. Hiermee worden de tabellen voor projecten, blogposts en ASP.NET Core Identity aangemaakt.

Identity gebruikt onder andere de tabellen `AspNetUsers`, `AspNetRoles` en `AspNetUserRoles`. Hierin worden gebruikers, rollen en de bijbehorende koppelingen opgeslagen.

### 4.1 Testdatabase

Voor de automatische integratietests gebruik ik een aparte database met de naam `WhoamiTestDb`. Hierdoor worden de tests niet uitgevoerd op de normale database `WhoamiDb`.

De testdatabase maakt gebruik van dezelfde Entity Framework Core-migraties als de normale database.

Om de testdatabase aan te maken, voer ik vanuit de map `backend` het volgende commando uit:

```powershell
dotnet ef database update --connection "Server=.\SQLEXPRESS;Database=WhoamiTestDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

In `CustomWebApplicationFactory.cs` wordt de databaseverbinding tijdens de integratietests vervangen door een verbinding met `WhoamiTestDb`.

Bij tests die gegevens aanmaken, worden de aangemaakte testgebruikers en records achteraf weer verwijderd. Zo blijft de testdatabase zo schoon mogelijk.

## 5. Backend starten

Open een terminal in de hoofdmap van het project en voer uit:

```powershell
cd backend
dotnet run
```

De API draait lokaal op:

`http://localhost:5111`

Swagger UI is beschikbaar via:

`http://localhost:5111/swagger`

Het poortnummer kan verschillen afhankelijk van de lokale configuratie.

## 6. CRUD-endpoints

Voor Project en Blogpost zijn elk vijf endpoints beschikbaar.

| Methode | Project | Blogpost |
|---|---|---|
| GET alle | `/api/projects` | `/api/blogposts` |
| GET op ID | `/api/projects/{id}` | `/api/blogposts/{id}` |
| POST | `/api/projects` | `/api/blogposts` |
| PUT | `/api/projects/{id}` | `/api/blogposts/{id}` |
| DELETE | `/api/projects/{id}` | `/api/blogposts/{id}` |

De API gebruikt de volgende HTTP-statuscodes:

- 200 OK – gegevens succesvol opgehaald
- 201 Created – nieuw item aangemaakt
- 204 No Content – item gewijzigd of verwijderd
- 404 Not Found – item niet gevonden

## 6.1 Authenticatie en autorisatie

Voor WPFW Opdracht 4 heb ik authenticatie en autorisatie toegevoegd met ASP.NET Core Identity.

Met authenticatie controleert de applicatie wie een gebruiker is. Met autorisatie wordt bepaald welke acties deze gebruiker mag uitvoeren.

### Registreren en inloggen

De API heeft twee endpoints voor authenticatie:

| Methode | Endpoint | Functie |
|---|---|---|
| POST | `/api/auth/register` | Nieuwe gebruiker registreren |
| POST | `/api/auth/login` | Inloggen met e-mailadres en wachtwoord |

Bij registratie wordt een nieuwe gebruiker aangemaakt met de rol `Gast`. Identity zorgt voor het veilig opslaan van wachtwoordhashes. Bij een succesvolle login gebruikt de applicatie cookie-authenticatie.

### Rollen en rechten

De applicatie gebruikt drie rollen: `Gast`, `Editor` en `Admin`.

| Actie | Gast | Editor | Admin |
|---|---|---|---|
| Projecten bekijken (GET) | Ja | Ja | Ja |
| Blogposts bekijken (GET) | Ja | Ja | Ja |
| Projecten toevoegen (POST) | Nee | Ja | Ja |
| Projecten wijzigen (PUT) | Nee | Ja | Ja |
| Projecten verwijderen (DELETE) | Nee | Ja | Ja |
| Blogposts toevoegen (POST) | Nee | Ja | Ja |
| Blogposts wijzigen (PUT) | Nee | Ja | Ja |
| Blogposts verwijderen (DELETE) | Nee | Ja | Ja |

De GET-endpoints zijn openbaar en kunnen ook zonder inloggen worden gebruikt.

Voor POST-, PUT- en DELETE-verzoeken wordt de autorisatiepolicy `EditorOrAdmin` gebruikt. Deze is ingesteld in `Program.cs` en wordt toegepast met `.RequireAuthorization("EditorOrAdmin")`.

### HTTP-statuscodes bij beveiliging

- `200 OK`: registratie of login is geslaagd.
- `400 Bad Request`: bijvoorbeeld bij een dubbele registratie.
- `401 Unauthorized`: een gebruiker is niet ingelogd of de login is mislukt.
- `403 Forbidden`: een ingelogde gebruiker heeft onvoldoende rechten.

Hiermee wordt voorkomen dat onbevoegde gebruikers projecten of blogposts kunnen toevoegen, wijzigen of verwijderen.

## 7. Testen

Voor WPFW Opdracht 4 heb ik automatische integratietests toegevoegd met xUnit en `WebApplicationFactory`.

Hiermee test ik de werking van de API door echte HTTP-verzoeken naar de applicatie te sturen. De tests controleren zowel de CRUD-functionaliteit als authenticatie en autorisatie.

De tests staan in de map `backend/Whoami.Api.Tests` en zijn verdeeld over drie bestanden:

| Bestand | Aantal tests | Onderdeel |
|---|---:|---|
| `ProjectTests.cs` | 7 | CRUD en autorisatie voor projecten |
| `BlogpostTests.cs` | 7 | CRUD en autorisatie voor blogposts |
| `AuthTests.cs` | 4 | Registreren en inloggen |
| **Totaal** | **18** | |

### 7.1 Testmatrix

| Test | Verwachte statuscode |
|---|---|
| Alle projecten ophalen | 200 OK |
| Niet-bestaand project ophalen | 404 Not Found |
| Project aanmaken zonder login | 401 Unauthorized |
| Project aanmaken als Gast | 403 Forbidden |
| Project aanmaken als Editor | 201 Created |
| Project wijzigen als Editor | 204 No Content |
| Project verwijderen als Editor | 204 No Content |
| Alle blogposts ophalen | 200 OK |
| Niet-bestaande blogpost ophalen | 404 Not Found |
| Blogpost aanmaken zonder login | 401 Unauthorized |
| Blogpost aanmaken als Gast | 403 Forbidden |
| Blogpost aanmaken als Editor | 201 Created |
| Blogpost wijzigen als Editor | 204 No Content |
| Blogpost verwijderen als Editor | 204 No Content |
| Nieuwe gebruiker registreren | 200 OK |
| Inloggen met correct wachtwoord | 200 OK |
| Inloggen met verkeerd wachtwoord | 401 Unauthorized |
| Registreren met bestaand e-mailadres | 400 Bad Request |

### 7.2 Tests uitvoeren

Open een terminal in de map `backend` en voer uit:

```powershell
dotnet test Whoami.slnx --verbosity normal
```

De tests gebruiken een aparte SQL Server Express-database met de naam `WhoamiTestDb`. Deze moet vooraf zijn aangemaakt en voorzien zijn van de EF Core-migraties.

### 7.3 Testresultaten

Na het uitvoeren van de tests kreeg ik het volgende resultaat:

```text
Test summary: total: 18, failed: 0, succeeded: 18, skipped: 0
Build succeeded
```

Alle 18 integratietests zijn geslaagd.

De tests controleren niet alleen de HTTP-statuscodes, maar bij verschillende CRUD-tests ook of gegevens daadwerkelijk zijn aangemaakt, gewijzigd of verwijderd.

Testgebruikers en aangemaakte records worden waar mogelijk na afloop verwijderd, zodat de testdatabase schoon blijft.

### 7.4 Handmatig testen

Naast de automatische tests heb ik de CRUD-endpoints handmatig getest met Swagger UI.

Ook heb ik met SQL Server Management Studio gecontroleerd of de gegevens correct in de database worden opgeslagen.

## 8. Repository

GitHub: https://github.com/ZayroXDF/Whoami