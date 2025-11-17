using System.ComponentModel.DataAnnotations;

namespace nexumApp.Models;

public class ConviteAdminViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; } = null!;

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = null!;

    public Guid Token { get; set; }
}
