import styles from './Select.module.css';


// label é o texto que vai aparecer primeiro no select
// value é o valor que ta selecionado, exemplo, concluido ou cancelada
// onchnage é a funcao para mudar o value
// options passa o value do select e tbm oq vai querer mostrar quando estiver selecionado
// props é o resto
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
