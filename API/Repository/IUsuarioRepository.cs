using API.Models;

namespace API.Repository
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> BuscarUsarioIndicadorAsync(string codigo);
        Task<bool> CodigoIndicacaoExistsAsync(string codigo);
    }
}
