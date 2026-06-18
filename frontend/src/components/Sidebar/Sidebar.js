import { NavLink } from 'react-router-dom';
import { BarChart2, Package, Users, ShoppingCart, Warehouse, Tag, UserCog } from 'lucide-react';
import styles from './Sidebar.module.css';

// Lista de links da sidebar — centralizada aqui para facilitar adicionar/remover itens
const links = [
  { to: '/relatorios', label: 'Relatórios', icon: BarChart2 },
  { to: '/produtos',   label: 'Produtos',   icon: Package },
  { to: '/categorias', label: 'Categorias', icon: Tag },
  { to: '/clientes',   label: 'Clientes',   icon: Users },
  { to: '/vendas',     label: 'Vendas',     icon: ShoppingCart },
  { to: '/estoque',    label: 'Movimentações', icon: Warehouse },
  { to: '/usuarios',   label: 'Usuários',   icon: UserCog },
];

function Sidebar() {
  return (
    // <aside> é a tag HTML semântica para conteúdo lateral
    <aside className={styles.sidebar}>

      {/* Nome do sistema no topo da sidebar */}
      <div className={styles.logo}>Vortex</div>

      <nav className={styles.nav}>
        {/* Percorre o array de links e renderiza um NavLink para cada um */}
        {links.map(({ to, label, icon: Icon }) => (
          <NavLink
            key={to} // key é obrigatório em listas, o React usa para identificar cada item
            to={to}
            className={({ isActive }) =>
              // isActive vem do NavLink, true quando a rota atual bate com "to"
              // se tiver ativo aplica os estilos dos links e o estilo ativo || se não não estiver passa so o link
              isActive ? `${styles.link} ${styles.active}` : styles.link
            }
          >
            <Icon size={18} />
            <span>{label}</span>
          </NavLink>
        ))}
      </nav>
    </aside>
  );
}

export default Sidebar;
