import { useState, useEffect } from 'react';
import { Pencil, Trash2, Plus, RotateCcw, KeyRound } from 'lucide-react';
import UsuarioApi from '../../services/UsuarioApi';
import Modal from '../../components/Modal/Modal';
import Input from '../../components/Input/Input';
import FiltroInativos from '../../components/FiltroInativos/FiltroInativos';
import Pagination from '../../components/Pagination/Pagination';
import styles from './Usuarios.module.css';

function Usuarios() {
  // Lista retornada pelo backend (já paginada e filtrada)
  const [usuarios, setUsuarios] = useState([]);

  // Filtros enviados para a API
  const [busca, setBusca] = useState('');
  const [mostrarInativos, setMostrarInativos] = useState(false);

  // Controle de paginação
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const pageSize = 10;

  // Modal de criar/editar
  const [modalAberto, setModalAberto] = useState(false);
  const [usuarioEditando, setUsuarioEditando] = useState(null);
  const [erro, setErro] = useState('');

  // Campos do formulário
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [telefone, setTelefone] = useState('');
  const [senha, setSenha] = useState('');

  // Modal de alterar senha
  const [modalSenhaAberto, setModalSenhaAberto] = useState(false);
  const [usuarioSenha, setUsuarioSenha] = useState(null);
  const [senhaAtual, setSenhaAtual] = useState('');
  const [novaSenha, setNovaSenha] = useState('');
  const [erroSenha, setErroSenha] = useState('');

  // Recarrega sempre que page, busca ou filtro de status mudar
  useEffect(() => {
    carregarUsuarios();
  }, [page, busca, mostrarInativos]);

  // Quando muda busca ou status, volta para página 1
  // (evita ficar em página que não existe mais após o filtro)
  useEffect(() => {
    setPage(1);
  }, [busca, mostrarInativos]);

  async function carregarUsuarios() {
    try {
      const data = await UsuarioApi.ListarAsync({
        page,
        pageSize,
        // busca || undefined evita mandar ?busca= vazio na URL
        busca: busca || undefined,
        // manda ativo=true para ativos, ativo=false para inativos
        ativo: mostrarInativos ? false : true,
      });

      setUsuarios(data.data);
      // Math.ceil(23/10) = 3 páginas — sempre arredonda pra cima
      setTotalPages(Math.ceil(data.total / pageSize));
    } catch (error) {
      console.error('Erro ao carregar usuários:', error);
    }
  }

  function abrirModalCriacao() {
    setUsuarioEditando(null);
    setNome(''); setEmail(''); setTelefone(''); setSenha('');
    setErro('');
    setModalAberto(true);
  }

  function abrirModalEdicao(usuario) {
    setUsuarioEditando(usuario);
    setNome(usuario.nome);
    setEmail(usuario.email);
    setTelefone(usuario.telefone);
    setSenha(''); // senha nunca vem preenchida na edição
    setErro('');
    setModalAberto(true);
  }

  function fecharModal() {
    setModalAberto(false);
    setErro('');
  }

  function abrirModalSenha(usuario) {
    setUsuarioSenha(usuario);
    setSenhaAtual(''); setNovaSenha('');
    setErroSenha('');
    setModalSenhaAberto(true);
  }

  function fecharModalSenha() {
    setModalSenhaAberto(false);
    setErroSenha('');
  }

  async function handleSalvar(e) {
    e.preventDefault();
    setErro('');

    try {
      if (usuarioEditando) {
        // Na edição não envia senha — alterada pelo modal de alterar senha
        await UsuarioApi.AtualizarAsync(usuarioEditando.id, {
          Nome: nome,
          Email: email,
          Telefone: telefone,
          Senha: '',
        });
      } else {
        await UsuarioApi.CriarAsync({ Nome: nome, Email: email, Telefone: telefone, Senha: senha });
      }
      await carregarUsuarios();
      fecharModal();
    } catch (error) {
      setErro(error.response?.data?.mensagem || 'Erro ao salvar usuário.');
    }
  }

  async function handleAlterarSenha(e) {
    e.preventDefault();
    setErroSenha('');

    try {
      await UsuarioApi.AlterarSenhaAsync(usuarioSenha.id, {
        SenhaAtual: senhaAtual,
        NovaSenha: novaSenha,
      });
      fecharModalSenha();
      alert('Senha alterada com sucesso!');
    } catch (error) {
      setErroSenha(error.response?.data?.mensagem || 'Erro ao alterar senha.');
    }
  }

  async function handleDeletar(id) {
    if (!window.confirm('Tem certeza que deseja desativar este usuário?')) return;

    try {
      await UsuarioApi.DeletarAsync(id);
      await carregarUsuarios();
    } catch (error) {
      alert(error.response?.data?.mensagem || 'Erro ao desativar usuário.');
    }
  }

  async function handleReativar(id) {
    if (!window.confirm('Deseja reativar este usuário?')) return;

    try {
      await UsuarioApi.ReativarAsync(id);
      await carregarUsuarios();
    } catch (error) {
      alert(error.response?.data?.mensagem || 'Erro ao reativar usuário.');
    }
  }

  return (
    <div>
      <div className={styles.topo}>
        <h1 className="page-title">Usuários</h1>
        <button className={styles.btnNovo} onClick={abrirModalCriacao}>
          <Plus size={16} />
          Novo Usuário
        </button>
      </div>

      {/* Filtros — enviados para o backend na query string */}
      <div className={styles.filtros}>
        <Input
          placeholder="Buscar por nome ou email..."
          value={busca}
          onChange={(e) => setBusca(e.target.value)}
        />
        <FiltroInativos value={mostrarInativos} onChange={setMostrarInativos} />
      </div>

      {/* Tabela */}
      <table className={styles.tabela}>
        <thead>
          <tr>
            <th>Nome</th>
            <th>Email</th>
            <th>Telefone</th>
            <th>Status</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {usuarios.length === 0 ? (
            <tr>
              <td colSpan={5} className={styles.vazio}>Nenhum usuário encontrado.</td>
            </tr>
          ) : (
            usuarios.map((usuario) => (
              <tr key={usuario.id}>
                <td>{usuario.nome}</td>
                <td>{usuario.email}</td>
                <td>{usuario.telefone}</td>
                <td>
                  <span className={usuario.ativo ? styles.ativo : styles.inativo}>
                    {usuario.ativo ? 'Ativo' : 'Inativo'}
                  </span>
                </td>
                <td>
                  <div className={styles.acoes}>
                    <button className={styles.btnEditar} onClick={() => abrirModalEdicao(usuario)}>
                      <Pencil size={15} />
                    </button>
                    {/* Chave amarela para abrir modal de alterar senha */}
                    <button className={styles.btnSenha} onClick={() => abrirModalSenha(usuario)}>
                      <KeyRound size={15} />
                    </button>
                    {/* Lixeira para ativo, RotateCcw para inativo */}
                    {usuario.ativo ? (
                      <button className={styles.btnDeletar} onClick={() => handleDeletar(usuario.id)}>
                        <Trash2 size={15} />
                      </button>
                    ) : (
                      <button className={styles.btnReativar} onClick={() => handleReativar(usuario.id)}>
                        <RotateCcw size={15} />
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

      {/* Modal criar e editar */}
      {modalAberto && (
        <Modal
          titulo={usuarioEditando ? 'Editar Usuário' : 'Novo Usuário'}
          onClose={fecharModal}
        >
          <form onSubmit={handleSalvar}>
            <Input label="Nome" value={nome} onChange={(e) => setNome(e.target.value)} placeholder="Nome completo" required />
            <Input label="Email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="email@exemplo.com" required />
            <Input label="Telefone" value={telefone} onChange={(e) => setTelefone(e.target.value)} placeholder="(11) 99999-9999" required />
            {/* Campo senha só aparece na criação — na edição usa modal de alterar senha */}
            {!usuarioEditando && (
              <Input label="Senha" type="password" value={senha} onChange={(e) => setSenha(e.target.value)} placeholder="Mínimo 6 caracteres" required />
            )}
            {erro && <p className={styles.erro}>{erro}</p>}
            <button type="submit" className={styles.btnSalvar}>Salvar</button>
          </form>
        </Modal>
      )}

      {/* Modal alterar senha */}
      {modalSenhaAberto && (
        <Modal titulo={`Alterar Senha — ${usuarioSenha?.nome}`} onClose={fecharModalSenha}>
          <form onSubmit={handleAlterarSenha}>
            <Input label="Senha Atual" type="password" value={senhaAtual} onChange={(e) => setSenhaAtual(e.target.value)} placeholder="••••••••" required />
            <Input label="Nova Senha" type="password" value={novaSenha} onChange={(e) => setNovaSenha(e.target.value)} placeholder="Mínimo 6 caracteres" required />
            {erroSenha && <p className={styles.erro}>{erroSenha}</p>}
            <button type="submit" className={styles.btnSalvar}>Alterar Senha</button>
          </form>
        </Modal>
      )}
    </div>
  );
}

export default Usuarios;
