using Microsoft.AspNetCore.Authorization;

namespace Gruppe6Oppgave2.AuthPolicy;

//Her defineres kravet for minimum rolle som en record som implementerer IAuthorizationRequirement
public record MinimumRoleRequirement(string MinimumRole) : IAuthorizationRequirement;
