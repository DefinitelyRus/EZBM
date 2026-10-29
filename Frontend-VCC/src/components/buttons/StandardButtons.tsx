import React from 'react';
import { ButtonBase, ButtonBaseProps } from './ButtonBase';
import styles from './Buttons.module.css';

export interface ButtonProps extends Omit<ButtonBaseProps, 'variantClass'> {}

export const FilledButton = React.forwardRef<HTMLButtonElement, ButtonProps>(
  (props, ref) => {
    return <ButtonBase ref={ref} variantClass={styles.filled} {...props} />;
  }
);
FilledButton.displayName = 'FilledButton';

export const ElevatedButton = React.forwardRef<HTMLButtonElement, ButtonProps>(
  (props, ref) => {
    return <ButtonBase ref={ref} variantClass={styles.elevated} {...props} />;
  }
);
ElevatedButton.displayName = 'ElevatedButton';

export const FilledTonalButton = React.forwardRef<
  HTMLButtonElement,
  ButtonProps
>((props, ref) => {
  return <ButtonBase ref={ref} variantClass={styles.filledTonal} {...props} />;
});
FilledTonalButton.displayName = 'FilledTonalButton';

export const OutlinedButton = React.forwardRef<HTMLButtonElement, ButtonProps>(
  (props, ref) => {
    return <ButtonBase ref={ref} variantClass={styles.outlined} {...props} />;
  }
);
OutlinedButton.displayName = 'OutlinedButton';

export const TextButton = React.forwardRef<HTMLButtonElement, ButtonProps>(
  (props, ref) => {
    return <ButtonBase ref={ref} variantClass={styles.text} {...props} />;
  }
);
TextButton.displayName = 'TextButton';

