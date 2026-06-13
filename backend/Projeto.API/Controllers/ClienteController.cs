using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Application.DTOs.Clientes.Request;
using Projeto.Application.Interfaces;

namespace Projeto.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IClienteApplication _clienteApplication;

    public ClienteController(IClienteApplication clienteApplication)
    {
        _clienteApplication = clienteApplication;
    }

    // GET /api/cliente?page=1&pageSize=10&busca=joao&ativo=true
    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? busca = null,
        [FromQuery] bool? ativo = null)
    {
        var response = await _clienteApplication.ListarPagedAsync(page, pageSize, busca, ativo);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var response = await _clienteApplication.BuscarPorIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ClienteRequest request)
    {
        var response = await _clienteApplication.CriarAsync(request);
        return Created(string.Empty, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] ClienteRequest request)
    {
        var response = await _clienteApplication.AtualizarAsync(id, request);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(int id)
    {
        await _clienteApplication.DeletarAsync(id);
        return NoContent();
    }
}
