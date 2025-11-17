using Gruppe6Oppgave2.Database;
using Gruppe6Oppgave2.Database.Tables;
using Gruppe6Oppgave2.Database;

namespace Gruppe6Oppgave2.Models.Report.Response;

public class ReportReviewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }

    public string Description { get; set; }
    public ReviewStatus ReviewStatus { get; set; }
    public ObjectDataModel? SelectedObject { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<ObjectDataModel> Objects { get; set; } = [];


    public class ObjectDataModel
    {
        public Guid Id { get; set; }
        public Point CentroidPoint { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ReviewStatus ObjectStatus { get; set; }
        public List<Point> Points { get; set; } = [];

    }

    public class FeedBackModel
    {
        public ReviewStatus ReviewStatus { get; set; }
        public FeedbackType FeedBack { get; set; }
    }
}
