using Gruppe6Oppgave2.Database;
using Gruppe6Oppgave2.Database.Tables;
using Microsoft.AspNetCore.Mvc;

namespace Gruppe6Oppgave2.Models.Report.Response;

// Handlinger for objektgjennomgang
public enum ObjectReviewAction
{
    Accept,
    Deny
}

// Modell for objektgjennomgang i rapporter
public class ReportObjectViewModel
{
    [FromRoute] public Guid ReportId { get; set; } // Id for rapporten som inneholder objektet
    [FromRoute] public Guid ObjectId { get; set; } // Id for objektet som skal gjennomgås

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReviewStatus? ReviewStatus { get; set; }
    public ObjectDataModel? SelectedObject { get; set; }

    public string? SuccessMessage { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool IsKartverket { get; set; }

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
        public FeedbackModel[] Feedbacks { get; set; } = [];

        public DateTime? VerifiedAt { get; set; }
        public bool IsVerified => VerifiedAt.HasValue;

        public GeometryType GeometryType { get; set; }
    }

    // Data modell for tilbakemeldinger på objekter
    public class FeedbackModel
    {
        public Guid Id { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public FeedbackType FeedbackType { get; set; }
        public Guid FeedbackById { get; set; }
        public string FeedbackByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
