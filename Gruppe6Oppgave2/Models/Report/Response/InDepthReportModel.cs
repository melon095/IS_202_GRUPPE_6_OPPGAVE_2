using Gruppe6Oppgave2.Database;

namespace Gruppe6Oppgave2.Models.Report.Response
{
    // Modell for detaljert rapportvisning
    public partial class InDepthReportModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public ObjectDataModel? SelectedObject { get; set; }

        public List<ObjectDataModel> Objects { get; set; } = []; // Liste over objekter i rapporten

        // Data modell for et enkelt objekt i rapporten
        public partial class ObjectDataModel
        {
            public Guid Id { get; set; }
            public Point CentroidPoint { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public ReviewStatus ObjectStatus { get; set; } // Gjennomgangsstatus for objektet

            public List<Point> Points { get; set; } = []; // Liste over punkter som definerer objektet

            public DateTime? VerifiedAt { get; set; } // Tidspunkt for når objektet ble verifisert
            public bool IsVerified => VerifiedAt.HasValue; // Indikerer om objektet er verifisert

        }
    }
}
