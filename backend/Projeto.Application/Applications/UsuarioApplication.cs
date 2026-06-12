using Projeto.Application.DTOs.Usuarios.Request;
using Projeto.Application.DTOs.Usuarios.Response;
using Projeto.Application.Exceptions;
using Projeto.Application.Interfaces;
using Projeto.Domain.Entities;
using Projeto.Repository.Interfaces;
using Projeto.Services.Interfaces;

namespace Projeto.Application.Applications;

public class UsuarioApplication : IUsuarioApplication
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IBCryptService _bcryptService;

    public UsuarioApplication(IUsuarioRepository usuarioRepository, IBCryptService bcryptService)
    {
        _usuarioRepository = usuarioRepository;
        _bcryptService     = bcryptService;
    }

    public async Task<List<UsuarioResponse>> ListarAsync()
    {
        try
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return usuarios.Select(u => MapearResponse(u)).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar usuários: {ex.Message}");
        }
    }

    public async Task<UsuarioResponse> BuscarPorIdAsync(int id)
    {
        try
        {
            var usuario = await ValidarUsuarioExistente(id);
            return MapearResponse(usuario);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar usuário: {ex.Message}");
        }
    }

    public async Task<UsuarioResponse> CriarAsync(UsuarioRequest request)
    {
        try
        {
            ValidarRequest(request);
            await ValidarEmailDisponivel(request.Email);

            var usuario = new Usuario
            {
                Nome      = request.Nome.Trim(),
                Email     = request.Email.Trim().ToLower(),
                Telefone  = request.Telefone.Trim(),
                // A senha vem em texto puro e é hasheada antes de salvar
                SenhaHash = _bcryptService.HashSenha(request.Senha),
                CriadoEm = DateTime.UtcNow,
                Ativo     = true
            };

            await _usuarioRepository.AddAsync(usuario);
            return MapearResponse(usuario);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao criar usuário: {ex.Message}");
        }
    }

    public async Task<UsuarioResponse> AtualizarAsync(int id, UsuarioRequest request)
    {
        try
        {
            ValidarRequest(request, ignorarSenha: true);

            var usuario = await ValidarUsuarioExistente(id);

            // Se o email mudou, valida se o novo email já está em uso
            if (!usuario.Email.Equals(request.Email.Trim().ToLower()))
                await ValidarEmailDisponivel(request.Email);

            usuario.Nome     = request.Nome.Trim();
            usuario.Email    = request.Email.Trim().ToLower();
            usuario.Telefone = request.Telefone.Trim();

            await _usuarioRepository.UpdateAsync(usuario);
            return MapearResponse(usuario);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao atualizar usuário: {ex.Message}");
        }
    }

    public async Task AlterarSenhaAsync(int id, AlterarSenhaRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.SenhaAtual))
                throw new ValidationException("Senha atual não pode ser vazia.");

            if (string.IsNullOrWhiteSpace(request.NovaSenha))
                throw new ValidationException("Nova senha não pode ser vazia.");

            if (request.NovaSenha.Length < 6)
                throw new ValidationException("Nova senha deve ter pelo menos 6 caracteres.");

            var usuario = await ValidarUsuarioExistente(id);

            // Verifica se a senha atual está correta antes de permitir a troca
            if (!_bcryptService.VerificarSenha(request.SenhaAtual, usuario.SenhaHash))
                throw new ValidationException("Senha atual incorreta.");

            usuario.SenhaHash = _bcryptService.HashSenha(request.NovaSenha);
            await _usuarioRepository.UpdateAsync(usuario);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao alterar senha: {ex.Message}");
        }
    }

    public async Task DeletarAsync(int id)
    {
        try
        {
            var usuario = await ValidarUsuarioExistente(id);
            await _usuarioRepository.DeleteAsync(usuario);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao deletar usuário: {ex.Message}");
        }
    }

    #region Úteis

    private static void ValidarRequest(UsuarioRequest request, bool ignorarSenha = false)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new ValidationException("Nome não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("E-mail não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(request.Telefone))
            throw new ValidationException("Telefone não pode ser vazio.");

        // Aqui so vai entrar no UPDATE, pq sempre por padrão vai ser FALSE, entao na hora do update so setar como TRUE ai ele valida senha
        if (!ignorarSenha)
        {
            if (string.IsNullOrWhiteSpace(request.Senha))
                throw new ValidationException("Senha não pode ser vazia.");

            if (request.Senha.Length < 6)
                throw new ValidationException("Senha deve ter pelo menos 6 caracteres.");
        }
    }

    private async Task ValidarEmailDisponivel(string email)
    {
        var existe = await _usuarioRepository.GetByEmailAsync(email.Trim().ToLower());
        if (existe != null)
            throw new ValidationException("Já existe um usuário com esse e-mail.");
    }

    private async Task<Usuario> ValidarUsuarioExistente(int id)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        if (usuario == null)
            throw new NotFoundException("Usuário não encontrado.");
        return usuario;
    }

    // Converte entidade em DTO sem expor SenhaHash, bom para segurança de dados
    private static UsuarioResponse MapearResponse(Usuario usuario) => new()
    {
        ID        = usuario.ID,
        Nome      = usuario.Nome,
        Email     = usuario.Email,
        Telefone  = usuario.Telefone,
        CriadoEm = usuario.CriadoEm,
        Ativo     = usuario.Ativo
    };

    #endregion
}
