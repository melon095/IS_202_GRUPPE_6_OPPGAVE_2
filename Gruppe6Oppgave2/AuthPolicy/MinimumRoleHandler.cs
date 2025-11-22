using Microsoft.AspNetCore.Authorization;

namespace Gruppe6Oppgave2.AuthPolicy;

// Her implementeres håndteringen av MinimumRoleRequirement
public class MinimumRoleHandler : AuthorizationHandler<MinimumRoleRequirement>
{
    // Definerer et ordbok som representerer rollehierarkiet med tilhørende verdier
    public static readonly Dictionary<string, int> RoleHierarchy = new()
    {
        // Hver rolle har sin verdi i hierarkiet
        { RoleValue.User, 1 },
        { RoleValue.Pilot, 2 },
        { RoleValue.Kartverket, 3 }
    };

    // Håndterer autorisasjonskravet ved å sjekke brukerens roller mot minimumsrollen
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        MinimumRoleRequirement requirement)
    {
        // Itererer gjennom rollehierarkiet for å sjekke om brukeren har tilstrekkelig rolle
        foreach (var (key, value) in RoleHierarchy)
        {
            // Sjekker om brukeren har rollen og om dens verdi er større enn eller lik minimumsrollen
            if (context.User.IsInRole(key) && value >= RoleHierarchy[requirement.MinimumRole])
            {
                // Hvis betingelsene er oppfylt, godkjenn kravet
                context.Succeed(requirement);
                break;
            }
            
        }
        // Fullfører oppgaven uten å returnere en verdi
        return Task.CompletedTask;
    }
}
