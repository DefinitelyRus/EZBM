import React from 'react';
import styles from './Feedback.module.css';

export interface DialogProps {
  open: boolean;
  onClose: () => void;
  title?: React.ReactNode;
  children?: React.ReactNode;
  actions?: React.ReactNode;
  className?: string;
}

export const Dialog: React.FC<DialogProps> = ({
  open,
  onClose,
  title,
  children,
  actions,
  className = '',
}) => {
  if (!open) return null;

  return (
    <div className={styles.dialogBackdrop} onClick={onClose}>
      <div
        className={`${styles.dialogContainer} ${className}`.trim()}
        onClick={(e) => e.stopPropagation()}
        role="dialog"
      >
        {title && <div className={styles.dialogTitle}>{title}</div>}
        {children && <div className={styles.dialogContent}>{children}</div>}
        {actions && <div className={styles.dialogActions}>{actions}</div>}
      </div>
    </div>
  );
};

