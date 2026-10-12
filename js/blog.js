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