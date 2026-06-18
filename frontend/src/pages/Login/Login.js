import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AuthApi from '../../services/AuthApi';
import UsuarioApi from '../../services/UsuarioApi';
import Input from '../../components/Input/Input';
import styles from './Login.module.css';

function Login() {
  // Controla qual formulário está visível: 'login' ou 'cadastro'
  const [modo, setModo] = useState('login');

  // Campos do login
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');

  // Campos do cadastro
  const [nome, setNome] = useState('');
  const [emailCadastro, setEmailCadastro] = useState('');
  const [telefone, setTelefone] = useState('');
  const [senhaCadastro, setSenhaCadastro] = useState('');

  // Mensagem de erro ou sucesso para exibir na tela
  const [mensagem, setMensagem] = useState('');

  // useNavigate permite redirecionar o usuário para outra rota
  const navigate = useNavigate();

  async function handleLogin(e) {
    // Impede o comportamento padrão do form de recarregar a página
    e.preventDefault();
    setMensagem('');

    try {
      const data = await AuthApi.LoginAsync({ Email: email, Senha: senha });

      // Salva o token no localStorage para ser usado nas próximas requisições
      localStorage.setItem('token', data.token);
      localStorage.setItem('usuarioNome', data.nome);
      // usuarioID (maiúsculo) porque o C# serializa UsuarioID → usuarioID em camelCase
      localStorage.setItem('usuarioId', data.usuarioID);

      // Redireciona para o dashboard após login bem-sucedido
      navigate('/relatorios');
    } catch (error) {
      setMensagem(error.response?.data?.mensagem || 'Erro ao fazer login.');
    }
  }

  async function handleCadastro(e) {
    e.preventDefault();
    setMensagem('');

    try {
      await UsuarioApi.CriarAsync({
        Nome: nome,
        Email: emailCadastro,
        Telefone: telefone,
        Senha: senhaCadastro,
      });

      // Volta para o login com mensagem de sucesso
      setMensagem('Cadastro realizado! Faça login.');
      setModo('login');
    } catch (error) {
      setMensagem(error.response?.data?.mensagem || 'Erro ao cadastrar.');
    }
  }

  return (
    <div className={styles.page}>
      <div className={styles.card}>

        {/* Nome do sistema */}
        <h1 className={styles.logo}>Vortex</h1>

        {/* Botões para alternar entre Login e Cadastro */}
        <div className={styles.tabs}>
          <button
            className={modo === 'login' ? styles.tabAtivo : styles.tab}
            onClick={() => { setModo('login'); setMensagem(''); }}
          >
            Login
          </button>
          <button
            className={modo === 'cadastro' ? styles.tabAtivo : styles.tab}
            onClick={() => { setModo('cadastro'); setMensagem(''); }}
          >
            Cadastro
          </button>
        </div>

        {/* Formulário de Login */}
        {modo === 'login' && (
          <form onSubmit={handleLogin} className={styles.form}>
            <Input
              label="Email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="seu@email.com"
              required
            />
            <Input
              label="Senha"
              type="password"
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              placeholder="••••••••"
              required
            />
            <button type="submit" className={styles.btnPrimario}>Entrar</button>
          </form>
        )}

        {/* Formulário de Cadastro */}
        {modo === 'cadastro' && (
          <form onSubmit={handleCadastro} className={styles.form}>
            <Input
              label="Nome"
              type="text"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
              placeholder="Seu nome"
              required
            />
            <Input
              label="Email"
              type="email"
              value={emailCadastro}
              onChange={(e) => setEmailCadastro(e.target.value)}
              placeholder="seu@email.com"
              required
            />
            <Input
              label="Telefone"
              type="text"
              value={telefone}
              onChange={(e) => setTelefone(e.target.value)}
              placeholder="(11) 99999-9999"
            />
            <Input
              label="Senha"
              type="password"
              value={senhaCadastro}
              onChange={(e) => setSenhaCadastro(e.target.value)}
              placeholder="••••••••"
              required
            />
            <button type="submit" className={styles.btnPrimario}>Cadastrar</button>
          </form>
        )}

        {/* Mensagem de erro ou sucesso, só aparece se tiver algo */}
        {mensagem && <p className={styles.mensagem}>{mensagem}</p>}

      </div>
    </div>
  );
}

export default Login;
