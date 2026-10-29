import React from 'react';
import styles from './Surfaces.module.css';

export interface BadgeProps extends React.HTMLAttributes<HTMLSpanElement> {
  value?: number | string;
  children?: React.ReactNode;
}

export const Badge: React.FC<BadgeProps> = ({
  value,
  children,
  className = '',
  ...props
}) => {
  const isSmall = value === undefined || value === null;
  const badgeStyle = isSmall ? styles.badgeSmall : styles.badgeLarge;

  const badgeElement = (
    <span
      className={`${styles.badge} ${badgeStyle} ${children ? styles.badgePositioned : ''} ${className}`.trim()}
      {...props}
    >
      {!isSmall && value}
    </span>
  );

  if (!children) return badgeElement;

  return (
    <div className={styles.badgeContainer}>
      {children}
      {badgeElement}
    </div>
  );
};

