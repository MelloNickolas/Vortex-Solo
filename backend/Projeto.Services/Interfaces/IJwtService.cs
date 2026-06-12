using Projeto.Domain.Entities;

namespace Projeto.Services.Interfaces;

public interface IJwtService
{
    // Gera um token JWT assinado com as claims do usuário.
    // Validade: 8 horas a partir do momento da geração.
    string GerarToken(Usuario usuario);
}
