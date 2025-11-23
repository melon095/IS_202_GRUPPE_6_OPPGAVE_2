using Microsoft.AspNetCore.Authorization;

namespace Gruppe6Oppgave2.AuthPolicy;

public record MinimumRoleRequirement(string MinimumRole) : IAuthorizationRequirement;
