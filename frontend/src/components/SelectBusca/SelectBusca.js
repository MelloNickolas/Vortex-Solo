import { useState } from 'react';
import styles from './SelectBusca.module.css';

// Select com busca
function SelectBusca({ label, options = [], value, onChange, placeholder = 'Buscar...' }) {

  // Texto digitado no campo de busca
  const [busca, setBusca] = useState('');
  // Controla se a lista está aberta ou fechada
  const [aberto, setAberto] = useState(false);

  // Filtra as opções conforme o texto digitado
  const opcoesFiltradas = options.filter((opt) =>
    opt.label.toLowerCase().includes(busca.toLowerCase())
  );

  // Acha o label da opção selecionada para exibir no input
  const opcaoSelecionada = options.find((opt) => String(opt.value) === String(value));

  function handleSelecionar(opt) {
    onChange(opt.value); // muda o valor do select quando for selecionado
    setBusca('');      
    setAberto(false);   
  }

  function handleAbrir() {
    setBusca(''); // limpa busca pra mostrar tudo
    setAberto(true);
  }

  return (
    <div className={styles.wrapper}>
      {label && <label className={styles.label}>{label}</label>}

      {/* Campo que mostra a opção selecionada ou o input de busca */}
      <div className={styles.campo} onClick={handleAbrir}>
        {aberto ? (
          // se tiver o modal aberto, aparece pra buscar
          <input
            className={styles.input}
            value={busca}
            onChange={(e) => setBusca(e.target.value)}
            placeholder={placeholder}
          />
        ) : (
          // Quando tiver fechado tu mostra a opcao que foi selecionada, se nao tiver nada, mostra o placeholder
          <span className={opcaoSelecionada ? styles.selecionado : styles.placeholder}>
            {opcaoSelecionada ? opcaoSelecionada.label : placeholder}
          </span>
        )}
        <span className={styles.seta}>▾</span>
      </div>

      {/* Lista de opções para aparecer só  aberto */}
      {aberto && (
        <div className={styles.lista}>
          {opcoesFiltradas.length === 0 ? (
            <div className={styles.vazio}>Nenhum resultado</div>
          ) : (
            opcoesFiltradas.map((opt) => (
              <div
                key={opt.value}
                className={styles.opcao}
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
