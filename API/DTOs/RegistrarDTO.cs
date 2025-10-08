using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegistrarDTO
{
    [Required(ErrorMessage = "O usuario é necessário!!")]
    public string? Nome { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "O email é necessário!!")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "A senha é necessária!!")]
    public string? Senha { get; set; }

    public string? CodigoIndicacao { get; set; }
}
