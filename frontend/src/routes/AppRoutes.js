import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import RotaProtegida from './RotaProtegida';

// Páginas
import Login from '../pages/Login/Login';
import Dashboard from '../pages/Dashboard/Dashboard';
import Relatorios from '../pages/Relatorios/Relatorios';
import Categorias from '../pages/Categorias/Categorias';
import Produtos from '../pages/Produtos/Produtos';
import Clientes from '../pages/Clientes/Clientes';
import Usuarios from '../pages/Usuarios/Usuarios';
import Vendas from '../pages/Vendas/Vendas';
import Estoque from '../pages/Estoque/Estoque';

function AppRoutes() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />

        <Route path="/dashboard" element={<RotaProtegida><Dashboard /></RotaProtegida>} />
        <Route path="/relatorios" element={<RotaProtegida><Relatorios /></RotaProtegida>} />
        <Route path="/categorias" element={<RotaProtegida><Categorias /></RotaProtegida>} />
        <Route path="/produtos" element={<RotaProtegida><Produtos /></RotaProtegida>} />
        <Route path="/clientes" element={<RotaProtegida><Clientes /></RotaProtegida>} />
        <Route path="/usuarios" element={<RotaProtegida><Usuarios /></RotaProtegida>} />
        <Route path="/vendas" element={<RotaProtegida><Vendas /></RotaProtegida>} />
        <Route path="/estoque" element={<RotaProtegida><Estoque /></RotaProtegida>} />

        <Route path="*" element={<Navigate to="/login" />} />
      </Routes>
    </BrowserRouter>
  );
}

export default AppRoutes;
