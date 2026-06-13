using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Application.Interfaces;

namespace Projeto.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CidadeController : ControllerBase
{
    private readonly ICidadeApplication _cidadeApplication;

    public CidadeController(ICidadeApplication cidadeApplication)
    {
        _cidadeApplication = cidadeApplication;
    }

    // GET /api/cidade/estado/5  retorna todas as cidades do estado 5
    [HttpGet("estado/{estadoId}")]
    public async Task<IActionResult> ListarPorEstado(int estadoId)
    {
        var response = await _cidadeApplication.ListarPorEstadoAsync(estadoId);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var response = await _cidadeApplication.BuscarPorIdAsync(id);
        return Ok(response);
    }
}
