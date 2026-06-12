using Projeto.Services.Interfaces;

namespace Projeto.Services.Auth;

public class BCryptService : IBCryptService
{
    public string HashSenha(string senha)
    {
        // WorkFactor 12 — bom equilíbrio entre segurança e performance
        return BCrypt.Net.BCrypt.HashPassword(senha, workFactor: 12);


        /*
        Real: Pensa assim — você quer que gerar um hash seja lento de propósito.
        Por quê? Se um hacker roubar o banco de dados e tentar descobrir as senhas por força bruta:

        workFactor 10 → ~100ms por tentativa
        workFactor 12 → ~400ms por tentativa  ← nosso caso
        workFactor 14 → ~1.600ms por tentativa
        Com workFactor 12, testar 1 milhão de senhas levaria ~46 dias. Com workFactor 10 levaria só ~3 dias.
        */
    }

    public bool VerificarSenha(string senha, string hash)
    {
        // Compara a senha em texto puro com o hash armazenado no banco
        return BCrypt.Net.BCrypt.Verify(senha, hash);
    }
}
