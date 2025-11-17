using System.ComponentModel.DataAnnotations;

namespace nexumApp.Models;

public class ConviteAdministrador
{
    [Key]
    public Guid Token { get; set; } = Guid.NewGuid();

    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // validade do convite (7 dias)
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);

    public bool Used { get; set; } = false;
}

