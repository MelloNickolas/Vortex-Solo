import { useState, useEffect } from 'react';
import RelatorioApi from '../../services/RelatorioApi';
import AIApi from '../../services/AIApi';
import styles from './Relatorios.module.css';

function Relatorios() {
  const [resumo, setResumo] = useState(null);
  const [maisVendidos, setMaisVendidos] = useState([]);
  const [abaixoMinimo, setAbaixoMinimo] = useState([]);
  const [formasPagamento, setFormasPagamento] = useState([]);
  const [carregando, setCarregando] = useState(true);

  const [respostaVendas, setRespostaVendas] = useState('');
  const [respostaEstoque, setRespostaEstoque] = useState('');
  const [carregandoVendasIA, setCarregandoVendasIA] = useState(false);
  const [carregandoEstoqueIA, setCarregandoEstoqueIA] = useState(false);

  useEffect(() => {
    async function carregar() {
      try {
        const [res, mv, am, fp] = await Promise.all([
          RelatorioApi.BuscarResumoAsync(),
          RelatorioApi.ListarProdutosMaisVendidosAsync(10),
          RelatorioApi.ListarProdutosAbaixoMinimoAsync(),
          RelatorioApi.ListarFormasPagamentoAsync(),
        ]);
        setResumo(res);
        setMaisVendidos(mv);
        setAbaixoMinimo(am);
        setFormasPagamento(fp);
      } catch (error) {
        console.error('Erro ao carregar relatórios:', error);
      } finally {
        setCarregando(false);
      }
    }
    carregar();
  }, []);

  async function handleAnalisarVendas() {
    setCarregandoVendasIA(true);
    setRespostaVendas('');
    try {
      const data = await AIApi.AnalisarVendasAsync();
      setRespostaVendas(data.resposta);
    } catch {
      setRespostaVendas('Erro ao consultar a IA.');
    } finally {
      setCarregandoVendasIA(false);
    }
  }

  async function handleAnalisarEstoque() {
    setCarregandoEstoqueIA(true);
    setRespostaEstoque('');
    try {
      const data = await AIApi.AnalisarEstoqueAsync();
      setRespostaEstoque(data.resposta);
    } catch {
      setRespostaEstoque('Erro ao consultar a IA.');
    } finally {
      setCarregandoEstoqueIA(false);
    }
  }

  function formatarMoeda(valor) {
    return Number(valor).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }

  if (carregando) {
    return <p className={styles.carregando}>Carregando relatórios...</p>;
  }

  return (
    <div>
      <h1 className="page-title">Relatórios</h1>

      {/* ── Cards de resumo (View: vw_ResumoVendas) ── */}
      <section className={styles.secao}>
        <h2 className={styles.secaoTitulo}>Resumo Geral — <span className={styles.fonte}>View: vw_ResumoVendas</span></h2>
        <div className={styles.cards}>
          <div className={styles.card}>
            <div className={styles.cardLabel}>Total de Vendas</div>
            <div className={styles.cardValor}>{resumo?.totalVendas ?? 0}</div>
          </div>
          <div className={styles.card}>
            <div className={styles.cardLabel}>Valor Total Faturado</div>
            <div className={styles.cardValor}>{formatarMoeda(resumo?.valorTotalGeral ?? 0)}</div>
          </div>
          <div className={styles.card}>
            <div className={styles.cardLabel}>Ticket Médio</div>
            <div className={styles.cardValor}>{formatarMoeda(resumo?.ticketMedio ?? 0)}</div>
          </div>
          <div className={styles.card}>
            <div className={styles.cardLabel}>Concluídas</div>
            <div className={`${styles.cardValor} ${styles.verde}`}>{resumo?.vendasConcluidas ?? 0}</div>
          </div>
          <div className={styles.card}>
            <div className={styles.cardLabel}>Canceladas</div>
            <div className={`${styles.cardValor} ${styles.vermelho}`}>{resumo?.vendasCanceladas ?? 0}</div>
          </div>
        </div>
      </section>

      {/* ── Produtos mais vendidos (SP: sp_ProdutosMaisVendidos) ── */}
      <section className={styles.secao}>
        <h2 className={styles.secaoTitulo}>Top 10 Produtos Mais Vendidos — <span className={styles.fonte}>SP: sp_ProdutosMaisVendidos</span></h2>
        {maisVendidos.length === 0 ? (
          <p className={styles.vazio}>Nenhum dado encontrado.</p>
        ) : (
          <table className={styles.tabela}>
            <thead>
              <tr>
                <th>#</th>
                <th>Produto</th>
                <th>Categoria</th>
                <th>Qtd. Vendida</th>
                <th>Total Faturado</th>
              </tr>
            </thead>
            <tbody>
              {maisVendidos.map((item, i) => (
                <tr key={i}>
                  <td>{i + 1}</td>
                  <td>{item.produto}</td>
                  <td>{item.categoria}</td>
                  <td>{item.totalVendido}</td>
                  <td>{formatarMoeda(item.totalFaturado)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>

      {/* ── Formas de pagamento (View: vw_VendasPorFormaPagamento) ── */}
      <section className={styles.secao}>
        <h2 className={styles.secaoTitulo}>Vendas por Forma de Pagamento — <span className={styles.fonte}>View: vw_VendasPorFormaPagamento</span></h2>
        {formasPagamento.length === 0 ? (
          <p className={styles.vazio}>Nenhum dado encontrado.</p>
        ) : (
          <table className={styles.tabela}>
            <thead>
              <tr>
                <th>Forma de Pagamento</th>
                <th>Quantidade</th>
                <th>Total</th>
              </tr>
            </thead>
            <tbody>
              {formasPagamento.map((item, i) => (
                <tr key={i}>
                  <td>{item.formaPagamentoNome}</td>
                  <td>{item.quantidade}</td>
                  <td>{formatarMoeda(item.total)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>

      {/* ── Estoque crítico (View: vw_ProdutosAbaixoMinimo) ── */}
      <section className={styles.secao}>
        <h2 className={styles.secaoTitulo}>
          Estoque Crítico — <span className={styles.fonte}>View: vw_ProdutosAbaixoMinimo</span>
          {abaixoMinimo.length > 0 && <span className={styles.alerta}> {abaixoMinimo.length} produto(s)</span>}
        </h2>
        {abaixoMinimo.length === 0 ? (
          <p className={styles.vazio}>Nenhum produto abaixo do estoque mínimo.</p>
        ) : (
          <table className={styles.tabela}>
            <thead>
              <tr>
                <th>Produto</th>
                <th>Categoria</th>
                <th>Estoque Atual</th>
                <th>Mínimo</th>
                <th>Déficit</th>
              </tr>
            </thead>
            <tbody>
              {abaixoMinimo.map((item) => (
                <tr key={item.id}>
                  <td>{item.nome}</td>
                  <td>{item.categoria}</td>
                  <td className={styles.vermelho}>{item.estoqueAtual}</td>
                  <td>{item.estoqueMinimo}</td>
                  <td className={styles.vermelho}>{item.deficit}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>
      {/* ── Seção IA ── */}
      <section className={styles.secao}>
        <h2 className={styles.secaoTitulo}>Inteligência Artificial — <span className={styles.fonte}>GitHub Models / gpt-4.1</span></h2>

        <div className={styles.iaBloco}>
          <div className={styles.iaItem}>
            <div className={styles.iaHeader}>
              <span>Análise de Vendas</span>
              <button className={styles.iaBotao} onClick={handleAnalisarVendas} disabled={carregandoVendasIA}>
                {carregandoVendasIA ? 'Analisando...' : 'Analisar com IA'}
              </button>
            </div>
            {respostaVendas && <p className={styles.iaResposta}>{respostaVendas}</p>}
          </div>

          <div className={styles.iaItem}>
            <div className={styles.iaHeader}>
              <span>Recomendações de Estoque</span>
              <button className={styles.iaBotao} onClick={handleAnalisarEstoque} disabled={carregandoEstoqueIA}>
                {carregandoEstoqueIA ? 'Analisando...' : 'Analisar com IA'}
              </button>
            </div>
            {respostaEstoque && <p className={styles.iaResposta}>{respostaEstoque}</p>}
          </div>
        </div>
      </section>
    </div>
  );
}

export default Relatorios;
