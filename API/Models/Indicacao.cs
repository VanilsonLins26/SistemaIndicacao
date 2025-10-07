using Microsoft.AspNetCore.Identity;

namespace API.Models;

public class Indicacao
{
    public int Id { get; set; }
    public DateTime DataIndicacao { get; set; }
    public Usuario? Indicador { get; set; }
    public Usuario? Indicado { get; set; }

}
