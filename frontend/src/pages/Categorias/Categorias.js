import { useState, useEffect } from 'react';
import { Pencil, Trash2, Plus } from 'lucide-react';
import CategoriaApi from '../../services/CategoriaApi';
import Modal from '../../components/Modal/Modal';
import Input from '../../components/Input/Input';
import styles from './Categorias.module.css';

function Categorias() {
  // Lista de categorias vindas da API
  const [categorias, setCategorias] = useState([]);

  // Texto digitado no filtro de busca
  const [busca, setBusca] = useState('');

  // Controla se o modal está aberto ou fechado
  const [modalAberto, setModalAberto] = useState(false);

  // Categoria sendo editada, null significa que é uma criação nova
  const [categoriaEditando, setCategoriaEditando] = useState(null);

  // Campo do formulário dentro do modal
  const [nome, setNome] = useState('');

  // Mensagem de erro dentro do modal
  const [erro, setErro] = useState('');

  // useEffect executa quando o componente carrega pela primeira vez
  useEffect(() => {
    carregarCategorias();
  }, []);

  async function carregarCategorias() {
    try {
      const data = await CategoriaApi.ListarAsync();
      setCategorias(data);
    } catch (error) {
      console.error('Erro ao carregar categorias:', error);
    }
  }

  // Abre o modal para criar uma nova categoria
  function abrirModalCriacao() {
    setCategoriaEditando(null); // null = criação
    setNome('');
    setErro('');
    setModalAberto(true);
  }

  // Abre o modal preenchido com os dados da categoria para editar
  function abrirModalEdicao(categoria) {
    setCategoriaEditando(categoria);
    setNome(categoria.nome);
    setErro('');
    setModalAberto(true);
  }

  function fecharModal() {
    setModalAberto(false);
    setNome('');
    setErro('');
  }

  async function handleSalvar(e) {
    e.preventDefault();
    setErro('');

    try {
      if (categoriaEditando) {
        // Se tem categoria sendo editada, atualiza
        await CategoriaApi.AtualizarAsync(categoriaEditando.id, { Nome: nome });
      } else {
        // Senão, cria uma nova
        await CategoriaApi.CriarAsync({ Nome: nome });
      }

      // Recarrega a lista e fecha o modal
      await carregarCategorias();
      fecharModal();
    } catch (error) {
      setErro(error.response?.data?.mensagem || 'Erro ao salvar categoria.');
    }
  }

  async function handleDeletar(id) {
    // Pede confirmação antes de deletar
    if (!window.confirm('Tem certeza que deseja deletar esta categoria?')) return;

    try {
      await CategoriaApi.DeletarAsync(id);
      await carregarCategorias();
    } catch (error) {
      alert(error.response?.data?.mensagem || 'Erro ao deletar categoria.');
    }
  }

  // Filtra as categorias localmente conforme o usuário digita na busca
  // toLowerCase para a busca não ser sensível a maiúsculas/minúsculas
  const categoriasFiltradas = categorias.filter((c) =>
    c.nome.toLowerCase().includes(busca.toLowerCase())
  );

  return (
    <div>
      <div className={styles.topo}>
        <h1 className="page-title">Categorias</h1>
        <button className={styles.btnNovo} onClick={abrirModalCriacao}>
          <Plus size={16} />
          Nova Categoria
        </button>
      </div>

      {/* Filtro de busca */}
      <div className={styles.filtros}>
        <Input
          placeholder="Buscar categoria..."
          value={busca}
          onChange={(e) => setBusca(e.target.value)}
        />
      </div>

      {/* Tabela de categorias */}
      <table className={styles.tabela}>
        <thead>
          <tr>
            <th>Nome</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {categoriasFiltradas.length === 0 ? (
            // Mensagem quando não há resultados
            <tr>
              <td colSpan={2} className={styles.vazio}>Nenhuma categoria encontrada.</td>
            </tr>
          ) : (
            categoriasFiltradas.map((categoria) => (
              <tr key={categoria.id}>
                <td>{categoria.nome}</td>
                <td>
                  <div className={styles.acoes}>
                    <button
                      className={styles.btnEditar}
                      onClick={() => abrirModalEdicao(categoria)}
                    >
                      <Pencil size={15} />
                    </button>
                    <button
                      className={styles.btnDeletar}
                      onClick={() => handleDeletar(categoria.id)}
                    >
                      <Trash2 size={15} />
                    </button>
                  </div>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>

      {/* Modal de criação/edição, ele so vai renderiza se estiver aberto */}
      {modalAberto && (
        <Modal
          titulo={categoriaEditando ? 'Editar Categoria' : 'Nova Categoria'}
          onClose={fecharModal}
        >
          <form onSubmit={handleSalvar}>
            <Input
              label="Nome"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
              placeholder="Nome da categoria"
              required
            />
            {/* Mensagem de erro do formulário */}
            {erro && <p className={styles.erro}>{erro}</p>}
            <button type="submit" className={styles.btnSalvar}>
              Salvar
            </button>
          </form>
        </Modal>
      )}
    </div>
  );
}

export default Categorias;
