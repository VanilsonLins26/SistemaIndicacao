using API.DbContext;
using API.DTOs;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AutenticacaoController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<Usuario> _userManager;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public AutenticacaoController(ITokenService tokenService, UserManager<Usuario> userManager, IConfiguration configuration, AppDbContext context)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
    }

    [HttpGet]
    public string Ok()
    {
        return "ok";
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null && await _userManager.CheckPasswordAsync(user, model.Senha))
        {

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("codigoIndicacao", user.CodigoIndicacao),
                new Claim("id", user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };



            var token = _tokenService.GerarAccessToken(authClaims, _configuration);
            var refreshToken = _tokenService.GerarRefreshToken();

            int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"], out int validity);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(validity);

            await _userManager.UpdateAsync(user);

            var usuarioInfo = new UsuarioDTO
            {
                Id = user.Id,
                Nome = user.UserName,
                Email = user.Email,
                Pontuacao = user.Pontuacao,
                CodigoIndicacao = user.CodigoIndicacao,
            };


            return Ok(new LoginResponseDTO
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiracao = token.ValidTo,
                Usuario = usuarioInfo
            });


        }

        return new UnauthorizedResult();
    }

    [HttpPost("registrar/{codigoIndicacao?}")]
    public async Task<IActionResult> RegistrarAsync([FromBody] RegistrarDTO model, string? codigoIndicacao)
    {
        var userExists = await _userManager.FindByNameAsync(model.Nome);
        if (userExists != null)
        {
            return new ObjectResult(new ResponseDTO { Status = "Error", Erro = "User already exists!" })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        var user = new Usuario
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Nome,
            CodigoIndicacao = await GerarCodigoUnicoAsync()

        };

        var result = await _userManager.CreateAsync(user, model.Senha);
        if (!result.Succeeded)
        {
            return new ObjectResult(new ResponseDTO { Status = "Erro", Erro = "Falha ao criar usuario." })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        if(codigoIndicacao is not null)
        {
            Usuario usuario = await BuscarUsarioIndicadorAsync(codigoIndicacao);
            if(usuario is not null)
            {
                usuario.Pontuacao++;
                var resultPontuacao = await _userManager.UpdateAsync(usuario);
                if (!result.Succeeded)
                {
                    return new ObjectResult(new ResponseDTO { Status = "Erro", Erro = "Falha ao atualizar a pontução do usuario indicador." })
                    {
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                }
            }
        }

        return new OkObjectResult(new ResponseDTO { Status = "Sucesso", Erro = "Usuario criado!" });
    }

    [HttpPost("refreshtoken")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] TokenDTO tokenModel)
    {
        if (tokenModel == null) return new BadRequestObjectResult("Invalid client request");

        var principal = _tokenService.ObterDoTokenExpirado(tokenModel.AccessToken, _configuration);
        if (principal == null) return new BadRequestObjectResult("Invalid access token/refresh token");

        var username = principal.Identity?.Name;
        var user = await _userManager.FindByNameAsync(username);

        if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return new BadRequestObjectResult("Invalid access token/refresh token");
        }

        var newAccessToken = _tokenService.GerarAccessToken(principal.Claims.ToList(), _configuration);
        var newRefreshToken = _tokenService.GerarRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new OkObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken
        });
    }

    private async Task<string> GerarCodigoUnicoAsync()
    {
        string codigo;

        do
            codigo = Guid.NewGuid().ToString("N")[..8].ToUpper();
        while (await _context.Users.AnyAsync(u => u.CodigoIndicacao == codigo));

        return codigo;
    }

    private async Task<Usuario?> BuscarUsarioIndicadorAsync(string codigo)
    {
        Usuario usuario = await _context.Users.FirstOrDefaultAsync(u => u.CodigoIndicacao == codigo);

        if(usuario is not null)
        {
            return usuario;
        }

        return null;
    }
}
