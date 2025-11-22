using Gruppe6Oppgave2.Database.Tables;

namespace Gruppe6Oppgave2.Models.Map.Request;

// Definerer geometrityper for reiseobjekter
public class FinalizeJourneyObject
{
    // Unik identifikator for objektet
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public bool Deleted { get; set; } = false; 
    public List<FinalizeJourneyPointDataModel> Points { get; set; }
    public Guid? TypeId { get; set; } = null;
    public GeometryType GeometryType { get; set; }
}

// Data modell for å fullføre en reise
public class FinalizeJourneyDataModel
{
    public string Title { get; set; } = "Ny tur";
    public string Description { get; set; } = "";
}

// Data modell for punkter i en reise
public class FinalizeJourneyPointDataModel
{
    public double Lat { get; set; }
    public double Lng { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Data modell for forespørsel om å fullføre en reise
public class FinalizeJourneyRequest
{
    public List<FinalizeJourneyObject> Objects { get; set; } = [];
    public FinalizeJourneyDataModel Journey { get; set; }
}
