import React from 'react';
import styles from './Surfaces.module.css';

export interface DividerProps extends React.HTMLAttributes<HTMLHRElement> {
  orientation?: 'horizontal' | 'vertical';
}

export const Divider: React.FC<DividerProps> = ({
  orientation = 'horizontal',
  className = '',
  ...props
}) => {
  const dividerClass =
    orientation === 'vertical'
      ? styles.dividerVertical
      : styles.dividerHorizontal;

  return (
    <hr
      className={`${dividerClass} ${className}`.trim()}
      aria-orientation={orientation}
      {...props}
    />
  );
};

