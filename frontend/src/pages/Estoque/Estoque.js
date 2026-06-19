import { useState, useEffect } from 'react';
import { ArrowDownCircle, ArrowUpCircle } from 'lucide-react';
import MovimentacaoApi from '../../services/MovimentacaoApi';
import ProdutoApi from '../../services/ProdutoApi';
import Modal from '../../components/Modal/Modal';
import Input from '../../components/Input/Input';
import SelectBusca from '../../components/SelectBusca/SelectBusca';
import Pagination from '../../components/Pagination/Pagination';
import styles from './Estoque.module.css';

// vamos usar para passar no select
const opcoesTipo = [
  { value: '', label: 'Todos os tipos' },
  { value: 'Entrada', label: 'Entrada' },
  { value: 'Saida', label: 'Saída' },
];
const motivosEntrada = [
  { value: 3, label: 'Compra' },
  { value: 4, label: 'Devolução' },
];
const motivosSaida = [
  { value: 5, label: 'Ajuste Manual' },
  { value: 6, label: 'Perda/Extravio' },
];
const labelMotivo = {
  Venda: 'Venda',
  CancelamentoVenda: 'Cancelamento de Venda',
  Compra: 'Compra',
  Devolucao: 'Devolução',
  AjusteManual: 'Ajuste Manual',
  Perda: 'Perda/Extravio',
};

function Estoque() {
  const [movimentacoes, setMovimentacoes] = useState([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const pageSize = 10;

  const [tipoFiltro, setTipoFiltro] = useState('');

  const [modalEntradaAberto, setModalEntradaAberto] = useState(false);
  const [modalSaidaAberto, setModalSaidaAberto] = useState(false);

  const [produtos, setProdutos] = useState([]);
  const [produtoId, setProdutoId] = useState('');
  const [quantidade, setQuantidade] = useState(1);
  const [motivo, setMotivo] = useState('');
  const [erro, setErro] = useState('');

  useEffect(() => {
    carregarMovimentacoes();
  }, [page, tipoFiltro]);

  useEffect(() => {
    setPage(1);
  }, [tipoFiltro]);

  async function carregarMovimentacoes() {
    try {
      const data = await MovimentacaoApi.ListarAsync({
        page,
        pageSize,
        tipo: tipoFiltro || undefined,
      });
      setMovimentacoes(data.data);
      setTotalPages(Math.ceil(data.total / pageSize));
    } catch (error) {
      console.error('Erro ao carregar movimentações:', error);
    }
  }

  async function abrirModal(tipo) {
    setProdutoId('');
    setQuantidade(1);
    setMotivo('');
    setErro('');

    try {
      const data = await ProdutoApi.ListarAsync({ page: 1, pageSize: 999 });
      setProdutos(data.data);
    } catch (error) {
      console.error('Erro ao carregar produtos:', error);
    }

    if (tipo === 'Entrada') setModalEntradaAberto(true);
    else setModalSaidaAberto(true);
  }

  function fecharModais() {
    setModalEntradaAberto(false);
    setModalSaidaAberto(false);
  }

  async function handleRegistrar(tipo) {
    setErro('');

    if (!produtoId) { setErro('Selecione um produto.'); return; }
    if (!motivo) { setErro('Selecione um motivo.'); return; }
    if (!quantidade || parseInt(quantidade) <= 0) { setErro('Informe uma quantidade válida.'); return; }

    const usuarioId = parseInt(localStorage.getItem('usuarioId'));
    if (!usuarioId || isNaN(usuarioId)) {
      setErro('Sessão inválida. Faça logout e login novamente.');
      return;
    }

    try {
      await MovimentacaoApi.CriarAsync({
        ProdutoID: Number(produtoId),
        UsuarioID: usuarioId,
        Tipo: tipo === 'Entrada' ? 1 : 2,
        Motivo: Number(motivo),
        Quantidade: parseInt(quantidade),
      });
      await carregarMovimentacoes();
      fecharModais();
    } catch (error) {
      setErro(error.response?.data?.mensagem || 'Erro ao registrar movimentação.');
    }
  }

  function formatarData(iso) {
    const d = new Date(iso);
    return `${d.toLocaleDateString('pt-BR')} ${d.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}`;
  }

  const opcoesProdutos = produtos.map((p) => ({
    value: p.id,
    label: `${p.nome} (${p.estoqueAtual} em estoque)`,
  }));

  return (
    <div>
      <div className={styles.topo}>
        <h1 className="page-title">Movimentações</h1>
        <div className={styles.botoes}>
          <button className={styles.btnEntrada} onClick={() => abrirModal('Entrada')}>
            <ArrowDownCircle size={16} />
            Registrar Entrada
          </button>
          <button className={styles.btnSaida} onClick={() => abrirModal('Saida')}>
            <ArrowUpCircle size={16} />
            Registrar Saída
          </button>
        </div>
      </div>

      <div className={styles.filtros}>
        <SelectBusca
          placeholder="Todos os tipos"
          value={tipoFiltro}
          onChange={setTipoFiltro}
          options={opcoesTipo}
        />
      </div>

      <table className={styles.tabela}>
        <thead>
          <tr>
            <th>Data</th>
            <th>Produto</th>
            <th>Tipo</th>
            <th>Motivo</th>
            <th>Quantidade</th>
            <th>Usuário</th>
          </tr>
        </thead>
        <tbody>
          {movimentacoes.length === 0 ? (
            <tr>
              <td colSpan={6} className={styles.vazio}>Nenhuma movimentação encontrada.</td>
            </tr>
          ) : (
            movimentacoes.map((m) => (
              <tr key={m.id}>
                <td>{formatarData(m.dataMovimento)}</td>
                <td>{m.produtoNome}</td>
                <td>
                  <span className={m.tipo === 'Entrada' ? styles.badgeEntrada : styles.badgeSaida}>
                    {m.tipo === 'Saida' ? 'Saída' : m.tipo}
                  </span>
                </td>
                <td>{labelMotivo[m.motivo] ?? m.motivo}</td>
                <td>{m.quantidade}</td>
                <td>{m.usuarioNome}</td>
              </tr>
            ))
          )}
        </tbody>
      </table>

      <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />

      {modalEntradaAberto && (
        <Modal titulo="Registrar Entrada" onClose={fecharModais}>
          <SelectBusca
            label="Produto"
            placeholder="Selecione o produto..."
            value={produtoId}
            onChange={setProdutoId}
            options={opcoesProdutos}
          />
          <Input
            label="Quantidade"
            type="number"
            value={quantidade}
            onChange={(e) => setQuantidade(e.target.value)}
            min={1}
          />
          <SelectBusca
            label="Motivo"
            placeholder="Selecione o motivo..."
            value={motivo}
            onChange={setMotivo}
            options={motivosEntrada}
          />
          {erro && <p className={styles.erro}>{erro}</p>}
          <button className={styles.btnSalvar} onClick={() => handleRegistrar('Entrada')}>
            Confirmar Entrada
          </button>
        </Modal>
      )}

      {modalSaidaAberto && (
        <Modal titulo="Registrar Saída" onClose={fecharModais}>
          <SelectBusca
            label="Produto"
            placeholder="Selecione o produto..."
            value={produtoId}
            onChange={setProdutoId}
            options={opcoesProdutos}
          />
          <Input
            label="Quantidade"
            type="number"
            value={quantidade}
            onChange={(e) => setQuantidade(e.target.value)}
            min={1}
          />
          <SelectBusca
            label="Motivo"
            placeholder="Selecione o motivo..."
            value={motivo}
            onChange={setMotivo}
            options={motivosSaida}
          />
          {erro && <p className={styles.erro}>{erro}</p>}
          <button className={styles.btnSalvar} onClick={() => handleRegistrar('Saida')}>
            Confirmar Saída
          </button>
        </Modal>
      )}
    </div>
  );
}

export default Estoque;
