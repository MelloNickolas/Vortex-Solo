import { useState, useEffect } from 'react';
import ClienteApi from '../../services/ClienteApi';
import AIApi from '../../services/AIApi';
import SelectBusca from '../../components/SelectBusca/SelectBusca';
import styles from './Relatorios.module.css';

function Relatorios() {
  // dados + resposta da IA de cada objeto SQL — tudo começa vazio
  // só é preenchido quando o usuário clica em "Analisar com IA"
  const [resumo, setResumo] = useState(null);
  const [respostaVendas, setRespostaVendas] = useState('');

  const [maisVendidos, setMaisVendidos] = useState(null);
  const [respostaProdutos, setRespostaProdutos] = useState('');

  const [formasPagamento, setFormasPagamento] = useState(null);
  const [respostaFormasPagamento, setRespostaFormasPagamento] = useState('');

  const [abaixoMinimo, setAbaixoMinimo] = useState(null);
  const [respostaEstoque, setRespostaEstoque] = useState('');

  // controla o "Analisando..." de cada botão
  const [carregando, setCarregando] = useState('');

  // lista de clientes para fn_TotalVendasCliente
  const [clientes, setClientes] = useState([]);
  const [clienteSelecionado, setClienteSelecionado] = useState('');
  const [totalCliente, setTotalCliente] = useState(null);
  const [respostaCliente, setRespostaCliente] = useState('');

  // só carrega a lista de clientes pro select (os relatórios vêm pela IA)
  useEffect(() => {
    async function carregarClientes() {
      try {
        const cls = await ClienteApi.ListarAsync({ page: 1, pageSize: 999 });
        setClientes(cls.data);
      } catch (error) {
        console.error('Erro ao carregar clientes:', error);
      }
    }
    carregarClientes();
  }, []);

  // VIEW: vw_ResumoVendas
  async function handleAnalisarVendas() {
    setCarregando('vendas');
    try {
      const data = await AIApi.AnalisarVendasAsync();
      setResumo(data.dados);
      setRespostaVendas(data.resposta);
    } catch {
      setRespostaVendas('Erro ao consultar a IA.');
    } finally {
      setCarregando('');
    }
  }

  // SP: sp_ProdutosMaisVendidos
  async function handleAnalisarProdutos() {
    setCarregando('produtos');
    try {
      const data = await AIApi.AnalisarProdutosMaisVendidosAsync();
      setMaisVendidos(data.dados);
      setRespostaProdutos(data.resposta);
    } catch {
      setRespostaProdutos('Erro ao consultar a IA.');
    } finally {
      setCarregando('');
    }
  }

  // VIEW: vw_VendasPorFormaPagamento
  async function handleAnalisarFormasPagamento() {
    setCarregando('formas');
    try {
      const data = await AIApi.AnalisarFormasPagamentoAsync();
      setFormasPagamento(data.dados);
      setRespostaFormasPagamento(data.resposta);
    } catch {
      setRespostaFormasPagamento('Erro ao consultar a IA.');
    } finally {
      setCarregando('');
    }
  }

  // VIEW: vw_ProdutosAbaixoMinimo
  async function handleAnalisarEstoque() {
    setCarregando('estoque');
    try {
      const data = await AIApi.AnalisarEstoqueAsync();
      setAbaixoMinimo(data.dados);
      setRespostaEstoque(data.resposta);
    } catch {
      setRespostaEstoque('Erro ao consultar a IA.');
    } finally {
      setCarregando('');
    }
  }

  // FUNCTION: fn_TotalVendasCliente
  async function handleAnalisarCliente() {
    if (!clienteSelecionado) return;
    setCarregando('cliente');
    try {
      const data = await AIApi.AnalisarClienteAsync(clienteSelecionado);
      setTotalCliente(data.dados.total);
      setRespostaCliente(data.resposta);
    } catch {
      setRespostaCliente('Erro ao consultar a IA.');
    } finally {
      setCarregando('');
    }
  }

  // formata número para moeda brasileira: 1234.5 → R$ 1.234,50
  function formatarMoeda(valor) {
    return Number(valor).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }

  // bloco do texto da análise da IA, reaproveitado em todas as seções
  function blocoResposta(texto) {
    if (!texto) return null;
    return (
      <div className={styles.iaResposta}>
        <div className={styles.iaRespostaLabel}>✦ Análise da IA</div>
        {texto}
      </div>
    );
  }

  return (
    <div>
      <h1 className="page-title">Relatórios Inteligentes</h1>

      {/* Resumo de vendas — VIEW: vw_ResumoVendas */}
      <section className={styles.secao}>
        <div className={styles.secaoHeader}>
          <div>
            <h2 className={styles.secaoNome}>Desempenho de Vendas</h2>
            <p className={styles.secaoDesc}>
              A IA analisa o faturamento total, ticket médio e a proporção de vendas concluídas
              x canceladas, gerando insights sobre a saúde financeira do negócio.
            </p>
          </div>
          <button className={styles.iaBotao} onClick={handleAnalisarVendas} disabled={carregando === 'vendas'}>
            {carregando === 'vendas' ? 'Analisando...' : 'Analisar com IA'}
          </button>
        </div>
        {!resumo ? (
          <div className={styles.placeholder}>Clique em "Analisar com IA" para gerar este relatório.</div>
        ) : (
          <>
            <div className={styles.cards}>
              <div className={styles.card}>
                <div className={styles.cardLabel}>Total de Vendas</div>
                <div className={styles.cardValor}>{resumo.totalVendas ?? 0}</div>
              </div>
              <div className={styles.card}>
                <div className={styles.cardLabel}>Valor Total Faturado</div>
                <div className={styles.cardValor}>{formatarMoeda(resumo.valorTotalGeral ?? 0)}</div>
              </div>
              <div className={styles.card}>
                <div className={styles.cardLabel}>Ticket Médio</div>
                <div className={styles.cardValor}>{formatarMoeda(resumo.ticketMedio ?? 0)}</div>
              </div>
              <div className={styles.card}>
                <div className={styles.cardLabel}>Concluídas</div>
                <div className={styles.cardValorVerde}>{resumo.vendasConcluidas ?? 0}</div>
              </div>
              <div className={styles.card}>
                <div className={styles.cardLabel}>Canceladas</div>
                <div className={styles.cardValorVermelho}>{resumo.vendasCanceladas ?? 0}</div>
              </div>
            </div>
            {blocoResposta(respostaVendas)}
          </>
        )}
      </section>

      {/* Top 10 produtos — SP: sp_ProdutosMaisVendidos */}
      <section className={styles.secao}>
        <div className={styles.secaoHeader}>
          <div>
            <h2 className={styles.secaoNome}>Produtos Mais Vendidos</h2>
            <p className={styles.secaoDesc}>
              A IA examina o ranking dos 10 produtos com maior volume de vendas e identifica
              padrões de categoria, destaques e recomendações estratégicas para o mix de produtos.
            </p>
          </div>
          <button className={styles.iaBotao} onClick={handleAnalisarProdutos} disabled={carregando === 'produtos'}>
            {carregando === 'produtos' ? 'Analisando...' : 'Analisar com IA'}
          </button>
        </div>
        {!maisVendidos ? (
          <div className={styles.placeholder}>Clique em "Analisar com IA" para gerar este relatório.</div>
        ) : (
          <>
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
            {blocoResposta(respostaProdutos)}
          </>
        )}
      </section>

      {/* Formas de pagamento — VIEW: vw_VendasPorFormaPagamento */}
      <section className={styles.secao}>
        <div className={styles.secaoHeader}>
          <div>
            <h2 className={styles.secaoNome}>Formas de Pagamento</h2>
            <p className={styles.secaoDesc}>
              A IA analisa como os clientes preferem pagar, destacando a forma mais utilizada,
              o total movimentado por cada uma e recomendações sobre o comportamento de pagamento.
            </p>
          </div>
          <button className={styles.iaBotao} onClick={handleAnalisarFormasPagamento} disabled={carregando === 'formas'}>
            {carregando === 'formas' ? 'Analisando...' : 'Analisar com IA'}
          </button>
        </div>
        {!formasPagamento ? (
          <div className={styles.placeholder}>Clique em "Analisar com IA" para gerar este relatório.</div>
        ) : (
          <>
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
            {blocoResposta(respostaFormasPagamento)}
          </>
        )}
      </section>

      {/* Estoque crítico — VIEW: vw_ProdutosAbaixoMinimo */}
      <section className={styles.secao}>
        <div className={styles.secaoHeader}>
          <div>
            <h2 className={styles.secaoNome}>Estoque Crítico</h2>
            <p className={styles.secaoDesc}>
              A IA verifica quais produtos estão abaixo do estoque mínimo e gera recomendações
              práticas de reposição, priorizando os itens com maior déficit.
            </p>
          </div>
          <button className={styles.iaBotao} onClick={handleAnalisarEstoque} disabled={carregando === 'estoque'}>
            {carregando === 'estoque' ? 'Analisando...' : 'Analisar com IA'}
          </button>
        </div>
        {!abaixoMinimo ? (
          <div className={styles.placeholder}>Clique em "Analisar com IA" para gerar este relatório.</div>
        ) : (
          <>
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
            {blocoResposta(respostaEstoque)}
          </>
        )}
      </section>

      {/* Total por cliente — FUNCTION: fn_TotalVendasCliente */}
      <section className={styles.secao}>
        <div className={styles.secaoHeader}>
          <div>
            <h2 className={styles.secaoNome}>Perfil de Cliente</h2>
            <p className={styles.secaoDesc}>
              Selecione um cliente e a IA analisa o total já comprado por ele, classificando
              o perfil e sugerindo uma ação comercial personalizada.
            </p>
          </div>
        </div>
        <div className={styles.funcaoBloco}>
          <SelectBusca
            placeholder="Selecione um cliente..."
            value={clienteSelecionado}
            onChange={(val) => { setClienteSelecionado(val); setTotalCliente(null); setRespostaCliente(''); }}
            options={clientes.map(c => ({ value: c.id, label: c.nome }))}
          />
          <button
            className={styles.iaBotao}
            onClick={handleAnalisarCliente}
            disabled={!clienteSelecionado || carregando === 'cliente'}
          >
            {carregando === 'cliente' ? 'Analisando...' : 'Analisar com IA'}
          </button>
        </div>
        {totalCliente !== null && (
          <>
            <p className={styles.funcaoResultado}>
              Total faturado: <strong>{formatarMoeda(totalCliente)}</strong>
            </p>
            {blocoResposta(respostaCliente)}
          </>
        )}
      </section>
    </div>
  );
}

export default Relatorios;
