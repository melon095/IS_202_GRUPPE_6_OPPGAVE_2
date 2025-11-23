using Microsoft.AspNetCore.Identity;

namespace Gruppe6Oppgave2.Database.Tables;

public class RoleTable : IdentityRole<Guid>
{
    public List<UserTable> Users { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
