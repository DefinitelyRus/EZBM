import React from 'react';
import styles from './Feedback.module.css';

export interface CircularProgressProps {
  size?: number;
  strokeWidth?: number;
  value?: number; // 0 to 100
  indeterminate?: boolean;
  className?: string;
}

export const CircularProgress: React.FC<CircularProgressProps> = ({
  size = 48,
  strokeWidth = 4,
  value = 0,
  indeterminate = false,
  className = '',
}) => {
  const center = size / 2;
  const radius = (size - strokeWidth) / 2;
  const circumference = 2 * Math.PI * radius;
  const strokeDashoffset = circumference - (value / 100) * circumference;

  return (
    <svg
      className={`${styles.circular} ${indeterminate ? styles.circularIndeterminate : ''} ${className}`.trim()}
      width={size}
      height={size}
      viewBox={`0 0 ${size} ${size}`}
    >
      <circle
        className={styles.circularTrack}
        cx={center}
        cy={center}
        r={radius}
        strokeWidth={strokeWidth}
        fill="none"
      />
      <circle
        className={styles.circularIndicator}
        cx={center}
        cy={center}
        r={radius}
        strokeWidth={strokeWidth}
        fill="none"
        strokeLinecap="round"
        strokeDasharray={circumference}
        strokeDashoffset={indeterminate ? undefined : strokeDashoffset}
      />
    </svg>
  );
};

export interface LinearProgressProps {
  value?: number; // 0 to 100
  indeterminate?: boolean;
  className?: string;
}

export const LinearProgress: React.FC<LinearProgressProps> = ({
  value = 0,
  indeterminate = false,
  className = '',
}) => {
  return (
    <div className={`${styles.linear} ${className}`.trim()} role="progressbar">
      {indeterminate ? (
        <>
          <div className={styles.linearIndeterminate1} />
          <div className={styles.linearIndeterminate2} />
        </>
      ) : (
        <div
          className={styles.linearActive}
          style={{ width: `${Math.min(100, Math.max(0, value))}%` }}
        />
      )}
    </div>
  );
};

