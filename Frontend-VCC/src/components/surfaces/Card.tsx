import React from 'react';
import styles from './Surfaces.module.css';

export interface CardProps extends React.HTMLAttributes<HTMLDivElement> {
  children?: React.ReactNode;
}

export const ElevatedCard: React.FC<CardProps> = ({
  className = '',
  children,
  ...props
}) => (
  <div className={`${styles.card} ${styles.elevatedCard} ${className}`.trim()} {...props}>
    {children}
  </div>
);

export const FilledCard: React.FC<CardProps> = ({
  className = '',
  children,
  ...props
}) => (
  <div className={`${styles.card} ${styles.filledCard} ${className}`.trim()} {...props}>
    {children}
  </div>
);

export const OutlinedCard: React.FC<CardProps> = ({
  className = '',
  children,
  ...props
}) => (
  <div className={`${styles.card} ${styles.outlinedCard} ${className}`.trim()} {...props}>
    {children}
  </div>
);

