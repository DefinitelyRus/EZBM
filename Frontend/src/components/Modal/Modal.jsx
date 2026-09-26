import "./Modal.css";

function Modal({
  isOpen,
  title,
  message,
  confirmText = "Confirm",
  cancelText = "Cancel",
  onConfirm,
  onCancel,
}) {
  if (!isOpen) return null;

  return (
    <div className="modal-overlay">
      <div className="custom-modal">
        <h4>{title}</h4>

        <p>{message}</p>

        <div className="modal-actions">
          <button className="btn confirm" onClick={onConfirm}>
            {confirmText}
          </button>

          <button className="btn cancel" onClick={onCancel}>
            {cancelText}
          </button>
        </div>
      </div>
    </div>
  );
}

export default Modal;
