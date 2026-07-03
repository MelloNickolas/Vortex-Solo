using Microsoft.EntityFrameworkCore;
using Projeto.Repository.FuncoesSQL;
using Projeto.Repository.Context;
using Projeto.Repository.Interfaces;

namespace Projeto.Repository.Repositories;

public class RelatorioRepository : IRelatorioRepository
{
    private readonly AppDbContext _context;

    public RelatorioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResumoVendasResponse?> BuscarResumoAsync()
    {
        return await _context.Database
            .SqlQuery<ResumoVendasResponse>($"SELECT * FROM vw_ResumoVendas")
            .FirstOrDefaultAsync();
    }
    /*
    CREATE VIEW vw_ResumoVendas AS
    SELECT
        COUNT(*) AS TotalVendas,
        ISNULL(SUM(CASE WHEN Status = 2 THEN ValorTotal END), 0) AS ValorTotalGeral,
        ISNULL(AVG(CASE WHEN Status = 2 THEN ValorTotal END), 0) AS TicketMedio,
        SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END)         AS VendasConcluidas,
        SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END)         AS VendasCanceladas
    FROM Vendas;
    GO    

    os case meio q ele fala assim, se o status for 2 voce soma no acumulador valortotal s nao voce soma 0, null nao daria certo ne 
    e as vendas concluidas e canceladas vamos fazer a msm logica para trazer
    */







    public async Task<List<ProdutoAbaixoMinimoResponse>> ListarProdutosAbaixoMinimoAsync()
    {
        return await _context.Database
            .SqlQuery<ProdutoAbaixoMinimoResponse>($"SELECT * FROM vw_ProdutosAbaixoMinimo ORDER BY Deficit DESC")
            .ToListAsync();
    }
    /*
    CREATE OR ALTER VIEW vw_ProdutosAbaixoMinimo AS
    SELECT
        p.ID,
        p.Nome,
        c.Nome AS Categoria,
        p.EstoqueAtual,
        p.EstoqueMinimo,
        p.EstoqueMinimo - p.EstoqueAtual    AS Deficit
    FROM Produtos p
    INNER JOIN Categorias c ON c.ID = p.CategoriaID  ----- AQUI LIGAMOS E CARREGAMOS JUNTO ==== INCLUDE
    WHERE p.EstoqueAtual < p.EstoqueMinimo;
    GO
    */






    public async Task<List<VendaPorFormaPagamentoResponse>> ListarVendasPorFormaPagamentoAsync()
    {
        return await _context.Database
            .SqlQuery<VendaPorFormaPagamentoResponse>($"SELECT * FROM vw_VendasPorFormaPagamento ORDER BY Total DESC")
            .ToListAsync();
    }
    /*
    CREATE OR ALTER VIEW vw_VendasPorFormaPagamento AS
    SELECT
        CASE FormaPagamento   ----------------------     Aqui so agrupamos para mostrar
            WHEN 1 THEN 'Dinheiro'
            WHEN 2 THEN 'Cartão de Débito'
            WHEN 3 THEN 'Cartão de Crédito'
            WHEN 4 THEN 'Pix'
            WHEN 5 THEN 'Boleto'
        END AS FormaPagamentoNome,
        COUNT(*) AS Quantidade,
        ISNULL(SUM(ValorTotal), 0)  AS Total
    FROM Vendas
    WHERE Status = 2  -------------- Só as vendas concluidas
    GROUP BY FormaPagamento;
    GO
    */






    public async Task<List<ProdutoMaisVendidoResponse>> ListarProdutosMaisVendidosAsync(int top)
    {
        return await _context.Database
            .SqlQuery<ProdutoMaisVendidoResponse>($"EXEC sp_ProdutosMaisVendidos @Top = {top}")
            .ToListAsync();
    }
    /*
    CREATE OR ALTER PROCEDURE sp_ProdutosMaisVendidos
    @Top INT = 10   -------   aqui passamos o parâmetro
    AS
    BEGIN   --------------------- tudo dentro desse BEGIN e PROCEDURE é o nosso proc
        SELECT TOP (@Top)
            p.Nome AS Produto,
            c.Nome AS Categoria,
            SUM(iv.Quantidade) AS TotalVendido,
            SUM(iv.Subtotal) AS TotalFaturado
        FROM ItensVenda iv
        INNER JOIN Produtos   p ON p.ID = iv.ProdutoID
        INNER JOIN Categorias c ON c.ID = p.CategoriaID
        INNER JOIN Vendas     v ON v.ID = iv.VendaID
        WHERE v.Status = 2
        GROUP BY p.Nome, c.Nome
        ORDER BY TotalVendido DESC;
    END;
    GO
    */








    public async Task<decimal> ConsultarTotalClienteAsync(int clienteId)
    {
        var result = await _context.Database
            .SqlQuery<TotalClienteResponse>($"SELECT dbo.fn_TotalVendasCliente({clienteId}) AS Total")
            .FirstOrDefaultAsync();

        return result?.Total ?? 0;
    }
    /*
    CREATE OR ALTER FUNCTION dbo.fn_TotalVendasCliente
    (
    @ClienteID INT   ---- o paramtro que estou passando
    )
    RETURNS DECIMAL(18, 2)     - aqui declaramos o tipo de valor que eu vou desenvolver
    AS
    BEGIN
        DECLARE @Total DECIMAL(18, 2);

        SELECT @Total = ISNULL(SUM(ValorTotal), 0)
        FROM Vendas
        WHERE ClienteID = @ClienteID
        AND Status = 2;

        RETURN @Total;
    END;
    GO

    */
}
