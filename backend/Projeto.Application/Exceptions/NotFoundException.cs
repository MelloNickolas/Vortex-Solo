namespace Projeto.Application.Exceptions;

// Lançada quando um registro não é encontrado no banco
// Ex buscar um produto com ID 99 que não existe
// O middleware converte isso em HTTP 404 Not Found
public class NotFoundException : Exception
{
    public NotFoundException(string mensagem) : base(mensagem) { }
}
