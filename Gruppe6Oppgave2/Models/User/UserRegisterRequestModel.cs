using System.ComponentModel.DataAnnotations;

namespace Gruppe6Oppgave2.Models.User.Request;

// Data modell for brukerregistrering
public class UserRegisterRequestModel
{
    [Required] public string Username { get; set; } = null!; // Brukernavn

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!; // Passord

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passordene er ikke like")]
    public string ConfirmPassword { get; set; } = null!; // Bekreft passord
}
