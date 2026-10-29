import React, { useRef } from 'react';
import { Search, X } from 'lucide-react';
import styles from './SearchField.module.css';

export interface SearchFieldProps
  extends React.InputHTMLAttributes<HTMLInputElement> {
  onClear?: () => void;
}

export const SearchField = React.forwardRef<
  HTMLInputElement,
  SearchFieldProps
>(({ className = '', value, onChange, onClear, placeholder = 'Search...', ...props }, ref) => {
  const localRef = useRef<HTMLInputElement>(null);
  const inputRef = (ref as React.RefObject<HTMLInputElement>) || localRef;

  const handleClear = () => {
    if (onClear) {
      onClear();
    } else if (inputRef.current) {
      inputRef.current.value = '';
      const event = new Event('input', { bubbles: true });
      inputRef.current.dispatchEvent(event);
    }
  };

  const showClear = Boolean(value);

  return (
    <div className={`${styles.searchBar} ${className}`.trim()}>
      <span className={styles.icon}>
        <Search size={20} />
      </span>
      <input
        ref={inputRef}
        type="search"
        className={styles.input}
        placeholder={placeholder}
        value={value}
        onChange={onChange}
        {...props}
      />
      {showClear && (
        <button
          type="button"
          className={styles.clearButton}
          onClick={handleClear}
          aria-label="Clear search"
        >
          <X size={18} />
        </button>
      )}
    </div>
  );
});

SearchField.displayName = 'SearchField';

