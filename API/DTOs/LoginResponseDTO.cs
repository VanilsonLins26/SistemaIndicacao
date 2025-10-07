using System.Reflection.Metadata.Ecma335;

namespace API.DTOs;

public class LoginResponseDTO
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime Expiracao { get; set; }
    public UsuarioDTO? Usuario { get; set; }
}
