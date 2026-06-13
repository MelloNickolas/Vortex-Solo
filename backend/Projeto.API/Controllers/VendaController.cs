using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Application.DTOs.Vendas.Request;
using Projeto.Application.Interfaces;

namespace Projeto.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class VendaController : ControllerBase
{
    private readonly IVendaApplication _vendaApplication;

    public VendaController(IVendaApplication vendaApplication)
    {
        _vendaApplication = vendaApplication;
    }

    // GET /api/venda?page=1&pageSize=10&status=Concluida&de=2026-01-01&ate=2026-12-31
    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? de = null,
        [FromQuery] DateTime? ate = null)
    {
        var response = await _vendaApplication.ListarPagedAsync(page, pageSize, status, de, ate);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var response = await _vendaApplication.BuscarPorIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] VendaRequest request)
    {
        var response = await _vendaApplication.CriarAsync(request);
        return Created(string.Empty, response);
    }

    // PATCH, atualização parcial, só muda o status
    [HttpPatch("{id}/cancelar")]
    public async Task<IActionResult> Cancelar(int id, [FromQuery] int usuarioId)
    {
        var response = await _vendaApplication.CancelarAsync(id, usuarioId);
        return Ok(response);
    }
}
