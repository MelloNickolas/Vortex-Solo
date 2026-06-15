import './App.css';
import AppRoutes from './routes/AppRoutes';

// App é o ponto de entrada da aplicação
// Ele só renderiza o AppRoutes, que cuida de todas as rotas
function App() {
  return <AppRoutes />;
}

export default App;
