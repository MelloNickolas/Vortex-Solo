import { useState, useEffect } from 'react';
import { BarChart, Bar, LineChart, Line, XAxis, YAxis, Tooltip, ResponsiveContainer } from 'recharts';
import RelatorioApi from '../../services/RelatorioApi';
import DashboardApi from '../../services/DashboardApi';
import styles from './Dashboard.module.css';

// estilo do tooltip (a caixinha que aparece ao passar o mouse) pra combinar com o fundo escuro
const tooltipEstilo = { background: '#1a1d27', border: '1px solid #2a2d3e', color: '#e2e8f0' };

function Dashboard() {
  const [maisVendidos, setMaisVendidos] = useState([]);
  const [mesAno, setMesAno] = useState(() => new Date().toISOString().slice(0, 7));
  const [vendasPorDia, setVendasPorDia] = useState([]);

  useEffect(() => {
    async function carregar() {
      const mv = await RelatorioApi.ListarProdutosMaisVendidosAsync(5);
      setMaisVendidos(mv);
    }
    carregar();
  }, []);

  useEffect(() => {
    async function carregar() {
      const [ano, mes] = mesAno.split('-');
      const data = await DashboardApi.VendasPorDiaAsync(Number(mes), Number(ano));
      setVendasPorDia(data);
    }
    carregar();
  }, [mesAno]);

  const dadosPorDia = vendasPorDia.map((d) => ({ dia: d.dia, total: d.total }));
  const dadosProdutos = maisVendidos.map((p) => ({ nome: p.produto, total: p.totalFaturado }));

  return (
    <div>
      <h1 className="page-title">Dashboard de Vendas</h1>

      <h2>Faturamento por Dia</h2>
      <input type="month" className={styles.filtro} value={mesAno} onChange={(e) => setMesAno(e.target.value)} />
      <ResponsiveContainer width="100%" height={300}>
        <LineChart data={dadosPorDia}>
          <XAxis dataKey="dia" />
          <YAxis />
          <Tooltip contentStyle={tooltipEstilo} />
          <Line dataKey="total" stroke="#6366f1" />
        </LineChart>
      </ResponsiveContainer>

      <h2>Faturamento por Produto (Top 5)</h2>
      <ResponsiveContainer width="100%" height={300}> {/* Aqui é meio q o molde, é o nosso container */}
        <BarChart data={dadosProdutos}> {/* É aqui o array de dados que ele vai desenhar. cada iten do array vem dessa forma - nome: "teclado", total :"1500"*/}
          <XAxis dataKey="nome" /> {/* aqui definimos qual será o eixo horizontal, entao aq colocamos o nome do produto*/}
          <YAxis /> 
          <Tooltip contentStyle={tooltipEstilo} /> {/* caixinha que aparece quando você passa o mouse por cima de uma barra. e o style é aquele estilo escuro*/}
          <Bar dataKey="total" fill="#6366f1" /> {/* Aqui é onde define as narras, a datakey é a chave de altura */}
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}

export default Dashboard;
