using Gruppe6Oppgave2.Database;
using Microsoft.AspNetCore.Mvc;

namespace Gruppe6Oppgave2.Models.Report;

// Sorteringsrekkefølge for rapporter
public enum SortOrder
{
    Ascending,
    Descending
}

// Visningsmodell for rapportindekssiden
public class ReportIndexViewModel
{
    [FromQuery] public int Page { get; set; } = 1; // Gjeldende side for paginering
    [FromQuery] public DateOnly SortDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow); // Filtrering basert på dato
    [FromQuery] public ReviewStatus? SortStatus { get; set; } = null; // Filtrering basert på gjennomgangsstatus
    [FromQuery] public SortOrder SortOrder { get; set; } = SortOrder.Descending; // Sorteringsrekkefølge

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public List<Report> Reports { get; set; } = [];

    // Modell for en enkelt rapport i rapportlisten
    public class Report
    {
        public Guid Id { get; set; }
        public string User { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public ReviewStatus Review { get; set; }
        public int TotalObjects { get; set; }
    }
}
