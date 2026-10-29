import React from 'react';
import styles from './Controls.module.css';

export interface SwitchProps
  extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'type'> {
  label?: string;
  thumbIcon?: React.ReactNode;
}

export const Switch = React.forwardRef<HTMLInputElement, SwitchProps>(
  ({ label, checked = false, thumbIcon, disabled, className = '', onChange, ...props }, ref) => {
    const isChecked = Boolean(checked);
    const containerClasses = [
      styles.switchContainer,
      isChecked ? styles.switchChecked : '',
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
          role="switch"
          aria-checked={isChecked}
          className={styles.switchInput}
          checked={isChecked}
          disabled={disabled}
          onChange={onChange}
          {...props}
        />
        <div className={styles.switchTrack}>
          <div className={styles.switchThumb}>{thumbIcon}</div>
        </div>
        {label && <span>{label}</span>}
      </label>
    );
  }
);

Switch.displayName = 'Switch';

