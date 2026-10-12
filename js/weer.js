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