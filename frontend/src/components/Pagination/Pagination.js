import styles from './Pagination.module.css';

// page (página atual), totalPages (total de páginas), onPageChange (função chamada ao mudar)
function Pagination({ page, totalPages, onPageChange }) {
  // Se só tem 1 página, nao mostra nada
  if (totalPages <= 1) return null;

  return (
    <div className={styles.wrapper}>
      <button
        className={styles.btn}
        onClick={() => onPageChange(page - 1)}
        disabled={page === 1} /* nao tem sentido apertar o botao de voltar e for a primeira página */
      >
        Anterior
      </button>

      {/* Mostra a página atual e o total */}
      <span className={styles.info}>
        {page} / {totalPages}
      </span>

      <button
        className={styles.btn}
        onClick={() => onPageChange(page + 1)}
        disabled={page === totalPages} /* nao tem sentido apertar o botao de próximo se for a ultima página */
      >
        Próximo
      </button>
    </div>
  );
}

export default Pagination;
