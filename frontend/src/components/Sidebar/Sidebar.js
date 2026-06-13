import { NavLink } from 'react-router-dom';
import {
  LayoutDashboard,
  Package,
  Users,
  ShoppingCart,
  Warehouse,
  Tag,
  UserCog,
} from 'lucide-react';
import styles from './Sidebar.module.css';

const links = [
  { to: '/dashboard',     label: 'Dashboard',     icon: LayoutDashboard },
  { to: '/produtos',      label: 'Produtos',       icon: Package },
  { to: '/categorias',    label: 'Categorias',     icon: Tag },
  { to: '/clientes',      label: 'Clientes',       icon: Users },
  { to: '/vendas',        label: 'Vendas',         icon: ShoppingCart },
  { to: '/estoque',       label: 'Estoque',        icon: Warehouse },
  { to: '/usuarios',      label: 'Usuários',       icon: UserCog },
];

function Sidebar() {
  return (
    <aside className={styles.sidebar}>
      <div className={styles.logo}>Vortex</div>

      <nav className={styles.nav}>
        {links.map(({ to, label, icon: Icon }) => (
          <NavLink
            key={to}
            to={to}
            className={({ isActive }) =>
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
