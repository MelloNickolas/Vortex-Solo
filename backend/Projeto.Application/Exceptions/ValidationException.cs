namespace Projeto.Application.Exceptions;

// Lançada quando os dados enviados são inválidos ou violam uma regra de negócio
// Exe tentar cadastrar um produto com preço negativo, ou nome em branco
// O middleware converte isso em HTTP 400 Bad Request
public class ValidationException : Exception
{
    public ValidationException(string mensagem) : base(mensagem) { }
}
