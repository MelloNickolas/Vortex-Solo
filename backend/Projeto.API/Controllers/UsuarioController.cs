using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Application.DTOs.Usuarios.Request;
using Projeto.Application.Interfaces;

namespace Projeto.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioApplication _usuarioApplication;

    public UsuarioController(IUsuarioApplication usuarioApplication)
    {
        _usuarioApplication = usuarioApplication;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var response = await _usuarioApplication.ListarAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var response = await _usuarioApplication.BuscarPorIdAsync(id);
        return Ok(response);
    }

    // qualquer pessoa pode se cadastrar
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] UsuarioRequest request)
    {
        var response = await _usuarioApplication.CriarAsync(request);
        return Created(string.Empty, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UsuarioRequest request)
    {
        var response = await _usuarioApplication.AtualizarAsync(id, request);
        return Ok(response);
    }

    [HttpPut("{id}/senha")]
    public async Task<IActionResult> AlterarSenha(int id, [FromBody] AlterarSenhaRequest request)
    {
        await _usuarioApplication.AlterarSenhaAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(int id)
    {
        await _usuarioApplication.DeletarAsync(id);
        return NoContent();
    }
}
