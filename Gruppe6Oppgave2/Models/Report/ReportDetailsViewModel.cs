using Gruppe6Oppgave2.Database;
using Gruppe6Oppgave2.Database.Tables;
using Gruppe6Oppgave2.Models.Report.Response;
using Microsoft.AspNetCore.Mvc;

namespace Gruppe6Oppgave2.Models.Report;

// Modell for detaljvisning av en rapport
public class ReportDetailsViewModel
{
    [FromRoute] public Guid ReportId { get; set; } // Id for rapporten
    [FromRoute] public Guid? ObjectId { get; set; } // Valgfri Id for et spesifikt objekt i rapporten

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public ObjectDataModel? SelectedObject { get; set; }

    public List<ObjectDataModel> Objects { get; set; } = [];

    // Data modell for et enkelt objekt i rapporten
    public class ObjectDataModel : IMapObject
    {
        public Guid Id { get; set; }
        public Guid TypeId { get; set; }
        public Point? CentroidPoint { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public ReviewStatus ObjectStatus { get; set; }
        public Point[] Points { get; set; } = [];
        public GeometryType GeometryType { get; set; }

        public DateTime? VerifiedAt { get; set; }
        public bool IsVerified => VerifiedAt.HasValue;
    }
}
