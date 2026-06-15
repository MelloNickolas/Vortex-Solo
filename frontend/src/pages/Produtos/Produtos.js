import { useState, useEffect } from 'react';
import { Pencil, Trash2, Plus } from 'lucide-react';
import ProdutoApi from '../../services/ProdutoApi';
import CategoriaApi from '../../services/CategoriaApi';
import Modal from '../../components/Modal/Modal';
import Input from '../../components/Input/Input';
import Select from '../../components/Select/Select';
import SelectBusca from '../../components/SelectBusca/SelectBusca';
import Pagination from '../../components/Pagination/Pagination';
import styles from './Produtos.module.css';

function Produtos() {
  // buscas na API
  const [produtos, setProdutos] = useState([]);
  const [categorias, setCategorias] = useState([]);

  // Filtros de busca
  const [busca, setBusca] = useState('');
  const [categoriaFiltro, setCategoriaFiltro] = useState('');

  // Paginação
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const pageSize = 10;

  // Modal
  const [modalAberto, setModalAberto] = useState(false);
  const [produtoEditando, setProdutoEditando] = useState(null);
  const [erro, setErro] = useState('');

  // Campos do formulário
  const [nome, setNome] = useState('');
  const [descricao, setDescricao] = useState('');
  const [preco, setPreco] = useState('');
  const [estoqueAtual, setEstoqueAtual] = useState('');
  const [estoqueMinimo, setEstoqueMinimo] = useState('');
  const [categoriaId, setCategoriaId] = useState('');

  // Carrega produtos e categorias ao montar o componente
  useEffect(() => {
    carregarCategorias();
  }, []);

  // Recarrega produtos sempre que page, busca ou categoriaFiltro mudar
  useEffect(() => {
    carregarProdutos();
  }, [page, busca, categoriaFiltro]);

  async function carregarProdutos() {
    try {
      const data = await ProdutoApi.ListarAsync({
        page,
        pageSize,
        nome: busca || undefined,           // O || funciona assim, se o valor da esquerda for vazio, usa o da direita.
        categoriaId: categoriaFiltro || undefined,
      });
      setProdutos(data.data);
      /*
        Imagina que você tem 23 produtos e o pageSize é 10:
        23 / 10 = 2.3 páginas     
        Então Math.ceil garante que sempre vai ter páginas suficientes para mostrar todos os itens, mesmo que a última página não esteja cheia 
      */
      setTotalPages(Math.ceil(data.total / pageSize));
    } catch (error) {
      console.error('Erro ao carregar produtos:', error);
    }
  }

  async function carregarCategorias() {
    try {
      const data = await CategoriaApi.ListarAsync();
      setCategorias(data);
    } catch (error) {
      console.error('Erro ao carregar categorias:', error);
    }
  }

  // Quando o usuário muda o filtro, volta para a página 1
  function handleBuscaChange(e) {
    setBusca(e.target.value);
    setPage(1);
  }

  function handleCategoriaFiltroChange(e) {
    setCategoriaFiltro(e.target.value);
    setPage(1);
  }

  function abrirModalCriacao() {
    setProdutoEditando(null);
    setNome('');
    setDescricao('');
    setPreco('');
    setEstoqueAtual('');
    setEstoqueMinimo('');
    setCategoriaId('');
    setErro('');
    setModalAberto(true);
  }

  function abrirModalEdicao(produto) {
    setProdutoEditando(produto);
    setNome(produto.nome);
    setDescricao(produto.descricao);
    setPreco(produto.preco);
    setEstoqueAtual(produto.estoqueAtual);
    setEstoqueMinimo(produto.estoqueMinimo);
    setCategoriaId(produto.categoriaID);
    setErro('');
    setModalAberto(true);
  }

  function fecharModal() {
    setModalAberto(false);
    setErro('');
  }

  async function handleSalvar(e) {
    e.preventDefault();
    setErro('');

    const payload = {
      Nome: nome,
      Descricao: descricao,
      Preco: parseFloat(preco),         // converte string para número
      EstoqueAtual: parseInt(estoqueAtual),
      EstoqueMinimo: parseInt(estoqueMinimo),
      CategoriaID: parseInt(categoriaId),
    };

    try {
      if (produtoEditando) {
        await ProdutoApi.AtualizarAsync(produtoEditando.id, payload);
      } else {
        await ProdutoApi.CriarAsync(payload);
      }
      await carregarProdutos();
      fecharModal();
    } catch (error) {
      setErro(error.response?.data?.mensagem || 'Erro ao salvar produto.');
    }
  }

  async function handleDeletar(id) {
    if (!window.confirm('Tem certeza que deseja deletar este produto?')) return;

    try {
      await ProdutoApi.DeletarAsync(id);
      await carregarProdutos();
    } catch (error) {
      alert(error.response?.data?.mensagem || 'Erro ao deletar produto.');
    }
  }

  // Monta as opções do Select de categoria para o filtro
  // A primeira opção vazia significa "todas as categorias"
  const opcoesCategoriaFiltro = [
    { value: '', label: 'Todas as categorias' },
    ...categorias.map((c) => ({ value: c.id, label: c.nome })),
  ];

  // Opções do Select dentro do modal (sem a opção vazia)
  const opcoesCategoriaForm = categorias.map((c) => ({ value: c.id, label: c.nome }));

  return (
    <div>
      <div className={styles.topo}>
        <h1 className="page-title">Produtos</h1>
        <button className={styles.btnNovo} onClick={abrirModalCriacao}>
          <Plus size={16} />
          Novo Produto
        </button>
      </div>

      {/* Filtros */}
      <div className={styles.filtros}>
        <Input
          placeholder="Buscar por nome..."
          value={busca}
          onChange={handleBuscaChange}
        />
        <SelectBusca
          placeholder="Todas as categorias"
          value={categoriaFiltro}
          onChange={(val) => { setCategoriaFiltro(val); setPage(1); }}
          // Função chamada quando o usuário seleciona uma opção. O val é o value da opção clicada (ex: 5 para a categoria de ID 5). Faz duas coisas:
          // setCategoriaFiltro(val) = salva a categoria escolhida
          // setPage(1) = volta para a página 1, porque se você estava na página 3 e mudou o filtro, os resultados mudam e não faz sentido continuar na página 3

          options={opcoesCategoriaFiltro}
        />
      </div>

      {/* Tabela */}
      <table className={styles.tabela}>
        <thead>
          <tr>
            <th>Nome</th>
            <th>Categoria</th>
            <th>Preço</th>
            <th>Estoque Atual</th>
            <th>Estoque Mínimo</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {produtos.length === 0 ? (
            <tr>
              <td colSpan={6} className={styles.vazio}>Nenhum produto encontrado.</td>
            </tr>
          ) : (
            produtos.map((produto) => (
              <tr key={produto.id}>
                <td>{produto.nome}</td>
                <td>{produto.categoriaNome}</td>
                {/* toFixed(2) formata o número com 2 casas decimais*/}
                <td>R$ {Number(produto.preco).toFixed(2)}</td>
                <td>{produto.estoqueAtual}</td>
                <td>{produto.estoqueMinimo}</td>
                <td>
                  <div className={styles.acoes}>
                    <button className={styles.btnEditar} onClick={() => abrirModalEdicao(produto)}>
                      <Pencil size={15} />
                    </button>
                    <button className={styles.btnDeletar} onClick={() => handleDeletar(produto.id)}>
                      <Trash2 size={15} />
                    </button>
                  </div>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>

      <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />

      {/* Modal */}
      {modalAberto && (
        <Modal
          titulo={produtoEditando ? 'Editar Produto' : 'Novo Produto'}
          onClose={fecharModal}
        >
          <form onSubmit={handleSalvar}>
            <Input
              label="Nome"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
              placeholder="Nome do produto"
              required
            />
            <Input
              label="Descrição"
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
              placeholder="Descrição do produto"
            />
            <Input
              label="Preço"
              type="number"
              step="0.01"
              value={preco}
              onChange={(e) => setPreco(e.target.value)}
              placeholder="0.00"
              required
            />
            <Input
              label="Estoque Atual"
              type="number"
              value={estoqueAtual}
              onChange={(e) => setEstoqueAtual(e.target.value)}
              placeholder="0"
              required
            />
            <Input
              label="Estoque Mínimo"
              type="number"
              value={estoqueMinimo}
              onChange={(e) => setEstoqueMinimo(e.target.value)}
              placeholder="0"
              required
            />
            <SelectBusca
              label="Categoria"
              placeholder="Selecionar categoria..."
              value={categoriaId}
              onChange={(val) => setCategoriaId(val)}
              options={opcoesCategoriaForm}
            />
            {erro && <p className={styles.erro}>{erro}</p>}
            <button type="submit" className={styles.btnSalvar}>Salvar</button>
          </form>
        </Modal>
      )}
    </div>
  );
}

export default Produtos;
