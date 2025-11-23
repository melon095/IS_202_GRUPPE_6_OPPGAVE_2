using Gruppe6Oppgave2.Database.Tables;
using Gruppe6Oppgave2.Models.Report.Response;

namespace Gruppe6Oppgave2.Models.Report;

/// <summary>
///     ViewModel for å vise react kartkomponent med objekter
/// </summary>
public class MapViewModel
{
    public IEnumerable<IMapObject> MapObjects { get; set; } = [];

    public IMapObject? SelectedMapObject { get; set; }

    public string MapElementId { get; set; } = "map";
}

/// <summary>
///     Representerer et interface som kan brukes for objekter som skal vises på kartet
/// </summary>
public interface IMapObject
{
    Guid Id { get; set; }
    string Title { get; set; }
    string Description { get; set; }
    Guid TypeId { get; set; }
    Point? CentroidPoint { get; set; }
    Point[] Points { get; set; }
    GeometryType GeometryType { get; set; }
}
