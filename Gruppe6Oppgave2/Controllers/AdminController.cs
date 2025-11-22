using Gruppe6Oppgave2.AuthPolicy;
using Gruppe6Oppgave2.Database;
using Gruppe6Oppgave2.Database.Tables;
using Gruppe6Oppgave2.Models.Report.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gruppe6Oppgave2.Controllers;

[Controller]
[Authorize(Policy = RoleValue.AtLeastKartverket)] //Bare kartverket har tilgang til denne controlleren
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger; // Logger for logging informasjon og feil
    private readonly DatabaseContext _dbContext; // Database context for tilgang til databasen

    // Konstruktør for AdminController
    public AdminController(ILogger<AdminController> logger, DatabaseContext ctx)
    {
        _logger = logger;
        _dbContext = ctx;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // Endrer status på et objekt i en rapport basert på admin sin vurdering
    public IActionResult ObjectReview(Guid id, Guid objectId, ObjectReviewAction StatusObject)
    {
        // Hent rapporten fra databasen inkludert hindringsobjekter og tilbakemeldinger
        var report = _dbContext.Reports
            .Include(r => r.HindranceObjects)
            .ThenInclude(o => o.HindrancePoints)
            .Include(r => r.Feedbacks)
            .FirstOrDefault(r => r.Id == id);

        // Sjekk om rapporten finnes
        if (report == null)
            return View("ErrorView"); //Returnerer viewet ErrorView hvis rapporten ikke finnes

        // Finn det spesifikke objektet i rapporten
        var selectedObject = report.HindranceObjects.SingleOrDefault(x => x.Id == objectId);
        if (selectedObject == null) //Dersom den ikke finnes
        {
            // Legg til en modellfeil
            ModelState.AddModelError("", "Objektet ble ikke funnet");
            return View("ErrorView", ViewData); //Returnerer viewet ErrorView hvis objektet ikke finnes
        }

        // Oppdater objektets status basert på admin sin vurdering
        selectedObject.ReviewStatus = StatusObject switch
        {
            //Basert på valgt handling, sett riktig ReviewStatus
            ObjectReviewAction.Accept => ReviewStatus.Resolved,
            ObjectReviewAction.Deny => ReviewStatus.Closed,
            _ => throw new ArgumentOutOfRangeException(nameof(StatusObject), "Ugyldig status valgt") //Håndterer ugyldig statusvalg
        };
        // Sett tidspunktet for verifisering til nåværende tid
        selectedObject.VerifiedAt = DateTime.UtcNow;

        // Verifiser rapportens overordnede status basert på objektenes status
        var reportVerify = report.HindranceObjects;

        // Finn objekter som ikke er gjennomgått og de som er avslått
        var notReviewed = reportVerify.Where(o => o.ReviewStatus == ReviewStatus.Draft).ToList();
        // Finn objekter som er avslått
        var rejectedObjects = reportVerify.Where(o => o.ReviewStatus == ReviewStatus.Closed).ToList();

        // Dersom det ikke er noen rapporter å se gjennom
        if (notReviewed.Count == 0)
        {
            report.ReviewStatus = ReviewStatus.Resolved;
            _logger.LogInformation("Alle objekter i rapporten er vurdert ({ID})", report.Id); //Logg informasjon om at alle objekter er vurdert
        }

        // Dersom alle objektene er avvist
        if (rejectedObjects.Count == reportVerify.Count)
        {
            // Sett rapportens status til "Closed"
            report.ReviewStatus = ReviewStatus.Closed;
            _logger.LogInformation("Rapport rejecta! ({ID})", report.Id);
        }

        // Lagre endringene i databasen
        _dbContext.SaveChanges();

        // Sett en suksessmelding og omdiriger tilbake til objektets side i rapporten
        TempData["Success"] = $"Objekt status endret til {selectedObject.ReviewStatus.GetDisplayName()}";
        return RedirectToAction("Object", "Report", new { reportId = id, objectId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // Legger til en kommentar/tilbakemelding på et objekt i en rapport
    public IActionResult Comment(Guid id, Guid objectID, string feedbackText, FeedbackType feedbackType)
    {
        // Sjekk om tilbakemeldingen er tom eller bare inneholder hvite tegn
        if (string.IsNullOrWhiteSpace(feedbackText))
        {
            TempData["Error"] = "Du kan ikke sende blank tilbakemelding";
            return RedirectToAction("Object", "Report", new { reportId = id, objectId = objectID }); // Omdiriger tilbake til objektets side i rapporten
        }

        // Hent rapporten fra databasen inkludert tilbakemeldinger og hindringsobjekter
        var report = _dbContext.Reports
            .Include(r => r.Feedbacks)
            .ThenInclude(r => r.FeedbackBy)
            .Include(r => r.HindranceObjects)
            .FirstOrDefault(r => r.Id == id);

        // Sjekk om rapporten finnes
        if (report == null)
        {
            ModelState.AddModelError("", "Rapprten ble ikke funnet");
            return View("ErrorView"); //Returnerer viewet ErrorView hvis rapporten ikke finnes
        }

        // Finn det spesifikke objektet i rapporten
        var obj = report.HindranceObjects.FirstOrDefault(o => o.Id == objectID);
        if (obj == null)
        {
            ModelState.AddModelError("", "Objektet ble ikke funnet");
            return RedirectToAction("Object", "Report", new { reportId = id, objectId = objectID });
        }

        // Hent bruker-ID fra pålogget bruker
        var ClaimUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Sjekk om bruker-ID er gyldig, ellers returner Unauthorized
        if (!Guid.TryParse(ClaimUserId, out var userId)) return Unauthorized();
        var feedback = new ReportFeedbackTable // Oppretter en ny tilbakemelding
        {
            //Innhold i tilbakemeldingen
            Id = Guid.NewGuid(),
            Feedback = feedbackText,
            FeedbackType = feedbackType,
            FeedbackById = userId,
            ReportId = report.Id,
            CreatedAt = DateTime.UtcNow
        };

        // Legg til tilbakemeldingen i databasen og lagre endringene
        _dbContext.ReportFeedbacks.Add(feedback);
        _dbContext.SaveChanges();

        // Sett en suksessmelding og omdiriger tilbake til objektets side i rapporten
        TempData["Success"] = "Tilbakemeldingen er sendt";
        return RedirectToAction("Object", "Report", new { reportId = id, objectId = objectID });
    }
}
