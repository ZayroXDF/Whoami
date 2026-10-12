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