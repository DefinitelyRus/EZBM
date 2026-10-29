import React from 'react';
import styles from './Controls.module.css';

export interface SliderProps
  extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'type' | 'onChange'> {
  value: number;
  min?: number;
  max?: number;
  step?: number;
  onChange?: (value: number) => void;
  label?: string;
}

export const Slider = React.forwardRef<HTMLInputElement, SliderProps>(
  (
    {
      value,
      min = 0,
      max = 100,
      step = 1,
      onChange,
      label,
      disabled,
      className = '',
      ...props
    },
    ref
  ) => {
    const percentage = Math.min(
      100,
      Math.max(0, ((value - min) / (max - min)) * 100)
    );

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
      onChange?.(Number(e.target.value));
    };

    const containerClasses = [
      styles.sliderContainer,
      disabled ? styles.disabled : '',
      className,
    ]
      .filter(Boolean)
      .join(' ');

    return (
      <div className={containerClasses}>
        {label && (
          <div
            style={{
              display: 'flex',
              justifyContent: 'space-between',
              fontFamily: 'var(--md-sys-typescale-label-medium-font)',
              fontSize: 'var(--md-sys-typescale-label-medium-size)',
            }}
          >
            <span>{label}</span>
            <span>{value}</span>
          </div>
        )}
        <div className={styles.sliderTrackWrapper}>
          <div className={styles.sliderInactiveTrack} />
          <div
            className={styles.sliderActiveTrack}
            style={{ width: `${percentage}%` }}
          />
          <div
            className={styles.sliderThumb}
            style={{ left: `${percentage}%` }}
          />
          <input
            ref={ref}
            type="range"
            min={min}
            max={max}
            step={step}
            value={value}
            disabled={disabled}
            className={styles.sliderInput}
            onChange={handleChange}
            {...props}
          />
        </div>
      </div>
    );
  }
);

Slider.displayName = 'Slider';

