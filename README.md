# Whoami – Portfolio Backend

Dit project is de backend van mijn Whoami-portfolio, ontwikkeld voor WPFW Opdracht 3 van de opleiding HBO-ICT Software Development aan De Haagse Hogeschool.

De backend is gebouwd met ASP.NET Core Minimal API en maakt het mogelijk om projecten en blogposts aan te maken, op te halen, te wijzigen en te verwijderen (CRUD).

## 1. Gebruikte technieken

- C# en .NET 10
- ASP.NET Core Minimal API
- Entity Framework Core
- SQL Server Express
- SQL Server Management Studio (SSMS)
- Swagger UI (Swashbuckle)
- Visual Studio Code
- Git en GitHub

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

Met `dotnet ef database update` worden de bestaande migraties toegepast en worden de tabellen `Projects` en `Blogposts` aangemaakt.

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

## 7. Testen

De endpoints zijn getest met Swagger UI.

Voor zowel projecten als blogposts zijn CRUD-bewerkingen uitgevoerd. Ook is gecontroleerd of de API een 404-statuscode teruggeeft bij een niet-bestaand ID.

Daarnaast is de persistentie getest door een project toe te voegen, de backend opnieuw te starten en te controleren of het project nog aanwezig was.

Met SQL Server Management Studio zijn de opgeslagen records in de database gecontroleerd.

## 8. Repository

GitHub: https://github.com/ZayroXDF/Whoami