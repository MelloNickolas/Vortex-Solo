import { useState, useEffect } from 'react';
import {
  BarChart, Bar, LineChart, Line,
  XAxis, YAxis, Tooltip, ResponsiveContainer, CartesianGrid,
} from 'recharts';
import RelatorioApi from '../../services/RelatorioApi';
import DashboardApi from '../../services/DashboardApi';
import styles from './Dashboard.module.css';

function Dashboard() {
  // resumo geral de vendas (vw_ResumoVendas)
  const [resumo, setResumo] = useState(null);
  // top produtos mais vendidos (sp_ProdutosMaisVendidos) — usados no gráfico de barras
  const [maisVendidos, setMaisVendidos] = useState([]);
  const [carregando, setCarregando] = useState(true);

  // mês selecionado no filtro — formato "AAAA-MM" (input type="month")
  // começa no mês atual
  const [mesAno, setMesAno] = useState(() => new Date().toISOString().slice(0, 7));
  // faturamento por dia do mês selecionado
  const [vendasPorDia, setVendasPorDia] = useState([]);

  // carrega resumo + top produtos uma vez ao abrir
  useEffect(() => {
    async function carregar() {
      try {
        const [res, mv] = await Promise.all([
          RelatorioApi.BuscarResumoAsync(),
          RelatorioApi.ListarProdutosMaisVendidosAsync(5),
        ]);
        setResumo(res);
        setMaisVendidos(mv);
      } catch (error) {
        console.error('Erro ao carregar dashboard:', error);
      } finally {
        setCarregando(false);
      }
    }
    carregar();
  }, []);

  // recarrega o gráfico por dia toda vez que o mês muda (filtro server-side)
  useEffect(() => {
    async function carregarVendasPorDia() {
      const [ano, mes] = mesAno.split('-'); // "2026-06" → ["2026", "06"]
      try {
        const data = await DashboardApi.VendasPorDiaAsync(Number(mes), Number(ano));
        setVendasPorDia(data);
      } catch (error) {
        console.error('Erro ao carregar vendas por dia:', error);
        setVendasPorDia([]);
      }
    }
    carregarVendasPorDia();
  }, [mesAno]);

  // formata número para moeda brasileira: 1234.5 → R$ 1.234,50
  function formatarMoeda(valor) {
    return Number(valor).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }

  if (carregando) {
    return <p className={styles.carregando}>Carregando dashboard...</p>;
  }

  // dados do gráfico mensal (por dia)
  const dadosPorDia = vendasPorDia.map((d) => ({ dia: d.dia, total: d.total }));

  // dados do gráfico de barras (top 5 produtos)
  const dadosGrafico = maisVendidos.map((p) => ({ nome: p.produto, total: p.totalFaturado }));

  return (
    <div>
      <h1 className="page-title">Dashboard de Vendas</h1>

      {/* Gráfico por dia do mês — com filtro de mês */}
      <section className={styles.grafico}>
        <div className={styles.graficoHeader}>
          <h2 className={styles.graficoTitulo}>Faturamento por Dia</h2>
          <input
            type="month"
            className={styles.filtroMes}
            value={mesAno}
            onChange={(e) => setMesAno(e.target.value)}
          />
        </div>
        {dadosPorDia.length === 0 ? (
          <p className={styles.vazio}>Nenhuma venda neste mês.</p>
        ) : (
          <ResponsiveContainer width="100%" height={300}>
            <LineChart data={dadosPorDia} margin={{ top: 10, right: 20, left: 0, bottom: 0 }}>
              <CartesianGrid strokeDasharray="3 3" stroke="#2a2d3e" />
              <XAxis dataKey="dia" stroke="#64748b" fontSize={12} />
              <YAxis stroke="#64748b" fontSize={12} />
              <Tooltip
                formatter={(valor) => formatarMoeda(valor)}
                labelFormatter={(dia) => `Dia ${dia}`}
                contentStyle={{ background: '#1a1d2e', border: '1px solid #2a2d3e', color: '#e2e8f0' }}
              />
              <Line type="monotone" dataKey="total" stroke="#6366f1" strokeWidth={2} dot={{ r: 3 }} />
            </LineChart>
          </ResponsiveContainer>
        )}
      </section>

      {/* Gráfico de barras: faturamento dos 5 produtos mais vendidos */}
      <section className={styles.grafico}>
        <h2 className={styles.graficoTitulo}>Faturamento por Produto (Top 5)</h2>
        <ResponsiveContainer width="100%" height={300}>
          <BarChart data={dadosGrafico} margin={{ top: 10, right: 20, left: 0, bottom: 0 }}>
            <CartesianGrid strokeDasharray="3 3" stroke="#2a2d3e" />
            <XAxis dataKey="nome" stroke="#64748b" fontSize={12} />
            <YAxis stroke="#64748b" fontSize={12} />
            <Tooltip
              formatter={(valor) => formatarMoeda(valor)}
              contentStyle={{ background: '#1a1d2e', border: '1px solid #2a2d3e', color: '#e2e8f0' }}
            />
            <Bar dataKey="total" fill="#6366f1" />
          </BarChart>
        </ResponsiveContainer>
      </section>

      {/* Cards com os números principais */}
      <div className={styles.cards}>
        <div className={styles.card}>
          <div className={styles.cardLabel}>Total de Vendas</div>
          <div className={styles.cardValor}>{resumo?.totalVendas ?? 0}</div>
        </div>
        <div className={styles.card}>
          <div className={styles.cardLabel}>Faturamento</div>
          <div className={styles.cardValor}>{formatarMoeda(resumo?.valorTotalGeral ?? 0)}</div>
        </div>
        <div className={styles.card}>
          <div className={styles.cardLabel}>Ticket Médio</div>
          <div className={styles.cardValor}>{formatarMoeda(resumo?.ticketMedio ?? 0)}</div>
        </div>
        <div className={styles.card}>
          <div className={styles.cardLabel}>Concluídas</div>
          <div className={styles.cardValorVerde}>{resumo?.vendasConcluidas ?? 0}</div>
        </div>
        <div className={styles.card}>
          <div className={styles.cardLabel}>Canceladas</div>
          <div className={styles.cardValorVermelho}>{resumo?.vendasCanceladas ?? 0}</div>
        </div>
      </div>
    </div>
  );
}

export default Dashboard;
