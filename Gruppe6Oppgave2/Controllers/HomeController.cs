using Gruppe6Oppgave2.Database;
using Gruppe6Oppgave2.Database.Tables;
using Gruppe6Oppgave2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Gruppe6Oppgave2.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    // Denne metoden håndterer feil og returnerer en feilsiden med relevant informasjon
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel
        { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    [Authorize]
    // Fyller databasen med testdata for debugging-formål
    public async Task<IActionResult> DebugFillDatabase(
        [FromServices] DatabaseContext dbContext,
        [FromServices] UserManager<UserTable> userManager,
        CancellationToken cancellationToken = default)
    {
        // Antall iterasjoner og størrelse for generering av hindringer
        const int ITERATIONS = 100;
        const double SIZE = 0.1;

        // Leftmost = 58.069402, 7.428365
        // Rightmost = 58.307080, 8.435572

        // Hent den nåværende brukeren
        var user = await userManager.GetUserAsync(User);

        // Hent alle hindringstyper fra databasen
        var hindranceTypes = dbContext.HindranceTypes.ToList();
        // Opprett en ny rapport
        var report = new ReportTable
        {
            // Fyller ut rapportdetaljer
            Id = Guid.NewGuid(),
            Title = "Debug Fill Report",
            Description = "This report was created to fill the database with test data.",
            ReviewStatus = ReviewStatus.Draft,
            ReportedById = user.Id
        };

        List<HindranceObjectTable> hindranceObjects = new(); // Liste for å lagre hindringsobjekter
        List<HindrancePointTable> hindrancePoints = new(); // Liste for å lagre hindringspunkter
        Random random = new(); // Random-objekt for å generere tilfeldige tall
        for (var i = 0; i < ITERATIONS; i++) // Loop for å generere hindringer
        {
            // Generer tilfeldige koordinater innenfor spesifisert område
            var lat = 58.069402 + random.NextDouble() * (58.307080 - 58.069402);
            var lon = 7.428365 + random.NextDouble() * (8.435572 - 7.428365);
            HindranceObjectTable hindranceObject = new() // Opprett et nytt hindringsobjekt
            {
                // Fyller ut hindringsobjektets detaljer
                Title = $"Hindrance {i + 1}",
                Description = "This is a randomly generated hindrance object for testing purposes.",
                Report = report
            };
            hindranceObjects.Add(hindranceObject); // Legg til hindringsobjektet i listen

            var pointType = random.Next(1, 4); // Velg en tilfeldig geometri-type (1: Punkt, 2: Linje, 3: Område)
            if (pointType == 1) // Punkt
            {
                HindrancePointTable hindrancePoint = new() // Opprett et nytt hindringspunkt
                {
                    // Fyller ut hindringspunktets detaljer
                    HindranceObject = hindranceObject,
                    Latitude = lat,
                    Longitude = lon,
                    Label = ""
                };
                hindrancePoints.Add(hindrancePoint); // Legg til hindringspunktet i listen
                hindranceObject.GeometryType = GeometryType.Point; // Sett geometri-typen til punkt
                hindranceObject.HindranceTypeId = hindranceTypes.First(ht => // Finn hindringstypen for punkt
                    ht.GeometryType == GeometryType.Point && ht.Name == HindranceTypeTable.DEFAULT_TYPE_NAME).Id; // Hent ID
            }
            else if (pointType == 2) // Linje 
            {
                HindrancePointTable hindrancePoint1 = new() // Opprett et nytt hindringspunkt
                {
                    // Fyller ut hindringspunktets detaljer
                    HindranceObject = hindranceObject,
                    Latitude = lat,
                    Longitude = lon,
                    Label = ""
                };
                HindrancePointTable hindrancePoint2 = new() // Opprett et nytt hindringspunkt
                {
                    // Fyller ut hindringspunktets detaljer
                    HindranceObject = hindranceObject,
                    Latitude = lat + random.NextDouble() * SIZE,
                    Longitude = lon + random.NextDouble() * SIZE,
                    Label = ""
                };
                hindrancePoints.Add(hindrancePoint1); // Legg til hindringspunktet i listen
                hindrancePoints.Add(hindrancePoint2); 
                hindranceObject.GeometryType = GeometryType.Line; // Sett geometri-typen til linje
                hindranceObject.HindranceTypeId = hindranceTypes.First(ht => // Finn hindringstypen for linje
                    ht.GeometryType == GeometryType.Line && ht.Name == HindranceTypeTable.DEFAULT_TYPE_NAME).Id; // Hent ID
            }
            else // Område
            {
                HindrancePointTable hindrancePoint1 = new() // Opprett et nytt hindringspunkt
                {
                    // Fyller ut hindringspunktets detaljer
                    HindranceObject = hindranceObject,
                    Latitude = lat,
                    Longitude = lon,
                    Label = ""
                };
                HindrancePointTable hindrancePoint2 = new() // Opprett et nytt hindringspunkt
                {
                    // Fyller ut hindringspunktets detaljer
                    HindranceObject = hindranceObject,
                    Latitude = lat + random.NextDouble() * SIZE,
                    Longitude = lon + random.NextDouble() * SIZE,
                    Label = ""
                };
                HindrancePointTable hindrancePoint3 = new() //  Opprett et nytt hindringspunkt
                {
                    // Fyller ut hindringspunktets detaljer
                    HindranceObject = hindranceObject,
                    Latitude = lat + random.NextDouble() * SIZE,
                    Longitude = lon + random.NextDouble() * SIZE,
                    Label = ""
                };
                hindrancePoints.Add(hindrancePoint1);
                hindrancePoints.Add(hindrancePoint2);
                hindrancePoints.Add(hindrancePoint3);
                hindranceObject.GeometryType = GeometryType.Area;
                hindranceObject.HindranceTypeId = hindranceTypes.First(ht =>
                    ht.GeometryType == GeometryType.Area && ht.Name == HindranceTypeTable.DEFAULT_TYPE_NAME).Id;
            }
        }

        dbContext.HindranceObjects.AddRange(hindranceObjects); // Legg til hindringsobjektene i databasen
        dbContext.HindrancePoints.AddRange(hindrancePoints); // Legg til hindringspunktene i databasen
        return await dbContext.SaveChangesAsync(cancellationToken) // Lagre endringene i databasen og omdiriger til Index-siden
            .ContinueWith<IActionResult>(_ => RedirectToAction("Index"), cancellationToken);
    }
}
