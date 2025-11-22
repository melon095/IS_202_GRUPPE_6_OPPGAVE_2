using Gruppe6Oppgave2.Database;
using Gruppe6Oppgave2.Database.Tables;

namespace Gruppe6Oppgave2.Models.Report.Response;

// Modell for objektgjennomgang i rapporter
public enum ObjectReviewActionOld 
{
    Accept,
    Deny
}

// Modell for objektgjennomgang i rapporter
public class ObjectReviewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }

    public string Description { get; set; }
    public ReviewStatus? ReviewStatus { get; set; }
    public ObjectDataModel? SelectedObject { get; set; }

    public string? SuccsessMessage { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<ObjectDataModel> Objects { get; set; } = [];

    // Data modell for et enkelt objekt i rapporten
    public class ObjectDataModel
    {
        public Guid Id { get; set; }
        public Point CentroidPoint { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ReviewStatus ObjectStatus { get; set; }
        public List<Point> Points { get; set; } = [];
        public List<FeedBackModel> Feedbacks { get; set; } = []; // Liste over tilbakemeldinger for objektet

        public DateTime? VerifiedAt { get; set; }
        public bool IsVerified => VerifiedAt.HasValue;
    }

    // Data modell for tilbakemeldinger på objekter
    public class FeedBackModel
    {
        public Guid Id { get; set; }
        public string Feedback { get; set; }
        public FeedbackType FeedbackType { get; set; }
        public Guid FeedbackById { get; set; }
        public string FeedbackByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
