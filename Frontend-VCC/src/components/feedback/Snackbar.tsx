import React from 'react';
import styles from './Feedback.module.css';

export interface SnackbarProps {
  open: boolean;
  message: string;
  actionLabel?: string;
  onAction?: () => void;
  className?: string;
}

export const Snackbar: React.FC<SnackbarProps> = ({
  open,
  message,
  actionLabel,
  onAction,
  className = '',
}) => {
  if (!open) return null;

  return (
    <div className={`${styles.snackbar} ${className}`.trim()} role="status">
      <span>{message}</span>
      {actionLabel && (
        <button
          type="button"
          className={styles.snackbarAction}
          onClick={onAction}
        >
          {actionLabel}
        </button>
      )}
    </div>
  );
};

