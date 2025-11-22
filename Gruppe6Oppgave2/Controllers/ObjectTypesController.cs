using Gruppe6Oppgave2.Models.ObjectTypes.Response;
using Gruppe6Oppgave2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gruppe6Oppgave2.Controllers;

public class ObjectTypesController : Controller
{
    // Henter en liste over objekt typer
    public async Task<ObjectTypesDataModel> List([FromServices] IObjectTypesService objectTypesService,
        CancellationToken cancellationToken = default) =>
        await objectTypesService.List(cancellationToken);
}
