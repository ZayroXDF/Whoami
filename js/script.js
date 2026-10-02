// =========================
// PROJECTEN
// =========================

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

const projectGrid = document.getElementById("project-grid");

function toonProjecten(lijst) {
    projectGrid.innerHTML = "";

    lijst.forEach(function(project) {
        const article = document.createElement("article");
        article.classList.add("project-card");

        const nummer = document.createElement("div");
        nummer.classList.add("project-number");
        nummer.textContent = project.nummer;

        const titel = document.createElement("h2");
        titel.textContent = project.titel;

        const beschrijving = document.createElement("p");
        beschrijving.textContent = project.beschrijving;

        const tags = document.createElement("div");
        tags.classList.add("tags");

        project.tags.forEach(function(tag) {
            const span = document.createElement("span");
            span.textContent = tag;
            tags.appendChild(span);
        });

        article.appendChild(nummer);
        article.appendChild(titel);
        article.appendChild(beschrijving);
        article.appendChild(tags);

        projectGrid.appendChild(article);
    });
}


// Alleen uitvoeren wanneer project-grid op de pagina bestaat
if (projectGrid) {

    toonProjecten(projecten);

    document.getElementById("toon-alles").addEventListener("click", function() {
        toonProjecten(projecten);
    });

    document.getElementById("toon-java").addEventListener("click", function() {
        const javaProjecten = projecten.filter(function(project) {
            return project.tags.includes("Java");
        });

        toonProjecten(javaProjecten);
    });

    document.getElementById("toon-web").addEventListener("click", function() {
        const webProjecten = projecten.filter(function(project) {
            return project.tags.includes("Web");
        });

        toonProjecten(webProjecten);
    });
}


// =========================
// BLOG
// =========================

const leesMeerKnoppen = document.querySelectorAll(".lees-meer");

leesMeerKnoppen.forEach(function(knop) {

    knop.addEventListener("click", function() {

        const extraTekst = knop.nextElementSibling;

        extraTekst.classList.toggle("open");

        if (extraTekst.classList.contains("open")) {
            knop.textContent = "Lees minder";
        } else {
            knop.textContent = "Lees meer";
        }

    });

});

// =========================
// CONTACTFORMULIER
// =========================

const contactForm = document.getElementById("contact-form");

if (contactForm) {

    contactForm.addEventListener("submit", function(event) {

        // Voorkomt dat het formulier echt wordt verstuurd
        event.preventDefault();

        const naam = document.getElementById("naam");
        const email = document.getElementById("email");
        const bericht = document.getElementById("bericht");

        const naamFout = document.getElementById("naam-fout");
        const emailFout = document.getElementById("email-fout");
        const berichtFout = document.getElementById("bericht-fout");
        const resultaat = document.getElementById("form-resultaat");

        // Oude foutmeldingen verwijderen
        naamFout.textContent = "";
        emailFout.textContent = "";
        berichtFout.textContent = "";
        resultaat.textContent = "";

        let formulierGeldig = true;


        // Naam controleren
        if (naam.value.trim() === "") {
            naamFout.textContent = "Vul je naam in.";
            formulierGeldig = false;
        }


        // E-mailadres controleren
        if (email.value.trim() === "") {
            emailFout.textContent = "Vul je e-mailadres in.";
            formulierGeldig = false;
        } else if (!email.validity.valid) {
            emailFout.textContent = "Vul een geldig e-mailadres in.";
            formulierGeldig = false;
        }


        // Bericht controleren
        if (bericht.value.trim() === "") {
            berichtFout.textContent = "Vul een bericht in.";
            formulierGeldig = false;
        } else if (bericht.value.trim().length < 10) {
            berichtFout.textContent =
                "Het bericht moet minimaal 10 tekens bevatten.";
            formulierGeldig = false;
        }


        // Alles correct
        if (formulierGeldig) {
            resultaat.textContent =
                "Bedankt! Het formulier is correct ingevuld.";

            contactForm.reset();
        }

    });

}

// =========================
// WEER API
// =========================

const weerElement = document.getElementById("weer");
const weerStatus = document.getElementById("weer-status");

if (weerElement && weerStatus) {

    function laadWeer() {

        weerStatus.textContent = "Weer laden...";

        const url =
            "https://api.open-meteo.com/v1/forecast?latitude=52.07&longitude=4.30&current=temperature_2m,wind_speed_10m";

        fetch(url)
            .then(function(response) {

                if (!response.ok) {
                    throw new Error("API kon niet worden geladen.");
                }

                return response.json();
            })

            .then(function(data) {

                const temperatuur = data.current.temperature_2m;
                const wind = data.current.wind_speed_10m;

                weerStatus.textContent =
                    "Temperatuur: " + temperatuur + " °C | Wind: " + wind + " km/u";
            })

            .catch(function(error) {

                weerStatus.textContent =
                    "Het weer kon niet worden geladen.";

                console.error(error);
            });
    }

    laadWeer();
}