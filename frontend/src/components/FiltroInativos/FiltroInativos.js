import styles from './FiltroInativos.module.css';

// onChange é a funcao que vai chamar
function FiltroInativos({ value, onChange }) {
  return (
    <button
      // Se value for true que é para mostrando inativos, aplica estilo ativo
      className={value ? `${styles.btn} ${styles.ativo}` : styles.btn}
      onClick={() => onChange(!value)} // inverte o valor atual ao clicar
      type="button"
    >
      {/* Muda o texto conforme o estado, sempre vai começar como falss, definimos isso na page */}
      {value ? 'Mostrando Inativos' : 'Mostrar Inativos'}
    </button> 
  );
}

export default FiltroInativos;
