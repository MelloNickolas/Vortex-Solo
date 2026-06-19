import { X } from 'lucide-react';
import styles from './Modal.module.css';


// 
function Modal({ titulo, onClose /*funcao para fechar*/ , children }) {
  return (
    // Fundo escuro da página, e se apertar nele, fecha
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

      
        <div className={styles.corpo}>
          {children}
        </div>

      </div>
    </div>
  );
}

export default Modal;
