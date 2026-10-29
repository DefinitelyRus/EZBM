import React from 'react';
import styles from './Surfaces.module.css';

export interface AvatarProps extends React.HTMLAttributes<HTMLDivElement> {
  src?: string;
  alt?: string;
  initials?: string;
  size?: number;
}

export const Avatar: React.FC<AvatarProps> = ({
  src,
  alt = '',
  initials,
  size = 40,
  className = '',
  style,
  ...props
}) => {
  return (
    <div
      className={`${styles.avatar} ${className}`.trim()}
      style={{ width: size, height: size, ...style }}
      {...props}
    >
      {src ? (
        <img src={src} alt={alt} className={styles.avatarImage} />
      ) : (
        initials || '?'
      )}
    </div>
  );
};

