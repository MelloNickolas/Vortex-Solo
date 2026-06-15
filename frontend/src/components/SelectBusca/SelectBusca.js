import { useState, useEffect, useRef } from 'react';
import styles from './SelectBusca.module.css';

// Select com campo de busca integrado
// Recebe: label, options ({ value, label }), value, onChange, placeholder
function SelectBusca({ label, options = [], value, onChange, placeholder = 'Buscar...' }) {
  // Texto digitado no campo de busca
  const [busca, setBusca] = useState('');

  // Controla se a lista está aberta ou fechada
  const [aberto, setAberto] = useState(false);

  // useRef aponta para o elemento HTML do componente
  // Usado para detectar clique fora e fechar a lista
  const ref = useRef(null);

  // Fecha o dropdown quando o usuário clica fora do componente
  useEffect(() => {
    function handleClickFora(e) {
      if (ref.current && !ref.current.contains(e.target)) {
        setAberto(false);
      }
    }
    document.addEventListener('mousedown', handleClickFora);
    // Remove o listener quando o componente sai da tela (cleanup)
    return () => document.removeEventListener('mousedown', handleClickFora);
  }, []);

  // Filtra as opções conforme o texto digitado
  const opcoesFiltradas = options.filter((opt) =>
    opt.label.toLowerCase().includes(busca.toLowerCase())
  );

  // Acha o label da opção selecionada para exibir no input
  const opcaoSelecionada = options.find((opt) => String(opt.value) === String(value));

  function handleSelecionar(opt) {
    onChange(opt.value); // avisa o pai qual valor foi selecionado
    setBusca('');        // limpa a busca
    setAberto(false);    // fecha a lista
  }

  function handleAbrir() {
    setBusca(''); // limpa busca ao abrir para mostrar todas as opções
    setAberto(true);
  }

  return (
    <div className={styles.wrapper} ref={ref}>
      {label && <label className={styles.label}>{label}</label>}

      {/* Campo que mostra a opção selecionada ou o input de busca */}
      <div className={styles.campo} onClick={handleAbrir}>
        {aberto ? (
          // Quando aberto: mostra input para digitar a busca
          <input
            className={styles.input}
            value={busca}
            onChange={(e) => setBusca(e.target.value)}
            placeholder={placeholder}
            autoFocus // foca automaticamente ao abrir
          />
        ) : (
          // Quando fechado: mostra o item selecionado ou placeholder
          <span className={opcaoSelecionada ? styles.selecionado : styles.placeholder}>
            {opcaoSelecionada ? opcaoSelecionada.label : placeholder}
          </span>
        )}
        {/* Seta indicando que é um dropdown */}
        <span className={styles.seta}>▾</span>
      </div>

      {/* Lista de opções — só aparece quando aberto */}
      {aberto && (
        <div className={styles.lista}>
          {opcoesFiltradas.length === 0 ? (
            <div className={styles.vazio}>Nenhum resultado</div>
          ) : (
            opcoesFiltradas.map((opt) => (
              <div
                key={opt.value}
                className={`${styles.opcao} ${String(opt.value) === String(value) ? styles.ativo : ''}`}
                onClick={() => handleSelecionar(opt)}
              >
                {opt.label}
              </div>
            ))
          )}
        </div>
      )}
    </div>
  );
}

export default SelectBusca;
