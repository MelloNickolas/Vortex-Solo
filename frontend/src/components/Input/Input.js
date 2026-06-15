import styles from './Input.module.css';

// label , value, onChange, e qualquer outro atributo HTML (placeholder, type, etc.)
function Input({ label, value, onChange, ...props }) {
  return (
    <div className={styles.wrapper}>
      {/* Só renderiza o label se ele for passado || && = se ele for passado renderiza */}
      {label && <label className={styles.label}>{label}</label>}
      <input
        className={styles.input}
        value={value}
        onChange={onChange}
        {...props} // repassa tudo: placeholder, type="password", disabled, etc.
      />
    </div>
  );
}

export default Input;
