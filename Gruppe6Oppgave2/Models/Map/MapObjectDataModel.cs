using Gruppe6Oppgave2.Database;
using Gruppe6Oppgave2.Database.Tables;

namespace Gruppe6Oppgave2.Models.Map;

// Data modell for et kartobjekt
public class MapObjectDataModel
{
    public Guid Id { get; set; }
    public Guid ReportId { get; set; }
    public Guid TypeId { get; set; }
    public GeometryType GeometryType { get; set; }
    public string? Title { get; set; }
    public IEnumerable<MapPoint> Points { get; set; } = []; // Liste over punkter som definerer objektet

    public string ReportedByRole { get; set; } = string.Empty;
    public Guid ReportedByUserId { get; set; }
    public ReviewStatus ReviewStatus { get; set; }

    // Sentrumspunkt for objektet 
    public class MapPoint
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
