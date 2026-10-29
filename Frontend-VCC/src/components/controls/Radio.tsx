import React from 'react';
import styles from './Controls.module.css';

export interface RadioProps
  extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'type'> {
  label?: string;
}

export const Radio = React.forwardRef<HTMLInputElement, RadioProps>(
  ({ label, checked = false, disabled, className = '', onChange, ...props }, ref) => {
    const isChecked = Boolean(checked);
    const containerClasses = [
      styles.radioContainer,
      isChecked ? styles.radioChecked : '',
      disabled ? styles.disabled : '',
      className,
    ]
      .filter(Boolean)
      .join(' ');

    return (
      <label className={containerClasses}>
        <input
          ref={ref}
          type="radio"
          className={styles.radioInput}
          checked={isChecked}
          disabled={disabled}
          onChange={onChange}
          {...props}
        />
        <div className={styles.radioCircle}>
          <div className={styles.radioDot} />
        </div>
        {label && <span>{label}</span>}
      </label>
    );
  }
);

Radio.displayName = 'Radio';

