import { useState, useEffect } from 'react';
import RelatorioApi from '../../services/RelatorioApi';
import ClienteApi from '../../services/ClienteApi';
import AIApi from '../../services/AIApi';
import SelectBusca from '../../components/SelectBusca/SelectBusca';
import styles from './Relatorios.module.css';

function Relatorios() {
  // dados da vw_ResumoVendas
  const [resumo, setResumo] = useState(null);
  // dados da sp_ProdutosMaisVendidos
  const [maisVendidos, setMaisVendidos] = useState([]);
  // dados da vw_ProdutosAbaixoMinimo
  const [abaixoMinimo, setAbaixoMinimo] = useState([]);
  // dados da vw_VendasPorFormaPagamento
  const [formasPagamento, setFormasPagamento] = useState([]);

  // resposta de texto que a IA retorna
  const [respostaVendas, setRespostaVendas] = useState('');
  const [respostaEstoque, setRespostaEstoque] = useState('');

  // lista de clientes para fn_TotalVendasCliente
  const [clientes, setClientes] = useState([]);
  // id do cliente escolhido no select da fn_TotalVendasCliente
  const [clienteSelecionado, setClienteSelecionado] = useState('');
  // resultado da função fn_TotalVendasCliente
  const [totalCliente, setTotalCliente] = useState(null);

  // roda uma vez ao abrir a página, carrega tudo
  useEffect(() => {
    async function carregar() {
      try {
        // Promise.all dispara todas as chamadas ao mesmo tempo, não uma por uma
        const [res, mv, am, fp, cls] = await Promise.all([
          RelatorioApi.BuscarResumoAsync(), // vw_ResumoVendas
          RelatorioApi.ListarProdutosMaisVendidosAsync(10), // sp_ProdutosMaisVendidos
          RelatorioApi.ListarProdutosAbaixoMinimoAsync(), // vw_ProdutosAbaixoMinimo
          RelatorioApi.ListarFormasPagamentoAsync(), // vw_VendasPorFormaPagamento
          ClienteApi.ListarAsync({ page: 1, pageSize: 999 }), // todos os clientes pro select
        ]);
        setResumo(res); // dados do resumo de vendas
        setMaisVendidos(mv); // dos produtos mais vendidos
        setAbaixoMinimo(am); // dos abaixo do minimo
        setFormasPagamento(fp); // das por forma de pagamento
        setClientes(cls.data);  // fazemos isso pq ele retorna data: [], total, patge, entao pegamos os dados que sao os clientes
      } catch (error) {
        console.error('Erro ao carregar relatórios:', error);
      }
    }
    carregar();
  }, []);

  // chama a fn_TotalVendasCliente com o cliente selecionado
  async function handleConsultarTotalCliente() {
    if (!clienteSelecionado) return;
    setTotalCliente(null);
    try {
      const data = await RelatorioApi.ConsultarTotalClienteAsync(clienteSelecionado);
      setTotalCliente(data.total);
    } catch {
      setTotalCliente(0);
    }
  }

  // chama a IA para analisar os dados de vendas
  async function handleAnalisarVendas() {
    setRespostaVendas('');
    try {
      const data = await AIApi.AnalisarVendasAsync();
      setRespostaVendas(data.resposta);
    } catch {
      setRespostaVendas('Erro ao consultar a IA.');
    }
  }

  // chama a IA para analisar o estoque crítico
  async function handleAnalisarEstoque() {
    setRespostaEstoque('');
    try {
      const data = await AIApi.AnalisarEstoqueAsync();
      setRespostaEstoque(data.resposta);
    } catch {
      setRespostaEstoque('Erro ao consultar a IA.');
    }
  }

  // formata número para moeda brasileira: 1234.5 → R$ 1.234,50
  // é um helper que falamos
  function formatarMoeda(valor) {
    return Number(valor).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }

  return (
    <div>
      <h1 className="page-title">Relatórios</h1>

      {/*Cards de resumo vw_ResumoVendas*/}
      <section className={styles.secao}>
        <h2 className={styles.secaoTitulo}>Resumo Geral — <span className={styles.fonte}>View: vw_ResumoVendas</span></h2>
        <div className={styles.cards}>
          <div className={styles.card}>
            <div className={styles.cardLabel}>Total de Vendas</div>
            {/* ?? 0 garante que não exibe null se o dado não vier */}
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
            {/* cardValorVerde já tem o estilo de valor + cor verde */}
            <div className={styles.cardValorVerde}>{resumo?.vendasConcluidas ?? 0}</div>
          </div>
          <div className={styles.card}>
            <div className={styles.cardLabel}>Canceladas</div>
            {/* cardValorVermelho já tem o estilo de valor + cor vermelha */}
            <div className={styles.cardValorVermelho}>{resumo?.vendasCanceladas ?? 0}</div>
          </div>
        </div>
      </section>
      




      {/*Top 10 produtos sp_ProdutosMaisVendidos*/}
      <section className={styles.secao}>
        <h2 className={styles.secaoTitulo}>Top 10 Produtos Mais Vendidos — SP: sp_ProdutosMaisVendidos</h2>
        {/* se não tiver dados exibe mensagem, senão exibe a tabela */}
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
              {/* i é o índice do map, usado para mostrar a posição (#1, #2...) */}
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



      {/*Formas de pagamento vw_VendasPorFormaPagamento*/}
      <section className={styles.secao}>
        <h2 className={styles.secaoTitulo}>Vendas por Forma de Pagamento — vw_VendasPorFormaPagamento</h2>
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





      {/*Estoque crítico View: vw_ProdutosAbaixoMinimo*/}
      <section className={styles.secao}>
        <h2 className={styles.secaoTitulo}>
          Estoque Crítico — vw_ProdutosAbaixoMinimo
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
                  {/* estoque atual e déficit em vermelho para chamar atenção */}
                  <td className={styles.vermelho}>{item.estoqueAtual}</td>
                  <td>{item.estoqueMinimo}</td>
                  <td className={styles.vermelho}>{item.deficit}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>







      {/*Total por cliente Function: fn_TotalVendasCliente*/}
      <section className={styles.secao}>
        <h2 className={styles.secaoTitulo}>Total por Cliente — fn_TotalVendasCliente</h2>
        <div className={styles.funcaoBloco}>
          {/* select com busca ao mudar limpa o resultado anterior */}
          <SelectBusca
            placeholder="Selecione um cliente..."
            value={clienteSelecionado}
            onChange={(val) => { setClienteSelecionado(val); setTotalCliente(null); }}
            options={clientes.map(c => ({ value: c.id, label: c.nome }))}
          />
          <button
            className={styles.funcaoBotao}
            onClick={handleConsultarTotalCliente}
            disabled={!clienteSelecionado}
          >
            Consultar
          </button>
          {/* só aparece quando totalCliente tem valor (não é null) */}
          {totalCliente !== null && (
            <span className={styles.funcaoResultado}>
              Total faturado: <strong>{formatarMoeda(totalCliente)}</strong>
            </span>
          )}
        </div>
      </section>














      {/*Seção IA*/}
      <section className={styles.secao}>
        <h2 className={styles.secaoTitulo}>Inteligência Artificial — GitHub Models / gpt-4.1</h2>

        <div className={styles.iaBloco}>
          {/* análise de vendas usa dados da vw_ResumoVendas */}
          <div className={styles.iaItem}>
            <div className={styles.iaHeader}>
              <span>Análise de Vendas</span>
              <button className={styles.iaBotao} onClick={handleAnalisarVendas}>
                Analisar com IA
              </button>
            </div>
            {/* só aparece quando a IA retornar alguma resposta */}
            {respostaVendas && <p className={styles.iaResposta}>{respostaVendas}</p>}
          </div>

          {/* recomendações de estoque usa dados da vw_ProdutosAbaixoMinimo */}
          <div className={styles.iaItem}>
            <div className={styles.iaHeader}>
              <span>Recomendações de Estoque</span>
              <button className={styles.iaBotao} onClick={handleAnalisarEstoque}>
                Analisar com IA
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
