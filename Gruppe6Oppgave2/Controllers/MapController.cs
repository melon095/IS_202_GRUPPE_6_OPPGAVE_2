using Gruppe6Oppgave2.AuthPolicy;
using Gruppe6Oppgave2.Database;
using Gruppe6Oppgave2.Database.Tables;
using Gruppe6Oppgave2.Models.Map;
using Gruppe6Oppgave2.Models.Map.Request;
using Gruppe6Oppgave2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gruppe6Oppgave2.Controllers;

[Controller]
[Authorize]
public class MapController : Controller
{
    private readonly ILogger<MapController> _logger; // Logger for logging informasjon og feil
    private readonly IHindranceService _hindranceService; // Service for håndtering av hindringer
    private readonly IJourneyOrchestrator _journeyOrchestrator; // Orkestrator for reisehåndtering
    private readonly IUnitOfWork _unitOfWork; // Enhet for håndtering av databaseoperasjoner
    private readonly UserManager<UserTable> _userManager; // UserManager for håndtering av brukere
    private readonly IHttpClientFactory _httpClientFactory; // Factory for å opprette HttpClient-instans

    public MapController(
        // Avhengighetsinjeksjon av nødvendige tjenester
        ILogger<MapController> logger,
        IHindranceService hindranceService,
        IJourneyOrchestrator journeyOrchestrator,
        IUnitOfWork unitOfWork,
        UserManager<UserTable> userManager,
        IHttpClientFactory httpClientFactory)
    {
        // Initialisering av felt med injiserte tjenester
        _logger = logger;
        _hindranceService = hindranceService;
        _journeyOrchestrator = journeyOrchestrator;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Index() => View(); // Returnerer standardvisningen for kartet

    [HttpPost]
    public async Task<IActionResult> SyncObject( // Synkroniserer et objekt i en reise
        [FromBody] PlacedObjectDataModel body, // DataModel som inneholder informasjon om objektet
        [FromQuery] Guid? journeyId = null, // Valgfritt reise-ID
        CancellationToken cancellationToken = default) // Avbestillings-token for asynkrone operasjoner
    {
        // Validerer modelltilstanden
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userManager.GetUserAsync(User); // Henter den nåværende brukeren
        if (user == null) return Unauthorized(); // Returnerer 401 hvis brukeren ikke er autentisert

        try // Prøver å synkronisere objektet
        {
            var (resultJourneyId, resultObjectId) = await _unitOfWork.ExecuteInTransactionAsync( // Utfører operasjonen i en database-transaksjon
                () => _journeyOrchestrator.SyncObject(user.Id, journeyId, body, cancellationToken), 
                cancellationToken); // Kaller orkestratoren for å synkronisere objektet

            return Ok(new { JourneyId = resultJourneyId, ObjectId = resultObjectId }); // Returnerer resultatet som JSON
        }
        catch (Exception ex) // Fanger opp eventuelle unntak som oppstår under synkroniseringen
        {
            _logger.LogError(ex, "Error syncing object for user {UserId}", user.Id); // Logger feilen
            return StatusCode(500, ex.Message); // Returnerer 500 med feilmeldingen
        }
    }

    [HttpPost]
    public async Task<IActionResult> FinalizeJourney(
        [FromBody] FinalizeJourneyRequest body,
        [FromQuery] Guid? journeyId = null,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState); // Validerer modelltilstanden
        if (journeyId is null) return BadRequest("JourneyId is required"); // Sjekker om reise-ID er gitt

        try // Prøver å fullføre reisen
        {
            var resultId = await _unitOfWork.ExecuteInTransactionAsync( // Utfører operasjonen i en database-transaksjon
                () => _journeyOrchestrator.Finalise(journeyId.Value, body, cancellationToken),
                cancellationToken); // Kaller orkestratoren for å fullføre reisen

            return Ok(new { JourneyId = resultId }); // Returnerer resultatet som JSON
        }
        catch (InvalidOperationException ex) // Fanger opp spesifikke unntak for ugyldige operasjoner
        {
            _logger.LogWarning(ex, "Journey finalization failed for {JourneyId}", journeyId); // Logger en advarsel
            return BadRequest(ex.Message); // Returnerer 400 med feilmeldingen
        }
        catch (Exception ex) // Fanger opp andre unntak
        {
            _logger.LogError(ex, "Error finalizing journey {JourneyId}", journeyId); // Logger feilen
            return StatusCode(500, ex.Message); // Returnerer 500 med feilmeldingen
        }
    }

    [HttpGet]
    public async Task<IEnumerable<MapObjectDataModel>> GetObjects(
        [FromQuery] DateTime? since = null,
        [FromQuery] Guid? reportId = null,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.GetUserAsync(User); // Henter den nåværende brukeren
        if (user == null)
            return []; // Returnerer tom liste hvis brukeren ikke er autentisert

        var roles = await _userManager.GetRolesAsync(user); // Henter brukerens roller
        if (roles is null || roles.Count == 0)
            return []; // Returnerer tom liste hvis brukeren ikke har noen roller

        var role = roles.Contains(RoleValue.Kartverket) ? RoleValue.Kartverket // Prioriterer Kartverket-rollen
            : roles.Contains(RoleValue.Pilot) ? RoleValue.Pilot // Prioriterer Pilot-rollen
            : roles.Contains(RoleValue.User) ? RoleValue.User // Prioriterer User-rollen
            : null; // Setter rollen til null hvis ingen kjente roller finnes

        if (role == null)
            return []; // Returnerer tom liste hvis ingen gyldig rolle er funnet

        // Henter alle objekter siden angitt dato
        return await _hindranceService.GetAllObjectsSince(user, role, reportId, since, cancellationToken);
    }

    [HttpGet("/Map/SatelliteTiles/{z:int}/{x:int}/{y:int}.jpg")] // Henter satellittfliser for kartvisning
    public async Task<IActionResult> SatelliteTiles(int x, int y, int z,
        CancellationToken cancellationToken = default) // Henter satellittfliser for kartvisning
    {
        try // Prøver å hente satellittflisen
        {
            using var client = _httpClientFactory.CreateClient("StadiaTiles"); // Oppretter en HttpClient for å hente fliser

            var response = await client.GetAsync($"/tiles/alidade_satellite/{z}/{x}/{y}.jpg", cancellationToken);
            if (!response.IsSuccessStatusCode) // Sjekker om forespørselen var vellykket
            {
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken); // Leser responsinnholdet
                _logger.LogWarning("Failed to fetch satellite tile {Z}/{X}/{Y}: {StatusCode} {ResponseBody}",
                    z, x, y, response.StatusCode, responseBody); // Logger en advarsel hvis flisen ikke kunne hentes

                return NotFound(); // Returnerer 404 hvis flisen ikke finnes
            }

            var stream = await response.Content.ReadAsStreamAsync(cancellationToken); // Leser innholdet som en strøm

            HttpContext.Response.Headers.ETag = response.Headers.ETag?.Tag ?? null; // Setter ETag-headeren i responsen
            HttpContext.Response.Headers.LastModified = response.Content.Headers.LastModified?.ToString() ?? null; // Setter Last-Modified-headeren i responsen
            HttpContext.Response.Headers.CacheControl =
                response.Headers.CacheControl?.ToString() ?? "public,max-age=3600"; // Setter Cache-Control-headeren i responsen

            return File(stream, "image/jpeg"); // Returnerer flisen som en JPEG-bildestrøm
        }
        catch
        {
            return NotFound(); // Returnerer 404 hvis det oppstår en feil
        }
    }
}
