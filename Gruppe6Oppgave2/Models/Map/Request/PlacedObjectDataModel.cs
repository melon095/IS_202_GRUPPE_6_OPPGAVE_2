using Gruppe6Oppgave2.Database.Tables;

namespace Gruppe6Oppgave2.Models.Map.Request;

// Data modell for et plassert objekt på kartet
public class PlacedObjectDataModel
{
    public List<PlacedPointDataModel> Points { get; set; } = []; // Liste over punkter som definerer objektet
    public Guid? TypeId { get; set; } = null;
    public GeometryType GeometryType { get; set; }
    public DateTime CreatedAt { get; set; }
}
