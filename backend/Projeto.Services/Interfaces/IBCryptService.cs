namespace Projeto.Services.Interfaces;

public interface IBCryptService
{
    // Gera o hash BCrypt de uma senha em texto puro.
    string HashSenha(string senha);

    // Verifica se a senha em texto puro corresponde ao hash armazenado.
    bool VerificarSenha(string senha, string hash);
}
