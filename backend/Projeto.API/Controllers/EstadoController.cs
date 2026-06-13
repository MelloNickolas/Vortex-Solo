using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Application.Interfaces;

namespace Projeto.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class EstadoController : ControllerBase
{
    private readonly IEstadoApplication _estadoApplication;

    public EstadoController(IEstadoApplication estadoApplication)
    {
        _estadoApplication = estadoApplication;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var response = await _estadoApplication.ListarAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var response = await _estadoApplication.BuscarPorIdAsync(id);
        return Ok(response);
    }
}
