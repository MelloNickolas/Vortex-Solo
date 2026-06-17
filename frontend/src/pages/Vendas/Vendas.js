import { useState, useEffect } from 'react';
import { Eye, X, Plus, ShoppingCart, Trash2 } from 'lucide-react';
import VendaApi from '../../services/VendaApi';
import ClienteApi from '../../services/ClienteApi';
import ProdutoApi from '../../services/ProdutoApi';
import CategoriaApi from '../../services/CategoriaApi';
import Modal from '../../components/Modal/Modal';
import Input from '../../components/Input/Input';
import SelectBusca from '../../components/SelectBusca/SelectBusca';
import Pagination from '../../components/Pagination/Pagination';
import styles from './Vendas.module.css';

// Opções fixas de forma de pagamento, os valores 1 ao 5 batem com o enum do backend
const opcoesPagamento = [
  { value: 1, label: 'Dinheiro' },
  { value: 2, label: 'Cartão Débito' },
  { value: 3, label: 'Cartão Crédito' },
  { value: 4, label: 'Pix' },
  { value: 5, label: 'Boleto' },
];

// Opções de filtro por status
const opcoesStatus = [
  { value: '', label: 'Todos os status' },
  { value: 'Pendente', label: 'Pendente' },
  { value: 'Concluida', label: 'Concluída' },
  { value: 'Cancelada', label: 'Cancelada' },
];

function Vendas() {
  // Lista de vendas paginada
  const [vendas, setVendas] = useState([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const pageSize = 10;

  // Filtros da listagem
  const [statusFiltro, setStatusFiltro] = useState('');
  const [de, setDe] = useState('');
  const [ate, setAte] = useState('');

  // Modal de nova venda
  const [modalNovaAberto, setModalNovaAberto] = useState(false);
  const [clientes, setClientes] = useState([]);    // para o SelectBusca de cliente
  const [categorias, setCategorias] = useState([]); // para o SelectBusca de categoria
  const [produtos, setProdutos] = useState([]);    // para o SelectBusca de produto (filtrado por categoria)
  const [clienteId, setClienteId] = useState('');
  const [formaPagamento, setFormaPagamento] = useState('');
  // Controles do "adicionar item"
  const [categoriaIdItem, setCategoriaIdItem] = useState(''); // categoria selecionada na seção de itens
  const [produtoIdItem, setProdutoIdItem] = useState('');
  const [quantidadeItem, setQuantidadeItem] = useState(1);
  // Itens já adicionados à venda
  const [itens, setItens] = useState([]);
  const [erroNova, setErroNova] = useState('');

  // Modal de detalhes
  const [modalDetalhesAberto, setModalDetalhesAberto] = useState(false);
  const [vendaSelecionada, setVendaSelecionada] = useState(null);

  // Recarrega sempre que page ou filtros mudarem
  useEffect(() => {
    carregarVendas();
  }, [page, statusFiltro, de, ate]);

  // Volta para página 1 quando filtros mudam
  useEffect(() => {
    setPage(1);
  }, [statusFiltro, de, ate]);

  async function carregarVendas() {
    try {
      const data = await VendaApi.ListarAsync({
        page,
        pageSize,
        // undefined não é enviado na URL
        status: statusFiltro || undefined,
        de: de || undefined,
        ate: ate || undefined,
      });
      setVendas(data.data);
      setTotalPages(Math.ceil(data.total / pageSize));
    } catch (error) {
      console.error('Erro ao carregar vendas:', error);
    }
  }

  async function abrirModalNova() {
    setClienteId('');
    setFormaPagamento('');
    setCategoriaIdItem('');
    setProdutoIdItem('');
    setQuantidadeItem(1);
    setItens([]);
    setErroNova('');
    setProdutos([]); // limpa produtos do modal anterior

    // Carrega clientes e categorias ao abrir o modal
    try {
      const [dadosClientes, dadosCategorias] = await Promise.all([
        ClienteApi.ListarAsync({ page: 1, pageSize: 999, ativo: true }),
        CategoriaApi.ListarAsync(),
      ]);
      setClientes(dadosClientes.data);
      setCategorias(dadosCategorias);
    } catch (error) {
      console.error('Erro ao carregar dados do formulário:', error);
    }

    setModalNovaAberto(true);
  }

  // Quando a categoria muda, busca os produtos dessa categoria no backend
  // e limpa o produto que estava selecionado (igual ao padrão Estado→Cidade)
  useEffect(() => {
    if (!categoriaIdItem) {
      setProdutos([]);
      setProdutoIdItem('');
      return;
    }
    async function carregarProdutosDaCategoria() {
      try {
        const data = await ProdutoApi.ListarAsync({
          page: 1,
          pageSize: 999,
          categoriaId: categoriaIdItem,
        });
        setProdutos(data.data);
        setProdutoIdItem(''); // limpa produto selecionado ao trocar categoria
      } catch (error) {
        console.error('Erro ao carregar produtos da categoria:', error);
      }
    }
    carregarProdutosDaCategoria();
  }, [categoriaIdItem]);

  function fecharModalNova() {
    setModalNovaAberto(false);
  }

  function handleAdicionarItem() {
    if (!produtoIdItem) return;

    // Procura o produto na lista pelo ID
    const produto = produtos.find((p) => p.id === Number(produtoIdItem));
    if (!produto) return;

    const qty = parseInt(quantidadeItem) || 1;

    // Se o produto já está na lista, só soma a quantidade
    const jaExiste = itens.find((i) => i.produtoId === produto.id);
    if (jaExiste) {
      setItens(itens.map((i) =>
        i.produtoId === produto.id
          ? { ...i, quantidade: i.quantidade + qty, subtotal: (i.quantidade + qty) * i.precoUnitario }
          : i
      ));
    } else {
      setItens([
        ...itens,
        {
          produtoId: produto.id,
          produtoNome: produto.nome,
          quantidade: qty,
          precoUnitario: produto.preco,
          // subtotal calculado aqui para não recalcular toda hora
          subtotal: qty * produto.preco,
        },
      ]);
    }

    // Limpa os campos de adicionar item
    setProdutoIdItem('');
    setQuantidadeItem(1);
  }

  function handleRemoverItem(produtoId) {
    setItens(itens.filter((i) => i.produtoId !== produtoId));
  }

  // Soma todos os subtotais dos itens
  const totalVenda = itens.reduce((acc, item) => acc + item.subtotal, 0);

  async function handleCriar(e) {
    e.preventDefault();
    setErroNova('');

    if (!clienteId) {
      setErroNova('Selecione um cliente.');
      return;
    }
    if (!formaPagamento) {
      setErroNova('Selecione a forma de pagamento.');
      return;
    }
    if (itens.length === 0) {
      setErroNova('Adicione pelo menos um item à venda.');
      return;
    }

    // UsuarioID do usuário logado (salvo no localStorage no login)
    const usuarioId = parseInt(localStorage.getItem('usuarioId'));

    try {
      await VendaApi.CriarAsync({
        ClienteID: Number(clienteId),
        UsuarioID: usuarioId,
        // FormaPagamento é um enum inteiro no backend (1=Dinheiro, 2=Debito...)
        FormaPagamento: Number(formaPagamento),
        Itens: itens.map((i) => ({
          ProdutoID: i.produtoId,
          Quantidade: i.quantidade,
        })),
      });
      await carregarVendas();
      fecharModalNova();
    } catch (error) {
      // Tenta mostrar a mensagem do backend, senão mostra o texto genérico
      const msg = error.response?.data?.mensagem
        || error.response?.data?.title
        || 'Erro ao criar venda.';
      setErroNova(msg);
    }
  }

  async function handleCancelar(id) {
    if (!window.confirm('Tem certeza que deseja cancelar esta venda?')) return;

    const usuarioId = parseInt(localStorage.getItem('usuarioId'));
    try {
      await VendaApi.CancelarAsync(id, usuarioId);
      await carregarVendas();
    } catch (error) {
      alert(error.response?.data?.mensagem || 'Erro ao cancelar venda.');
    }
  }

  async function abrirDetalhes(venda) {
    // Sempre busca pelo ID para garantir que os itens vêm completos
    try {
      const data = await VendaApi.BuscarPorIdAsync(venda.id);
      setVendaSelecionada(data);
      setModalDetalhesAberto(true);
    } catch (error) {
      console.error('Erro ao buscar detalhes da venda:', error);
    }
  }

  function fecharModalDetalhes() {
    setModalDetalhesAberto(false);
    setVendaSelecionada(null);
  }

  // Formata data ISO para dd/mm/aaaa hh:mm
  function formatarData(iso) {
    const d = new Date(iso);
    const data = d.toLocaleDateString('pt-BR');
    const hora = d.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
    return `${data} ${hora}`;
  }

  // Opções do SelectBusca de clientes: value=id, label=nome
  const opcoesClientes = clientes.map((c) => ({
    value: c.id,
    label: c.nome,
  }));

  // Opções do SelectBusca de categorias
  const opcoesCategorias = categorias.map((c) => ({
    value: c.id,
    label: c.nome,
  }));

  // Opções do SelectBusca de produtos — mostra nome, preço e estoque disponível
  const opcoesProdutos = produtos.map((p) => ({
    value: p.id,
    label: `${p.nome} — R$ ${Number(p.preco).toFixed(2)} (${p.estoqueAtual} em estoque)`,
  }));

  return (
    <div>
      <div className={styles.topo}>
        <h1 className="page-title">Vendas</h1>
        <button className={styles.btnNovo} onClick={abrirModalNova}>
          <Plus size={16} />
          Nova Venda
        </button>
      </div>

      {/* Filtros: status + intervalo de datas */}
      <div className={styles.filtros}>
        <div className={styles.filtroStatus}>
          <SelectBusca
            placeholder="Todos os status"
            value={statusFiltro}
            onChange={setStatusFiltro}
            options={opcoesStatus}
          />
        </div>
        <div className={styles.filtroData}>
          <Input
            label="De"
            type="date"
            value={de}
            onChange={(e) => setDe(e.target.value)}
          />
        </div>
        <div className={styles.filtroData}>
          <Input
            label="Até"
            type="date"
            value={ate}
            onChange={(e) => setAte(e.target.value)}
          />
        </div>
      </div>

      {/* Tabela de vendas */}
      <table className={styles.tabela}>
        <thead>
          <tr>
            <th>Cliente</th>
            <th>Usuário</th>
            <th>Data</th>
            <th>Pagamento</th>
            <th>Total</th>
            <th>Status</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {vendas.length === 0 ? (
            <tr>
              <td colSpan={7} className={styles.vazio}>Nenhuma venda encontrada.</td>
            </tr>
          ) : (
            vendas.map((venda) => (
              <tr key={venda.id}>
                <td>{venda.clienteNome}</td>
                <td>{venda.usuarioNome}</td>
                <td>{formatarData(venda.dataVenda)}</td>
                <td>{venda.formaPagamento}</td>
                <td>R$ {Number(venda.valorTotal).toFixed(2)}</td>
                <td>
                  <span className={styles[`status${venda.status}`]}>
                    {venda.status === 'Concluida' ? 'Concluída' : venda.status}
                  </span>
                </td>
                <td>
                  <div className={styles.acoes}>
                    {/* Lupa para ver detalhes */}
                    <button className={styles.btnVer} onClick={() => abrirDetalhes(venda)}>
                      <Eye size={15} />
                    </button>
                    {/* X para cancelar — só aparece se não estiver cancelada */}
                    {venda.status !== 'Cancelada' && (
                      <button className={styles.btnCancelar} onClick={() => handleCancelar(venda.id)}>
                        <X size={15} />
                      </button>
                    )}
                  </div>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>

      <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />

      {/* Modal nova venda */}
      {modalNovaAberto && (
        <Modal titulo="Nova Venda" onClose={fecharModalNova}>
          <form onSubmit={handleCriar}>
            <SelectBusca
              label="Cliente"
              placeholder="Selecione o cliente..."
              value={clienteId}
              onChange={setClienteId}
              options={opcoesClientes}
            />

            <SelectBusca
              label="Forma de Pagamento"
              placeholder="Selecione a forma de pagamento..."
              value={formaPagamento}
              onChange={setFormaPagamento}
              options={opcoesPagamento}
            />

            {/* Seção de itens */}
            <div className={styles.secaoItens}>
              <p className={styles.tituloItens}>
                <ShoppingCart size={15} /> Itens da Venda
              </p>

              {/* Primeiro seleciona a categoria */}
              <SelectBusca
                label="Categoria"
                placeholder="Selecione a categoria..."
                value={categoriaIdItem}
                onChange={setCategoriaIdItem}
                options={opcoesCategorias}
              />

              {/* Produto só aparece depois de selecionar a categoria — igual Estado→Cidade */}
              {categoriaIdItem && (
                <div className={styles.adicionarItem}>
                  <div className={styles.selectProduto}>
                    <SelectBusca
                      placeholder="Buscar produto..."
                      value={produtoIdItem}
                      onChange={setProdutoIdItem}
                      options={opcoesProdutos}
                    />
                  </div>
                  <Input
                    type="number"
                    value={quantidadeItem}
                    onChange={(e) => setQuantidadeItem(e.target.value)}
                    min={1}
                    style={{ width: '80px' }}
                  />
                  <button
                    type="button"
                    className={styles.btnAdicionarItem}
                    onClick={handleAdicionarItem}
                    disabled={!produtoIdItem}
                  >
                    <Plus size={15} />
                  </button>
                </div>
              )}

              {/* Tabela de itens já adicionados */}
              {itens.length > 0 && (
                <>
                  <table className={styles.tabelaItens}>
                    <thead>
                      <tr>
                        <th>Produto</th>
                        <th>Qtd</th>
                        <th>Preço Unit.</th>
                        <th>Subtotal</th>
                        <th></th>
                      </tr>
                    </thead>
                    <tbody>
                      {itens.map((item) => (
                        <tr key={item.produtoId}>
                          <td>{item.produtoNome}</td>
                          <td>{item.quantidade}</td>
                          <td>R$ {Number(item.precoUnitario).toFixed(2)}</td>
                          <td>R$ {Number(item.subtotal).toFixed(2)}</td>
                          <td>
                            <button
                              type="button"
                              className={styles.btnRemoverItem}
                              onClick={() => handleRemoverItem(item.produtoId)}
                            >
                              <Trash2 size={13} />
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                  <p className={styles.totalVenda}>
                    Total: <strong>R$ {totalVenda.toFixed(2)}</strong>
                  </p>
                </>
              )}
            </div>

            {erroNova && <p className={styles.erro}>{erroNova}</p>}
            <button type="submit" className={styles.btnSalvar}>
              Finalizar Venda
            </button>
          </form>
        </Modal>
      )}

      {/* Modal detalhes da venda */}
      {modalDetalhesAberto && vendaSelecionada && (
        <Modal titulo="Detalhes da Venda" onClose={fecharModalDetalhes}>
          <div className={styles.detalhesInfo}>
            <div className={styles.detalhesLinha}>
              <span className={styles.detalhesLabel}>Cliente</span>
              <span>{vendaSelecionada.clienteNome}</span>
            </div>
            <div className={styles.detalhesLinha}>
              <span className={styles.detalhesLabel}>Vendedor</span>
              <span>{vendaSelecionada.usuarioNome}</span>
            </div>
            <div className={styles.detalhesLinha}>
              <span className={styles.detalhesLabel}>Data</span>
              <span>{formatarData(vendaSelecionada.dataVenda)}</span>
            </div>
            <div className={styles.detalhesLinha}>
              <span className={styles.detalhesLabel}>Pagamento</span>
              <span>{vendaSelecionada.formaPagamento}</span>
            </div>
            <div className={styles.detalhesLinha}>
              <span className={styles.detalhesLabel}>Status</span>
              <span className={styles[`status${vendaSelecionada.status}`]}>
                {vendaSelecionada.status === 'Concluida' ? 'Concluída' : vendaSelecionada.status}
              </span>
            </div>
          </div>

          <p className={styles.tituloItens}><ShoppingCart size={15} /> Itens</p>

          <table className={styles.tabelaItens}>
            <thead>
              <tr>
                <th>Produto</th>
                <th>Qtd</th>
                <th>Preço Unit.</th>
                <th>Subtotal</th>
              </tr>
            </thead>
            <tbody>
              {vendaSelecionada.itens.map((item) => (
                <tr key={item.id}>
                  <td>{item.produtoNome}</td>
                  <td>{item.quantidade}</td>
                  <td>R$ {Number(item.precoUnitario).toFixed(2)}</td>
                  <td>R$ {Number(item.subtotal).toFixed(2)}</td>
                </tr>
              ))}
            </tbody>
          </table>

          <p className={styles.totalVenda}>
            Total: <strong>R$ {Number(vendaSelecionada.valorTotal).toFixed(2)}</strong>
          </p>
        </Modal>
      )}
    </div>
  );
}

export default Vendas;
