import React from 'react';
import { Check, Minus } from 'lucide-react';
import styles from './Controls.module.css';

export interface CheckboxProps
  extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'type'> {
  label?: string;
  indeterminate?: boolean;
  error?: boolean;
}

export const Checkbox = React.forwardRef<HTMLInputElement, CheckboxProps>(
  ({ label, checked = false, indeterminate = false, error = false, disabled, className = '', onChange, ...props }, ref) => {
    const isChecked = Boolean(checked);
    const containerClasses = [
      styles.checkboxContainer,
      isChecked ? styles.checkboxChecked : '',
      indeterminate ? styles.checkboxIndeterminate : '',
      error ? styles.checkboxError : '',
      disabled ? styles.disabled : '',
      className,
    ]
      .filter(Boolean)
      .join(' ');

    return (
      <label className={containerClasses}>
        <input
          ref={ref}
          type="checkbox"
          className={styles.checkboxInput}
          checked={isChecked}
          disabled={disabled}
          onChange={onChange}
          {...props}
        />
        <div className={styles.checkboxBox}>
          {indeterminate ? (
            <Minus size={14} strokeWidth={3} />
          ) : isChecked ? (
            <Check size={14} strokeWidth={3} />
          ) : null}
        </div>
        {label && <span>{label}</span>}
      </label>
    );
  }
);

Checkbox.displayName = 'Checkbox';

