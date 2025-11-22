namespace Gruppe6Oppgave2.Models.Map.Request;

// Data modell for et plassert punkt på kartet
public class PlacedPointDataModel
{
    public double Lat { get; set; }
    public double Lng { get; set; }
    public string Label { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
