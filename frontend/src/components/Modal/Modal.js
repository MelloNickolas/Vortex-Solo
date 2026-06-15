import { X } from 'lucide-react';
import styles from './Modal.module.css';


// titulo, onClose (função para fechar), children (conteúdo interno)
function Modal({ titulo, onClose, children }) {
  return (
    // Fundo escuro que cobre a página inteira
    <div className={styles.overlay} onClick={onClose}>

      {/* O e.stopPropagation() impede que clicar dentro do card feche o modal */}
      <div className={styles.card} onClick={(e) => e.stopPropagation()}>

        {/* Cabeçalho com título e botão de fechar */}
        <div className={styles.header}>
          <h2 className={styles.titulo}>{titulo}</h2>
          <button className={styles.btnFechar} onClick={onClose}>
            <X size={18} />
          </button>
        </div>

        {/* Conteúdo passado por dentro do componente */}
        <div className={styles.corpo}>
          {children}
        </div>

      </div>
    </div>
  );
}

export default Modal;
