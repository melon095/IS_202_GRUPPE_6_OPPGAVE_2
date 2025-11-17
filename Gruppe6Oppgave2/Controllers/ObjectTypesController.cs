using Gruppe6Oppgave2.Models.ObjectTypes.Response;
using Gruppe6Oppgave2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gruppe6Oppgave2.Controllers;

public class ObjectTypesController : Controller
{
    private readonly IObjectTypesService _objectTypesService;

    public ObjectTypesController(IObjectTypesService objectTypesService)
    {
        _objectTypesService = objectTypesService;
    }

    public async Task<ObjectTypesDataModel> List(CancellationToken cancellationToken = default) =>
        await _objectTypesService.List(cancellationToken);
}
