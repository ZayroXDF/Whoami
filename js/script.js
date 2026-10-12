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

/*----------------------------------------------------------------------------------------------------
/* BLOG
/*----------------------------------------------------------------------------------------------------
   Regelt de Lees meer- en Lees minder-knoppen.
   Hiermee kan de gebruiker aanvullende blogtekst openen. */

/* BLOGKNOPPEN SELECTEREN
   querySelectorAll zoekt alle elementen met class lees-meer.
   Hierdoor kunnen meerdere blogknoppen worden gebruikt. */
const leesMeerKnoppen = document.querySelectorAll(".lees-meer");

/* BLOGKNOPPEN DOORLOPEN
   forEach voert dezelfde functie uit voor iedere knop.
   Elke knop krijgt een eigen click-eventlistener. */
leesMeerKnoppen.forEach(function(knop) {

    /* KLIK OP DE KNOP
       addEventListener wacht tot de gebruiker op de knop klikt.
       Daarna wordt de bijbehorende blogtekst aangepast. */
    knop.addEventListener("click", function() {

        /* EXTRA TEKST SELECTEREN
           nextElementSibling selecteert het element direct na de knop.
           In mijn HTML is dit de div met class blog-extra. */
        const extraTekst = knop.nextElementSibling;

        /* EXTRA TEKST TONEN OF VERBERGEN
           classList.toggle voegt de class open toe of verwijdert deze.
           CSS bepaalt daarmee of de extra tekst zichtbaar is. */
        extraTekst.classList.toggle("open");

        /* KNOPTEKST AANPASSEN
           contains controleert of de class open aanwezig is.
           De knop toont daarna Lees minder of Lees meer. */
        if (extraTekst.classList.contains("open")) {
            knop.textContent = "Lees minder";
        } else {
            knop.textContent = "Lees meer";
        }

    });

});

/*----------------------------------------------------------------------------------------------------
/* CONTACTFORMULIER
/*----------------------------------------------------------------------------------------------------
   Controleert de naam, het e-mailadres en het bericht.
   Toont foutmeldingen wanneer de invoer niet geldig is. */

/* FORMULIER SELECTEREN
   getElementById zoekt het contactformulier in de HTML.
   De if-controle voorkomt fouten op pagina's zonder formulier. */
const contactForm = document.getElementById("contact-form");

if (contactForm) {

    /* FORMULIER VERWERKEN
       addEventListener reageert wanneer het formulier wordt verzonden.
       De functie controleert eerst alle ingevulde gegevens. */
    contactForm.addEventListener("submit", function(event) {

        /* STANDAARD VERZENDING STOPPEN
           preventDefault voorkomt dat de browser het formulier verstuurt.
           Hierdoor kan JavaScript de invoer zelf controleren. */
        event.preventDefault();

        /* INVOERVELDEN SELECTEREN
           getElementById zoekt de drie invoervelden in de HTML.
           JavaScript kan hiermee de ingevulde waarden uitlezen. */
        const naam = document.getElementById("naam");
        const email = document.getElementById("email");
        const bericht = document.getElementById("bericht");

        /* FOUTMELDINGEN SELECTEREN
           Deze elementen tonen foutmeldingen bij de invoervelden.
           Het resultaat-element toont de algemene formulierfeedback. */
        const naamFout = document.getElementById("naam-fout");
        const emailFout = document.getElementById("email-fout");
        const berichtFout = document.getElementById("bericht-fout");
        const resultaat = document.getElementById("form-resultaat");

        /* OUDE FOUTMELDINGEN VERWIJDEREN
           textContent maakt alle eerdere meldingen leeg.
           Hierdoor blijven oude fouten niet zichtbaar. */
        naamFout.textContent = "";
        emailFout.textContent = "";
        berichtFout.textContent = "";
        resultaat.textContent = "";

        /* FORMULIERSTATUS
           let maakt een variabele waarvan de waarde kan veranderen.
           Bij een fout wordt formulierGeldig op false gezet. */
        let formulierGeldig = true;

        /* NAAM CONTROLEREN
           trim verwijdert spaties aan het begin en einde.
           Bij een leeg veld verschijnt een foutmelding. */
        if (naam.value.trim() === "") {
            naamFout.textContent = "Vul je naam in.";
            formulierGeldig = false;
        }

        /* E-MAILADRES CONTROLEREN
           Controleert of het veld leeg is of ongeldig is ingevuld.
           validity.valid gebruikt de ingebouwde HTML-validatie. */
        if (email.value.trim() === "") {
            emailFout.textContent = "Vul je e-mailadres in.";
            formulierGeldig = false;
        } else if (!email.validity.valid) {
            emailFout.textContent = "Vul een geldig e-mailadres in.";
            formulierGeldig = false;
        }

        /* BERICHT CONTROLEREN
           Controleert of het bericht is ingevuld.
           length controleert of het minimaal 10 tekens bevat. */
        if (bericht.value.trim() === "") {
            berichtFout.textContent = "Vul een bericht in.";
            formulierGeldig = false;
        } else if (bericht.value.trim().length < 10) {
            berichtFout.textContent =
                "Het bericht moet minimaal 10 tekens bevatten.";
            formulierGeldig = false;
        }

        /* FORMULIER CORRECT INGEVULD
           Bij geldige invoer verschijnt een bevestigingsmelding.
           reset maakt daarna alle invoervelden leeg. */
        if (formulierGeldig) {
            resultaat.textContent =
                "Bedankt! Het formulier is correct ingevuld.";

            contactForm.reset();
        }

    });

}

/*----------------------------------------------------------------------------------------------------
/* WEER API
/*----------------------------------------------------------------------------------------------------
   Haalt actuele weergegevens op via de Open-Meteo API.
   Toont de temperatuur en windsnelheid van Den Haag. */

/* WEERELEMENTEN SELECTEREN
   getElementById zoekt de weer-container en de statustekst.
   De if-controle voorkomt fouten op pagina's zonder weergegevens. */
const weerElement = document.getElementById("weer");
const weerStatus = document.getElementById("weer-status");

if (weerElement && weerStatus) {

    /* FUNCTIE LAADWEER
       Deze functie regelt het ophalen van de weergegevens.
       Eerst verschijnt een laadmelding voor de gebruiker. */
    function laadWeer() {

        weerStatus.textContent = "Weer laden...";

        /* API-ADRES
           Bevat de coördinaten van Den Haag en gewenste weergegevens.
           De API levert temperatuur en windsnelheid terug. */
        const url =
            "https://api.open-meteo.com/v1/forecast?latitude=52.07&longitude=4.30&current=temperature_2m,wind_speed_10m";

        /* API-REQUEST
           fetch verstuurt een HTTP GET-request naar Open-Meteo.
           De server stuurt een response met weergegevens terug. */
        fetch(url)
            .then(function(response) {

                /* RESPONSE CONTROLEREN
                   response.ok controleert of het request succesvol was.
                   Bij een fout wordt een Error aangemaakt. */
                if (!response.ok) {
                    throw new Error("API kon niet worden geladen.");
                }

                /* JSON VERWERKEN
                   response.json leest de ontvangen JSON-gegevens.
                   De gegevens worden omgezet naar een JavaScript-object. */
                return response.json();
            })

            /* WEERGEGEVENS UITLEZEN
               data bevat het JavaScript-object met API-gegevens.
               current bevat de actuele temperatuur en windsnelheid. */
            .then(function(data) {

                const temperatuur = data.current.temperature_2m;
                const wind = data.current.wind_speed_10m;

                /* WEERGEGEVENS TONEN
                   textContent vervangt de laadmelding door de weergegevens.
                   De bezoeker ziet de temperatuur en windsnelheid. */
                weerStatus.textContent =
                    "Temperatuur: " + temperatuur + " °C | Wind: " + wind + " km/u";
            })

            /* FOUTAFHANDELING
               catch vangt fouten op bij het ophalen of verwerken.
               De gebruiker krijgt een melding en de console toont de fout. */
            .catch(function(error) {

                weerStatus.textContent =
                    "Het weer kon niet worden geladen.";

                console.error(error);
            });
    }

    /* WEERGEGEVENS LADEN
       Roept laadWeer aan zodra dit scriptgedeelte wordt uitgevoerd.
       Hierdoor start het API-request automatisch bij het openen. */
    laadWeer();
}