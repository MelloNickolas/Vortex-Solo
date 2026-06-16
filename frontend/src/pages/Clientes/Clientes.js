import { useState, useEffect } from 'react';
import { Pencil, Trash2, Plus, RotateCcw } from 'lucide-react';
import ClienteApi from '../../services/ClienteApi';
import EstadoApi from '../../services/EstadoApi';
import CidadeApi from '../../services/CidadeApi';
import Modal from '../../components/Modal/Modal';
import Input from '../../components/Input/Input';
import SelectBusca from '../../components/SelectBusca/SelectBusca';
import FiltroInativos from '../../components/FiltroInativos/FiltroInativos';
import Pagination from '../../components/Pagination/Pagination';
import styles from './Clientes.module.css';

function Clientes() {
  const [clientes, setClientes] = useState([]);

  // Filtros
  const [busca, setBusca] = useState('');
  const [mostrarInativos, setMostrarInativos] = useState(false);

  // Paginação
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const pageSize = 10;

  // Modal
  const [modalAberto, setModalAberto] = useState(false);
  const [clienteEditando, setClienteEditando] = useState(null);
  const [erro, setErro] = useState('');

  // Campos do formulário
  const [nome, setNome] = useState('');
  const [cpf, setCpf] = useState('');
  const [telefone, setTelefone] = useState('');
  const [email, setEmail] = useState('');
  const [rua, setRua] = useState('');
  const [numero, setNumero] = useState('');

  // Estados e cidades para o endereço
  const [estados, setEstados] = useState([]);
  const [cidades, setCidades] = useState([]);
  const [estadoId, setEstadoId] = useState('');
  const [cidadeId, setCidadeId] = useState('');

  // Carrega estados e clientes ao montar a página
  useEffect(() => {
    carregarEstados();
  }, []);

  // Recarrega clientes quando filtros ou página mudam
  useEffect(() => {
    carregarClientes();
  }, [page, busca, mostrarInativos]);

  // Quando o estado muda, carrega as cidades daquele estado e limpa a cidade selecionada
  useEffect(() => {
    if (estadoId) {
      carregarCidades(estadoId);
    } else {
      setCidades([]);
    }
    setCidadeId('');
  }, [estadoId]);

  async function carregarClientes() {
    try {
      const data = await ClienteApi.ListarAsync({
        page,
        pageSize,
        busca: busca || undefined,
        // Se mostrarInativos for true, passa false (mostra inativos)
        // Se for false, passa true (mostra só ativos)
        ativo: mostrarInativos ? false : true,
      });
      setClientes(data.data);
      setTotalPages(Math.ceil(data.total / pageSize));
    } catch (error) {
      console.error('Erro ao carregar clientes:', error);
    }
  }

  async function carregarEstados() {
    try {
      const data = await EstadoApi.ListarAsync();
      setEstados(data);
    } catch (error) {
      console.error('Erro ao carregar estados:', error);
    }
  }

  async function carregarCidades(idEstado) {
    try {
      const data = await CidadeApi.ListarPorEstadoAsync(idEstado);
      setCidades(data);
    } catch (error) {
      console.error('Erro ao carregar cidades:', error);
    }
  }

  function handleBuscaChange(e) {
    setBusca(e.target.value);
    setPage(1);
  }

  function handleMostrarInativos(valor) {
    setMostrarInativos(valor);
    setPage(1);
  }

  function abrirModalCriacao() {
    setClienteEditando(null);
    setNome(''); setCpf(''); setTelefone(''); setEmail('');
    setRua(''); setNumero(''); setEstadoId(''); setCidadeId('');
    setErro('');
    setModalAberto(true);
  }

  async function abrirModalEdicao(cliente) {
    setClienteEditando(cliente);
    setNome(cliente.nome);
    setCpf(cliente.cpf);
    setTelefone(cliente.telefone);
    setEmail(cliente.email);
    setRua(cliente.rua);
    setNumero(cliente.numero);
    setCidadeId(cliente.cidadeID);
    setErro('');

    // Precisa buscar o estado da cidade para preencher o select de estado
    // A resposta da cidade tem o estadoID
    try {
      const cidade = await CidadeApi.BuscarPorIdAsync(cliente.cidadeID);
      setEstadoId(cidade.estadoID);
    } catch (error) {
      console.error('Erro ao buscar cidade:', error);
    }

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
      CPF: cpf,
      Telefone: telefone,
      Email: email,
      Rua: rua,
      Numero: numero,
      CidadeID: parseInt(cidadeId),
    };

    try {
      if (clienteEditando) {
        await ClienteApi.AtualizarAsync(clienteEditando.id, payload);
      } else {
        await ClienteApi.CriarAsync(payload);
      }
      await carregarClientes();
      fecharModal();
    } catch (error) {
      setErro(error.response?.data?.mensagem || 'Erro ao salvar cliente.');
    }
  }

  async function handleDeletar(id) {
    if (!window.confirm('Tem certeza que deseja desativar este cliente?')) return;

    try {
      await ClienteApi.DeletarAsync(id);
      await carregarClientes();
    } catch (error) {
      alert(error.response?.data?.mensagem || 'Erro ao desativar cliente.');
    }
  }

  async function handleReativar(id) {
    if (!window.confirm('Deseja reativar este cliente?')) return;

    try {
      await ClienteApi.ReativarAsync(id);
      await carregarClientes();
    } catch (error) {
      alert(error.response?.data?.mensagem || 'Erro ao reativar cliente.');
    }
  }

  /*
  Resultado final:
  opcoesEstados = [
  { value: 1, label: 'São Paulo (SP)' },
  { value: 2, label: 'Rio de Janeiro (RJ)' },
]  
  */
  const opcoesEstados = estados.map((e) => ({ value: e.id, label: `${e.nome} (${e.uf})` }));
  const opcoesCidades = cidades.map((c) => ({ value: c.id, label: c.nome }));

  return (
    <div>
      <div className={styles.topo}>
        <h1 className="page-title">Clientes</h1>
        <button className={styles.btnNovo} onClick={abrirModalCriacao}>
          <Plus size={16} />
          Novo Cliente
        </button>
      </div>

      {/* Filtros */}
      <div className={styles.filtros}>
        <Input
          placeholder="Buscar por nome..."
          value={busca}
          onChange={handleBuscaChange}
        />
        <FiltroInativos value={mostrarInativos} onChange={handleMostrarInativos} />
      </div>

      {/* Tabela */}
      <table className={styles.tabela}>
        <thead>
          <tr>
            <th>Nome</th>
            <th>CPF</th>
            <th>Telefone</th>
            <th>Cidade</th>
            <th>Status</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {clientes.length === 0 ? (
            <tr>
              <td colSpan={6} className={styles.vazio}>Nenhum cliente encontrado.</td>
            </tr>
          ) : (
            clientes.map((cliente) => (
              <tr key={cliente.id}>
                <td>{cliente.nome}</td>
                <td>{cliente.cpf}</td>
                <td>{cliente.telefone}</td>
                <td>{cliente.cidadeNome}</td>
                <td>
                  {/* colore o status conforme se estiver ativo, ou nao */}
                  <span className={cliente.ativo ? styles.ativo : styles.inativo}>
                    {cliente.ativo ? 'Ativo' : 'Inativo'}
                  </span>
                </td>
                <td>
                  <div className={styles.acoes}>
                    <button className={styles.btnEditar} onClick={() => abrirModalEdicao(cliente)}>
                      <Pencil size={15} />
                    </button>
                    {/* Se ativo mostra lixeira para desativar. Se inativo mostra seta para reativar */}
                    {cliente.ativo ? (
                      <button className={styles.btnDeletar} onClick={() => handleDeletar(cliente.id)}>
                        <Trash2 size={15} />
                      </button>
                    ) : (
                      <button className={styles.btnReativar} onClick={() => handleReativar(cliente.id)}>
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

      {/* Modal */}
      {modalAberto && (
        <Modal
          titulo={clienteEditando ? 'Editar Cliente' : 'Novo Cliente'}
          onClose={fecharModal}
        >
          <form onSubmit={handleSalvar}>
            <Input label="Nome" value={nome} onChange={(e) => setNome(e.target.value)} placeholder="Nome completo" required />
            <Input label="CPF" value={cpf} onChange={(e) => setCpf(e.target.value)} placeholder="000.000.000-00" required />
            <Input label="Telefone" value={telefone} onChange={(e) => setTelefone(e.target.value)} placeholder="(11) 99999-9999" />
            <Input label="Email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="email@exemplo.com" />
            <Input label="Rua" value={rua} onChange={(e) => setRua(e.target.value)} placeholder="Nome da rua" />
            <Input label="Número" value={numero} onChange={(e) => setNumero(e.target.value)} placeholder="123" />

            {/* Estado primeiro — ao selecionar, carrega as cidades automaticamente */}
            <SelectBusca
              label="Estado"
              placeholder="Selecionar estado..."
              value={estadoId}
              onChange={(val) => setEstadoId(val)}
              options={opcoesEstados}
            />

            {/* Cidade só aparece depois que um estado for selecionado */}
            {estadoId && ( // essa linha que faz isso, se estado nao for null aparece o selectBusca
              <SelectBusca
                label="Cidade"
                placeholder="Selecionar cidade..."
                value={cidadeId}
                onChange={(val) => setCidadeId(val)}
                options={opcoesCidades}
              />
            )}

            {erro && <p className={styles.erro}>{erro}</p>}
            <button type="submit" className={styles.btnSalvar}>Salvar</button>
          </form>
        </Modal>
      )}
    </div>
  );
}

export default Clientes;
