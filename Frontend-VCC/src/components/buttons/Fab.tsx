import React from 'react';
import styles from './Fab.module.css';

export interface FabProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  size?: 'regular' | 'small' | 'large';
  icon: React.ReactNode;
  label?: string;
}

export const Fab = React.forwardRef<HTMLButtonElement, FabProps>(
  ({ size = 'regular', icon, label, className = '', disabled, ...props }, ref) => {
    const isExtended = Boolean(label);
    const sizeClass = isExtended ? styles.extended : styles[size];
    const combinedClassName = `${styles.fab} ${sizeClass} ${className}`.trim();

    return (
      <button
        ref={ref}
        type="button"
        className={combinedClassName}
        disabled={disabled}
        {...props}
      >
        <span className={styles.stateLayer} aria-hidden="true" />
        <span className={styles.icon}>{icon}</span>
        {label && <span className={styles.label}>{label}</span>}
      </button>
    );
  }
);

Fab.displayName = 'Fab';

