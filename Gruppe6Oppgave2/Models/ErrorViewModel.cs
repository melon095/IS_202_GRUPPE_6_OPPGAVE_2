namespace Gruppe6Oppgave2.Models;

// Modell for feilvisningssiden
public class ErrorViewModel
{
    public string? RequestId { get; set; } // Unik ID for forespørselen

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId); // Indikerer om RequestId skal vises
}
