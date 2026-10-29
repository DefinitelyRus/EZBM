import React from 'react';
import styles from './ButtonBase.module.css';

export interface ButtonBaseProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  children?: React.ReactNode;
  icon?: React.ReactNode;
  trailingIcon?: React.ReactNode;
  variantClass?: string;
}

export const ButtonBase = React.forwardRef<HTMLButtonElement, ButtonBaseProps>(
  (
    {
      children,
      icon,
      trailingIcon,
      variantClass = '',
      className = '',
      disabled,
      ...props
    },
    ref
  ) => {
    const combinedClassName = `${styles.base} ${variantClass} ${className}`.trim();

    return (
      <button
        ref={ref}
        type="button"
        className={combinedClassName}
        disabled={disabled}
        {...props}
      >
        <span className={styles.stateLayer} aria-hidden="true" />
        {icon && <span className={styles.icon}>{icon}</span>}
        {children && <span className={styles.label}>{children}</span>}
        {trailingIcon && (
          <span className={styles.trailingIcon}>{trailingIcon}</span>
        )}
      </button>
    );
  }
);

ButtonBase.displayName = 'ButtonBase';

