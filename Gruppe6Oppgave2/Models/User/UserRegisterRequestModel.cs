using System.ComponentModel.DataAnnotations;

namespace Gruppe6Oppgave2.Models.User.Request;

public class UserRegisterRequestModel
{
    [Required]
    public string Username { get; set; } = null!;
    
    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    
    [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passordene er ikke like")]
    public string ConfirmPassword { get; set; } = null!;
}
