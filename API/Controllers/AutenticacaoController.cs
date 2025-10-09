using API.DbContext;
using API.DTOs;
using API.Models;
using API.Repository;
using API.Services;
using Microsoft.AspNetCore.Authorization;
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
    private readonly IUsuarioRepository _usuarioRepository;

    public AutenticacaoController(ITokenService tokenService, UserManager<Usuario> userManager, IConfiguration configuration, IUsuarioRepository usuarioRepository)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _configuration = configuration;
        _usuarioRepository = usuarioRepository;
    }



    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null && await _userManager.CheckPasswordAsync(user, model.Senha))
        {



            var token = _tokenService.GerarAccessToken(user, _configuration);
            var refreshToken = _tokenService.GerarRefreshToken();

            int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"], out int validity);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(validity);

            await _userManager.UpdateAsync(user);

            var usuarioInfo = new UsuarioDTO
            {
                Id = user.Id,
                Nome = user.NomeCompleto,
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

    [HttpPost("registrar")]
    public async Task<IActionResult> RegistrarAsync([FromBody] RegistrarDTO model)
    {
        var userExists = await _userManager.FindByEmailAsync(model.Email);
        if (userExists != null)
        {
            return Conflict(new ResponseDTO { Status = "Erro", Erro = "Este email já está cadastrado!" });
        }

        var user = new Usuario
        {
            NomeCompleto = model.Nome,
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Email,
            CodigoIndicacao = await _tokenService.GerarCodigoUnicoAsync()
        };

        var token = _tokenService.GerarAccessToken(user, _configuration);
        var refreshToken = _tokenService.GerarRefreshToken();

        int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"], out int validity);
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(validity);


        var result = await _userManager.CreateAsync(user, model.Senha);
        if (!result.Succeeded)
        {
            return new ObjectResult(new ResponseDTO { Status = "Erro", Erro = "Falha ao criar usuario." })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        if (!string.IsNullOrEmpty(model.CodigoIndicacao))
        {
            var usuarioIndicador = await _usuarioRepository.BuscarUsarioIndicadorAsync(model.CodigoIndicacao);
            if(usuarioIndicador is not null)
            {
                usuarioIndicador.Pontuacao++;
                var resultPontuacao = await _userManager.UpdateAsync(usuarioIndicador);

            }
        }

        var usuarioInfo = new UsuarioDTO
        {
            Id = user.Id,
            Nome = user.NomeCompleto,
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

    [HttpGet("perfil")]
    [Authorize]
    public async Task<IActionResult> ObterInfoUsuarios()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (usuarioId is null)
            return Unauthorized();

        var usuario = await _userManager.FindByIdAsync(usuarioId);

        if (usuario is null)
            return NotFound("Usuario não encontrado, faça o login novamente");

        var usuarioDto = new UsuarioDTO
        {
            Id = usuario.Id,
            Nome = usuario.NomeCompleto,
            Email = usuario.Email,
            CodigoIndicacao = usuario.CodigoIndicacao,
            Pontuacao = usuario.Pontuacao
        };
        return Ok(usuarioDto);

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

        var newAccessToken = _tokenService.GerarAccessToken(user, _configuration);
        var newRefreshToken = _tokenService.GerarRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new OkObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken
        });
    }

}
