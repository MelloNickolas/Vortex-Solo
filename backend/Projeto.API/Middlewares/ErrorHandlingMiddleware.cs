using System.Net;
using System.Text.Json;
using Projeto.Application.Exceptions;

namespace Projeto.API.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    // RequestDelegate é o tipo que representa o próximo passo no pipeline HTTP. Pensa como uma fila,
    // cada middleware passa a requisição pro próximo. O _next é quem está na frente na fila.
    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /*
    Esse é o método que o ASP.NET chama para cada requisição que chega. 
    O HttpContext carrega tudo sobre aquela requisição headers, body, a resposta que vai sair, cookies, etc.
    */
    public async Task Invoke(HttpContext context)
    {
         try
        {
            await _next(context); // se der certo, tudo segue normal
        }
        catch (Exception ex)
        {
            await TratarExcecao(context, ex); // se qualquer coisa explodir, intercepta
        }
    }

    private static async Task TratarExcecao(HttpContext context, Exception ex)
    {
        var statusCode = ex switch
        {
            NotFoundException      => HttpStatusCode.NotFound,            // 404
            ValidationException    => HttpStatusCode.BadRequest,          // 400
            UnauthorizedException  => HttpStatusCode.Unauthorized,        // 401
            _                      => HttpStatusCode.InternalServerError  // 500
        };

        var resposta = new
        {
            sucesso  = false,
            mensagem = ex.Message
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(resposta));
    }
}
