using Microsoft.AspNetCore.Identity;

namespace API.Models;

public class Usuario : IdentityUser
{
    public string? NomeCompleto { get; set; }
    public int Pontuacao { get; set; }
    public string? CodigoIndicacao { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}
