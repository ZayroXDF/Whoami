/*----------------------------------------------------------------------------------------------------
/* PROJECTEN
/*----------------------------------------------------------------------------------------------------
   Bevat alle projectgegevens in een array met objecten.
   JavaScript gebruikt deze gegevens om projectkaarten te maken. */

const projecten = [
    {
        nummer: "01",
        titel: "FoodLink",
        beschrijving: "Een JavaFX-applicatie gekoppeld aan een slimme locker met Arduino, RFID en een MySQL-database.",
        tags: ["Java", "JavaFX", "MySQL", "Arduino"]
    },
    {
        nummer: "02",
        titel: "Hotel Simulator",
        beschrijving: "Een hotelsimulatie gebouwd in Java Swing volgens het MVC-principe, inclusief gasten, personeel, routes en events.",
        tags: ["Java", "Swing", "MVC", "JUnit"]
    },
    {
        nummer: "03",
        titel: "Smart Environment",
        beschrijving: "Een project waarin sensordata en externe databronnen overzichtelijk worden weergegeven in een dashboard.",
        tags: ["Web", "API", "Sensor Data"]
    },
    {
        nummer: "04",
        titel: "Portfolio Website",
        beschrijving: "Deze persoonlijke portfoliosite, ontwikkeld met semantische HTML5 en responsive CSS3.",
        tags: ["HTML5", "CSS3", "Responsive"]
    }
];


/* PROJECT-GRID SELECTEREN
   getElementById zoekt het HTML-element met id project-grid.
   Hierin worden de projectkaarten geplaatst. */
const projectGrid = document.getElementById("project-grid");


/* FUNCTIE TOONPROJECTEN
   Ontvangt een lijst met projecten en toont deze op de pagina.
   De functie maakt voor ieder project een nieuwe HTML-kaart. */
function toonProjecten(lijst) {

    /* BESTAANDE KAARTEN VERWIJDEREN
       innerHTML maakt de project-grid leeg.
       Hierdoor worden oude kaarten niet dubbel weergegeven. */
    projectGrid.innerHTML = "";

    /* PROJECTEN DOORLOPEN
       forEach voert de functie uit voor ieder project in de lijst.
       Elk project wordt afzonderlijk omgezet naar HTML. */
    lijst.forEach(function(project) {

        /* PROJECTKAART AANMAKEN
           createElement maakt een nieuw article-element.
           classList.add geeft dit element de CSS-class project-card. */
        const article = document.createElement("article");
        article.classList.add("project-card");

        /* PROJECTNUMMER
           Maakt een div voor het nummer van het project.
           textContent vult deze met het nummer uit de array. */
        const nummer = document.createElement("div");
        nummer.classList.add("project-number");
        nummer.textContent = project.nummer;

        /* PROJECTTITEL
           Maakt een h2-element voor de titel.
           textContent haalt de titel uit het projectobject. */
        const titel = document.createElement("h2");
        titel.textContent = project.titel;

        /* PROJECTBESCHRIJVING
           Maakt een p-element voor de beschrijving.
           De tekst komt uit het projectobject. */
        const beschrijving = document.createElement("p");
        beschrijving.textContent = project.beschrijving;

        /* TAGS-CONTAINER
           Maakt een div waarin alle projecttags komen.
           De class tags zorgt voor de juiste CSS-opmaak. */
        const tags = document.createElement("div");
        tags.classList.add("tags");

        /* TAGS TOEVOEGEN
           forEach doorloopt alle tags van het project.
           appendChild voegt iedere nieuwe span toe aan de tags-div. */
        project.tags.forEach(function(tag) {
            const span = document.createElement("span");
            span.textContent = tag;
            tags.appendChild(span);
        });

        /* PROJECTKAART OPBOUWEN
           appendChild voegt de gemaakte elementen toe aan article.
           Hierdoor krijgt de kaart een nummer, titel, tekst en tags. */
        article.appendChild(nummer);
        article.appendChild(titel);
        article.appendChild(beschrijving);
        article.appendChild(tags);

        /* PROJECTKAART WEERGEVEN
           Voegt de volledige article toe aan de project-grid.
           Hierdoor verschijnt de projectkaart op de website. */
        projectGrid.appendChild(article);
    });
}


/* CONTROLEREN OF PROJECT-GRID BESTAAT
   Voert de projectcode alleen uit op de Projects-pagina.
   Zo voorkomt de if-controle fouten op andere pagina's. */
if (projectGrid) {

    /* ALLE PROJECTEN TONEN
       Roept toonProjecten aan met de volledige array.
       Hierdoor verschijnen alle projecten bij het laden. */
    toonProjecten(projecten);

    /* FILTER ALLE PROJECTEN
       addEventListener reageert wanneer de gebruiker klikt.
       De volledige projectenlijst wordt opnieuw weergegeven. */
    document.getElementById("toon-alles").addEventListener("click", function() {
        toonProjecten(projecten);
    });

    /* FILTER JAVA
       filter maakt een nieuwe lijst met alleen Java-projecten.
       includes controleert of de tags het woord Java bevatten. */
    document.getElementById("toon-java").addEventListener("click", function() {
        const javaProjecten = projecten.filter(function(project) {
            return project.tags.includes("Java");
        });

        toonProjecten(javaProjecten);
    });

    /* FILTER WEB
       filter selecteert projecten met de tag Web.
       Daarna toont toonProjecten alleen deze projecten. */
    document.getElementById("toon-web").addEventListener("click", function() {
        const webProjecten = projecten.filter(function(project) {
            return project.tags.includes("Web");
        });

        toonProjecten(webProjecten);
    });
}