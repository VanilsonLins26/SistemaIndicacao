using API.DbContext;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repository;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    // Injetamos o AppDbContext aqui, para que o repositório possa acessar o banco.
    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> BuscarUsarioIndicadorAsync(string codigo)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.CodigoIndicacao == codigo);
    }

    public async Task<bool> CodigoIndicacaoExistsAsync(string codigo)
    {
        return await _context.Users.AnyAsync(u => u.CodigoIndicacao == codigo);
    }

}
