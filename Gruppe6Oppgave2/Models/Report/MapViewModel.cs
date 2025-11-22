using Gruppe6Oppgave2.Database.Tables;
using Gruppe6Oppgave2.Models.Report.Response;

namespace Gruppe6Oppgave2.Models.Report;

// Modell for visning av kart med objekter
public class MapViewModel
{
    public IEnumerable<IMapObject> MapObjects { get; set; } = []; // Liste over kartobjekter

    public IMapObject? SelectedMapObject { get; set; } // Valgt kartobjekt

    public string MapElementId { get; set; } = "map"; // Id for HTML-elementet som inneholder kartet
}

// Grensesnitt for kartobjekter
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
