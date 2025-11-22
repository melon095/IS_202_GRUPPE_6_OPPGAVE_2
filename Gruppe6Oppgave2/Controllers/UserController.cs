using Gruppe6Oppgave2.AuthPolicy;
using Gruppe6Oppgave2.Database;
using Gruppe6Oppgave2.Database.Tables;
using Gruppe6Oppgave2.Models.User;
using Gruppe6Oppgave2.Models.User.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gruppe6Oppgave2.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger; // Logger for logging informasjon og feil
    private readonly UserManager<UserTable> _userManager; // UserManager for håndtering av brukere
    private readonly SignInManager<UserTable> _signInManager; // SignInManager for håndtering av pålogging
    private readonly RoleManager<RoleTable> _roleManager; // RoleManager for håndtering av roller
    private readonly IHttpContextAccessor _httpContextAccessor; // HttpContextAccessor for tilgang til HTTP-konteksten

    // Konstruktør for UserController
    public UserController(ILogger<UserController> logger,
        UserManager<UserTable> userManager,
        SignInManager<UserTable> signInManager,
        RoleManager<RoleTable> roleManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    [AllowAnonymous]// Håndterer GET-forespørsler til /User/Login
    public IActionResult Login(string? returnUrl = null) // Håndterer GET-forespørsler til /User/Login
    {
        UserLoginRequestModel model = new(); // Oppretter en tom modell for pålogging
        ViewData["ReturnUrl"] = returnUrl; // Setter ReturnUrl i ViewData for bruk i viewet

        return View(model); // Returnerer viewet med den tomme modellen
    }

    [HttpGet]
    [AllowAnonymous] // Håndterer GET-forespørsler til /User/AccessDenied
    public IActionResult AccessDenied() => View(); // Returnerer viewet for tilgang nektet

    [HttpGet]
    [Authorize] // Håndterer GET-forespørsler til /User/Logout
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _httpContextAccessor.HttpContext?.Session.Clear();

        return RedirectToAction("Index", "Home"); // Returnerer til hjemmesiden etter utlogging
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() => View(); // Returnerer viewet for registrering

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserLoginRequestModel body, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(body); // Returnerer viewet med modellen hvis modelltilstanden er ugyldig

        returnUrl ??= Url.Content("~/"); // Setter standard returnUrl hvis den er null

        var user = await _userManager.FindByNameAsync(body.Username);
        if (user == null) // Legger til en modellfeil hvis brukeren ikke finnes
        {
            ModelState.AddModelError(string.Empty, "Ugyldig brukernavn eller passord."); // Legger til en modellfeil hvis brukeren ikke finnes

            return View(body); // Returnerer viewet med modellen hvis brukeren ikke finnes
        }

        var result = await _signInManager.PasswordSignInAsync(body.Username, body.Password, body.RememberMe, false);
        if (!result.Succeeded) // Legger til en modellfeil hvis påloggingen mislykkes
        {
            ModelState.AddModelError(string.Empty, "Ugyldig brukernavn eller passord."); // Legger til en modellfeil hvis påloggingen mislykkes

            return View(body); // Returnerer viewet med modellen hvis påloggingen mislykkes
        }

        _httpContextAccessor.HttpContext?.Session.SetString("Username", body.Username); // Setter brukernavn i sesjonen
        _httpContextAccessor.HttpContext?.Session.SetString("UserId", user.Id.ToString()); // Setter bruker-ID i sesjonen

        _logger.LogInformation("Bruker {Username} logget inn.", body.Username); // Logger informasjon om vellykket pålogging
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl); // Omdirigerer til returnUrl hvis den er lokal

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken] // Beskytter mot CSRF-angrep
    public async Task<IActionResult> Register(UserRegisterRequestModel model)
    {
        if (!ModelState.IsValid)
            return View(model); // Returnerer viewet med modellen hvis modelltilstanden er ugyldig

        var role = await _roleManager.FindByNameAsync(RoleValue.User); // Henter standardrollen for nye brukere
        if (role == null) // Legger til en modellfeil hvis standardrollen ikke finnes
        {
            ModelState.AddModelError(string.Empty, "Standardrolle ikke funnet.");
            return View(model); // Returnerer viewet med modellen hvis standardrollen ikke finnes
        }

        var user = new UserTable // Oppretter en ny bruker med informasjon fra modellen
        {
            // Setter standardverdier for den nye brukeren
            UserName = model.Username,
            IsActive = true,
            RoleId = role.Id
        };

        var result = await _userManager.CreateAsync(user, model.Password); // Oppretter brukeren med angitt passord
        if (!result.Succeeded) // Legger til modellfeil hvis opprettelsen av brukeren mislykkes
        {
            // Legger til modellfeil for hver feil som oppstod under opprettelsen av brukeren
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);

            return View(model); // Returnerer viewet med modellen hvis opprettelsen av brukeren mislykkes
        }

        await _userManager.AddToRoleAsync(user, RoleValue.User); // Legger brukeren til standardrollen
        await _signInManager.SignInAsync(user, false); // Logger inn den nye brukeren uten å huske påloggingen

        _logger.LogInformation("Bruker {Username} opprettet en ny konto.", model.Username); // Logger informasjon om vellykket opprettelse av konto

        _httpContextAccessor.HttpContext?.Session.SetString("Username", model.Username); // Setter brukernavn i sesjonen
        _httpContextAccessor.HttpContext?.Session.SetString("UserId", user.Id.ToString()); // Setter bruker-ID i sesjonen

        return RedirectToAction("Index", "Home"); // Omdirigerer til hjemmesiden etter vellykket registrering
    }

    [HttpGet("User/SetRole/{role}")] 
    [Authorize(Policy = RoleValue.AtLeastUser)] // Krever at brukeren har minst User-rollen
    public async Task<IActionResult> SetRole([FromRoute] string role)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound(); // Returnerer 404 hvis brukeren ikke finnes

        var newRole = await _roleManager.FindByNameAsync(role);
        if (newRole == null)
            return NotFound(); // Returnerer 404 hvis den angitte rollen ikke finnes

        var currentRoles = await _userManager.GetRolesAsync(user); // Henter brukerens nåværende roller
        await _userManager.RemoveFromRolesAsync(user, currentRoles); // Fjerner brukeren fra alle nåværende roller
        await _userManager.AddToRoleAsync(user, role); // Legger brukeren til den nye rollen

        user.RoleId = newRole.Id; // Oppdaterer brukerens rolle-ID
        await _userManager.UpdateAsync(user); // Oppdaterer brukerens informasjon i databasen

        await _signInManager.SignInAsync(user, false); // Logger inn brukeren på nytt for å oppdatere rolleinformasjonen

        return RedirectToAction("Index", "Home"); // Omdirigerer til hjemmesiden etter vellykket rolleendring
    }
}
