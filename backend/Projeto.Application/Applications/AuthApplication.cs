using Projeto.Application.DTOs.Auth.Request;
using Projeto.Application.DTOs.Auth.Response;
using Projeto.Application.Exceptions;
using Projeto.Application.Interfaces;
using Projeto.Repository.Interfaces;
using Projeto.Services.Interfaces;

namespace Projeto.Application.Applications;

public class AuthApplication : IAuthApplication
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IBCryptService _bcryptService;
    private readonly IJwtService _jwtService;

    public AuthApplication(
        IUsuarioRepository usuarioRepository,
        IBCryptService bcryptService,
        IJwtService jwtService)
    {
        _usuarioRepository = usuarioRepository;
        _bcryptService     = bcryptService;
        _jwtService        = jwtService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ValidationException("E-mail não pode ser vazio.");

            if (string.IsNullOrWhiteSpace(request.Senha))
                throw new ValidationException("Senha não pode ser vazia.");

            // Busca o usuário pelo email, retorna null se não existir
            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email.Trim().ToLower());

            // Mensagem genérica proposital, não informamos se foi email ou senha errada
            // Isso evita que alguém descubra quais emails estão cadastrados
            if (usuario == null)
                throw new UnauthorizedException("E-mail ou senha inválidos.");

            if (!usuario.Ativo)
                throw new UnauthorizedException("Usuário inativo. Entre em contato com o administrador.");

            if (!_bcryptService.VerificarSenha(request.Senha, usuario.SenhaHash))
                throw new UnauthorizedException("E-mail ou senha inválidos.");

            var token = _jwtService.GerarToken(usuario);

            return new LoginResponse
            {
                Token     = token,
                Nome      = usuario.Nome,
                Email     = usuario.Email,
                // Deve ser igual ao AddHours(8) configurado no JwtService
                Expiracao = DateTime.UtcNow.AddHours(8)
            };
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (UnauthorizedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao realizar login: {ex.Message}");
        }
    }
}
