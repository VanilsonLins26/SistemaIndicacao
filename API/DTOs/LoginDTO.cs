using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class LoginDTO
{
    [EmailAddress]
    [Required(ErrorMessage = "O email é necessário!!")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "A senha é necessária!!")]
    public string? Senha { get; set; }

}
