using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Application.DTOs.Produtos.Request;
using Projeto.Application.Interfaces;

namespace Projeto.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly IProdutoApplication _produtoApplication;

    public ProdutoController(IProdutoApplication produtoApplication)
    {
        _produtoApplication = produtoApplication;
    }

    // GET /api/produto?page=1&pageSize=10&nome=notebook&categoriaId=2
    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] int page = 1, //  se  nao vier na url usa 1
        [FromQuery] int pageSize = 10, //  se nao vier na url usa 10
        [FromQuery] string? nome = null,
        [FromQuery] int? categoriaId = null)
    {
        var response = await _produtoApplication.ListarPagedAsync(page, pageSize, nome, categoriaId);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var response = await _produtoApplication.BuscarPorIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ProdutoRequest request)
    {
        var response = await _produtoApplication.CriarAsync(request);
        return Created(string.Empty, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] ProdutoRequest request)
    {
        var response = await _produtoApplication.AtualizarAsync(id, request);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(int id)
    {
        await _produtoApplication.DeletarAsync(id);
        return NoContent();
    }
}
