import React from 'react';
import styles from './Typography.module.css';

type ScaleSize = 'large' | 'medium' | 'small';

interface BaseTypographyProps extends React.HTMLAttributes<HTMLElement> {
  size?: ScaleSize;
  as?: React.ElementType;
}

export const Display: React.FC<BaseTypographyProps> = ({
  size = 'large',
  as: Component = 'h1',
  className = '',
  children,
  ...props
}) => {
  const sizeMap: Record<ScaleSize, string> = {
    large: styles.displayLarge,
    medium: styles.displayMedium,
    small: styles.displaySmall,
  };
  return (
    <Component className={`${sizeMap[size]} ${className}`.trim()} {...props}>
      {children}
    </Component>
  );
};

export const Headline: React.FC<BaseTypographyProps> = ({
  size = 'large',
  as: Component = 'h2',
  className = '',
  children,
  ...props
}) => {
  const sizeMap: Record<ScaleSize, string> = {
    large: styles.headlineLarge,
    medium: styles.headlineMedium,
    small: styles.headlineSmall,
  };
  return (
    <Component className={`${sizeMap[size]} ${className}`.trim()} {...props}>
      {children}
    </Component>
  );
};

export const Title: React.FC<BaseTypographyProps> = ({
  size = 'large',
  as: Component = 'h3',
  className = '',
  children,
  ...props
}) => {
  const sizeMap: Record<ScaleSize, string> = {
    large: styles.titleLarge,
    medium: styles.titleMedium,
    small: styles.titleSmall,
  };
  return (
    <Component className={`${sizeMap[size]} ${className}`.trim()} {...props}>
      {children}
    </Component>
  );
};

export const Body: React.FC<BaseTypographyProps> = ({
  size = 'large',
  as: Component = 'p',
  className = '',
  children,
  ...props
}) => {
  const sizeMap: Record<ScaleSize, string> = {
    large: styles.bodyLarge,
    medium: styles.bodyMedium,
    small: styles.bodySmall,
  };
  return (
    <Component className={`${sizeMap[size]} ${className}`.trim()} {...props}>
      {children}
    </Component>
  );
};

export const Label: React.FC<BaseTypographyProps> = ({
  size = 'large',
  as: Component = 'span',
  className = '',
  children,
  ...props
}) => {
  const sizeMap: Record<ScaleSize, string> = {
    large: styles.labelLarge,
    medium: styles.labelMedium,
    small: styles.labelSmall,
  };
  return (
    <Component className={`${sizeMap[size]} ${className}`.trim()} {...props}>
      {children}
    </Component>
  );
};

