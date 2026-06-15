import styles from './Select.module.css';

// label, value, onChange, e options (array de { value, label })
function Select({ label, value, onChange, options = [], ...props }) {
  return (
    <div className={styles.wrapper}>
      {/* Se for passado voce renderiza o label*/}
      {label && <label className={styles.label}>{label}</label>}
      <select
        className={styles.select}
        value={value}
        onChange={onChange}
        {...props}
      >
        {/* Mapeia o array de opções em elementos <option> */}
        {options.map((opt) => (
          <option key={opt.value} value={opt.value}>
            {opt.label}
          </option>
        ))}
      </select>
    </div>
  );
}

export default Select;
