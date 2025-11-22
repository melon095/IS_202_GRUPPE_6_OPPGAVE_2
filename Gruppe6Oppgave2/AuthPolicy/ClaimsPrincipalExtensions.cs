using System.Security.Claims;

namespace Gruppe6Oppgave2.AuthPolicy;

public static class ClaimsPrincipalExtensions
{
    // Sjekker om brukeren har minst den angitte rollen basert på rollehierarkiet
    public static bool HasAtLeastRole(this ClaimsPrincipal user, string minimumRole)
    {
        // Gå gjennom alle roller i rollehierarkiet
        foreach (var role in MinimumRoleHandler.RoleHierarchy.Keys)
        {
            // Hent hierarkiverdiene for den nåværende rollen og minimumsrollen
            var l = MinimumRoleHandler.RoleHierarchy[role];
            var r = MinimumRoleHandler.RoleHierarchy[minimumRole];

            // Sjekk om brukeren har den nåværende rollen og om dens verdi er større enn eller lik minimumsrollen
            if (user.IsInRole(role) && l >= r)
            {
                return true; // Brukeren har minst den angitte rollen
            }
        }

        return false; // Brukeren har ikke minst den angitte rollen
    }
}
