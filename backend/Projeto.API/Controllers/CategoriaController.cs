using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Application.DTOs.Categorias.Request;
using Projeto.Application.Interfaces;

namespace Projeto.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaApplication _categoriaApplication;

    public CategoriaController(ICategoriaApplication categoriaApplication)
    {
        _categoriaApplication = categoriaApplication;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var response = await _categoriaApplication.ListarAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var response = await _categoriaApplication.BuscarPorIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CategoriaRequest request)
    {
        var response = await _categoriaApplication.CriarAsync(request);
        return Created(string.Empty, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] CategoriaRequest request)
    {
        var response = await _categoriaApplication.AtualizarAsync(id, request);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(int id)
    {
        await _categoriaApplication.DeletarAsync(id);
        return NoContent();
    }
}
