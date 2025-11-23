namespace Gruppe6Oppgave2.Models;

/// <summary>
///     ViewModel for feilside
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
