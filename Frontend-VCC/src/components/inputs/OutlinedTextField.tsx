import React, { useState } from 'react';
import styles from './TextField.module.css';
import { BaseTextFieldProps } from './FilledTextField';

export const OutlinedTextField = React.forwardRef<
  HTMLInputElement,
  BaseTextFieldProps
>(
  (
    {
      label,
      supportingText,
      error = false,
      leadingIcon,
      trailingIcon,
      value,
      defaultValue,
      className = '',
      disabled,
      placeholder,
      onFocus,
      onBlur,
      ...props
    },
    ref
  ) => {
    const [isFocused, setIsFocused] = useState(false);
    const [innerValue, setInnerValue] = useState(value ?? defaultValue ?? '');

    const handleFocus = (e: React.FocusEvent<HTMLInputElement>) => {
      setIsFocused(true);
      onFocus?.(e);
    };

    const handleBlur = (e: React.FocusEvent<HTMLInputElement>) => {
      setIsFocused(false);
      onBlur?.(e);
    };

    const isFloating =
      isFocused ||
      Boolean(value) ||
      Boolean(innerValue) ||
      Boolean(placeholder);

    const containerClasses = [
      styles.container,
      isFloating ? styles.outlinedFloating : '',
      error ? styles.error : '',
      disabled ? styles.disabled : '',
      className,
    ]
      .filter(Boolean)
      .join(' ');

    return (
      <div className={containerClasses}>
        <div className={`${styles.inputWrapper} ${styles.outlinedWrapper}`}>
          {leadingIcon && (
            <span className={styles.leadingIcon}>{leadingIcon}</span>
          )}
          <div style={{ flex: 1, position: 'relative', display: 'flex' }}>
            {label && (
              <label className={`${styles.label} ${styles.outlinedLabel}`}>
                {label}
              </label>
            )}
            <input
              ref={ref}
              className={styles.input}
              value={value}
              defaultValue={defaultValue}
              disabled={disabled}
              placeholder={placeholder}
              onFocus={handleFocus}
              onBlur={handleBlur}
              onChange={(e) => {
                setInnerValue(e.target.value);
                props.onChange?.(e);
              }}
              {...props}
            />
          </div>
          {trailingIcon && (
            <span className={styles.trailingIcon}>{trailingIcon}</span>
          )}
        </div>
        {supportingText && (
          <div className={styles.supportingText}>{supportingText}</div>
        )}
      </div>
    );
  }
);

OutlinedTextField.displayName = 'OutlinedTextField';

