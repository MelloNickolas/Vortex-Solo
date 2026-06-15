import { Navigate } from 'react-router-dom';
import Sidebar from '../components/Sidebar/Sidebar';
import '../App.css';

// Componente que protege rotas, se não tiver token, manda para o login
function RotaProtegida({ children }) {
  const token = localStorage.getItem('token');

  // Se não tiver token salvo, redireciona para /login
  if (!token) {
    return <Navigate to="/login" />;
  }

  // Se tiver token, renderiza o layout com sidebar e o conteúdo da página
  return (
    <div className="layout">
      <Sidebar />
      <main className="content">
        {children}
      </main>
    </div>
  );
}

export default RotaProtegida;
