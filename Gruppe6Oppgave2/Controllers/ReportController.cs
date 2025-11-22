using Gruppe6Oppgave2.AuthPolicy;
using Gruppe6Oppgave2.Database;
using Gruppe6Oppgave2.Models.Report;
using Gruppe6Oppgave2.Models.Report.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gruppe6Oppgave2.Controllers;

[Authorize]
public class ReportController : Controller
{
    private const int ReportPerPage = 10; // Antall rapporter per side for paginering

    private readonly DatabaseContext _dbContext; // Database context for tilgang til databasen

    public ReportController(DatabaseContext dbContext) // Konstruktør for ReportController
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    // Håndterer GET-forespørsler til /Report/Index
    public async Task<IActionResult> Index(ReportIndexViewModel model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return View(model); // Returnerer viewet med modellen hvis modelltilstanden er ugyldig

        var reportQuery = _dbContext.Reports // Starter spørringen mot Reports-tabellen
            .Where(r => DateOnly.FromDateTime(r.CreatedAt) <= model.SortDate)
            .AsQueryable(); // Gjør spørringen queryable for videre filtrering

        if (model.SortStatus is { } status) // Filtrerer rapporter basert på valgt status
            reportQuery = reportQuery.Where(r => r.ReviewStatus == status);

        var totalReports = await reportQuery.CountAsync(cancellationToken); // Teller totalt antall rapporter som matcher filtrene
        var totalPages = (int)Math.Ceiling(totalReports / (double)ReportPerPage); // Beregner totalt antall sider for paginering

        reportQuery = model.SortOrder == SortOrder.Ascending // Sorterer rapporter basert på valgt sorteringsrekkefølge
            ? reportQuery.OrderBy(r => r.CreatedAt)
            : reportQuery.OrderByDescending(r => r.CreatedAt); // Standard sortering er synkende

        var reports = await reportQuery // Henter rapporter for den gjeldende siden
            .Skip((model.Page - 1) * ReportPerPage) // Hopper over rapporter fra tidligere sider
            .Take(ReportPerPage) // Tar kun rapporter for den gjeldende siden
            .Include(r => r.ReportedBy) // Inkluderer relaterte brukerinformasjon
            .Include(r => r.HindranceObjects) // Inkluderer relaterte hindringsobjekter
            .ToListAsync(cancellationToken); // Utfører spørringen asynkront

        model.CurrentPage = model.Page; // Setter den nåværende siden i modellen
        model.TotalPages = totalPages; // Setter totalt antall sider i modellen
        model.Reports = reports.Select(r => new ReportIndexViewModel.Report // Mapper rapportdata til visningsmodellen
        {
            // Mapper rapportdata til visningsmodellen
            Id = r.Id,
            User = r.ReportedBy?.UserName ?? "Ukjent",
            Title = r.Title,
            CreatedAt = r.CreatedAt,
            Review = r.ReviewStatus,
            TotalObjects = r.HindranceObjects.Count
        }).ToList(); // Konverterer til liste

        return View(model); // Returnerer viewet med den fylte modellen
    }

    [HttpGet("Report/Details/{reportId:guid}/{objectId:guid?}")] // Håndterer GET-forespørsler til /Report/Details/{reportId}/{objectId}
    public async Task<IActionResult> Details(ReportDetailsViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid) 
            return View(model); // Returnerer viewet med modellen hvis modelltilstanden er ugyldig

        // Henter rapporten fra databasen basert på rapport-ID
        var report = await _dbContext.Reports
            .Where(r => r.Id == model.ReportId)
            .Include(r => r.ReportedBy)
            .Include(r => r.HindranceObjects)
            .ThenInclude(ho => ho.HindrancePoints)
            .FirstOrDefaultAsync(cancellationToken);

        if (report is null)
        {
            ModelState.AddModelError(string.Empty, "Rapporten ble ikke funnet.");

            return View("ErrorView", model); // Returnerer ErrorView hvis rapporten ikke finnes
        }

        var selectedObject = report.HindranceObjects // Finn det valgte hindringsobjektet basert på objekt-ID
            .FirstOrDefault(ho => ho.Id == model.ObjectId); // Hvis objectId er null, vil dette være null

        // Fyller ut rapportdetaljene i modellen
        model.ReportId = report.Id;
        model.Title = report.Title;
        model.Description = report.Description;
        model.CreatedAt = report.CreatedAt;

        // Fyller ut hindringsobjektene i modellen
        foreach (var obj in report.HindranceObjects)
        {
            // Mapper hindringsobjektdetaljer til visningsmodellen
            var objectModel = new ReportDetailsViewModel.ObjectDataModel
            {
                // Mapper hindringsobjektdetaljer til visningsmodellen
                Id = obj.Id,
                Title = obj.Title,
                Description = obj.Description,
                TypeId = obj.HindranceTypeId,
                ObjectStatus = obj.ReviewStatus,
                VerifiedAt = obj.VerifiedAt,
                GeometryType = obj.GeometryType,
                CentroidPoint = obj.HindrancePoints.Count > 0
                    ? new Point( // Beregner sentrumspunktet basert på gjennomsnittlige koordinater
                        obj.HindrancePoints.Average(p => p.Latitude),
                        obj.HindrancePoints.Average(p => p.Longitude)) // Beregner sentrumspunktet basert på gjennomsnittlige koordinater
                    : null, // Setter til null hvis det ikke finnes noen punkter
                Points = obj.HindrancePoints // Henter og mapper hindringspunktene til visningsmodellen
                    .OrderBy(p => p.Order) // Sorterer punktene basert på rekkefølge
                    .Select(p => new Point(p)) // Mapper hvert hindringspunkt til Point-modellen
                    .ToArray() // Konverterer til array
            };

            model.Objects.Add(objectModel); // Legger til hindringsobjektet i modellens liste

            if (selectedObject != null && obj.Id == selectedObject.Id)
                model.SelectedObject = objectModel; // Setter det valgte objektet i modellen hvis det matcher
        }

        return View(model); // Returnerer viewet med den fylte modellen
    }

    [HttpGet("Report/Object/{reportId:guid}/{objectId:guid}")]
    public async Task<IActionResult> Object(ReportObjectViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Henter rapporten fra databasen basert på rapport-ID
        var report = await _dbContext.Reports
            .Where(r => r.Id == model.ReportId)
            .Include(r => r.HindranceObjects)
            .ThenInclude(ho => ho.HindrancePoints)
            .Include(r => r.Feedbacks)
            .ThenInclude(f => f.FeedbackBy)
            .FirstOrDefaultAsync(cancellationToken);

        if (report is null)
        {
            ModelState.AddModelError(string.Empty, "Rapporten ble ikke funnet.");

            return View("ErrorView", model); // Returnerer ErrorView hvis rapporten ikke finnes
        }

        // Fyller ut rapportdetaljene i modellen
        model.ReportId = report.Id;
        model.Title = report.Title;
        model.Description = report.Description;
        model.CreatedAt = report.CreatedAt;
        model.ReviewStatus = report.ReviewStatus;
        model.IsKartverket = User.HasAtLeastRole(RoleValue.Kartverket);

        foreach (var obj in report.HindranceObjects)
        {
            var objectModel = new ReportObjectViewModel.ObjectDataModel
            {
                Id = obj.Id,
                Title = obj.Title,
                Description = obj.Description,
                TypeId = obj.HindranceTypeId,
                ObjectStatus = obj.ReviewStatus,
                VerifiedAt = obj.VerifiedAt,
                GeometryType = obj.GeometryType,
                CentroidPoint = obj.HindrancePoints.Count > 0
                    ? new Point(
                        obj.HindrancePoints.Average(p => p.Latitude),
                        obj.HindrancePoints.Average(p => p.Longitude))
                    : null,
                Points = obj.HindrancePoints
                    .OrderBy(p => p.Order)
                    .Select(p => new Point(p))
                    .ToArray(),
                Feedbacks = report.Feedbacks
                    .Where(f => f.ReportId == report.Id)
                    .OrderBy(f => f.CreatedAt)
                    .Select(f => new ReportObjectViewModel.FeedbackModel
                    {
                        Id = f.Id,
                        Feedback = f.Feedback,
                        FeedbackType = f.FeedbackType,
                        FeedbackById = f.FeedbackById,
                        FeedbackByName = f.FeedbackBy?.UserName ?? "Ukjent",
                        CreatedAt = f.CreatedAt
                    })
                    .ToArray()
            };

            if (obj.Id == model.ObjectId)
                model.SelectedObject = objectModel; // Setter det valgte objektet i modellen hvis det matcher

            model.Objects.Add(objectModel); // Legger til hindringsobjektet i modellens liste
        }

        return View(model); // Returnerer viewet med den fylte modellen
    }
}
