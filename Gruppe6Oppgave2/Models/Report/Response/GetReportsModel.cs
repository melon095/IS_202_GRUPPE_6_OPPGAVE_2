using Gruppe6Oppgave2.Database.Tables;

namespace Gruppe6Oppgave2.Models.Report.Response;

public class GetReportsModel
{
    // Liste over rapporter
    public List<ReportTable> Reports { get; set; } = [];
}