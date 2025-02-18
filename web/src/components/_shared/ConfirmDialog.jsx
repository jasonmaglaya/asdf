import { Modal, Button } from "react-bootstrap";

export default function ConfirmDialog({
  title,
  confirmButtonText,
  confirmButtonTextBusy,
  cancelButtonText,
  handleConfirm,
  handleClose,
  show,
  children,
  isBusy,
}) {
  return (
    <Modal
      show={show}
      backdrop="static"
      keyboard={false}
      centered
      animation={false}
    >
      <Modal.Header>
        <Modal.Title>{title || "Confirm"}</Modal.Title>
      </Modal.Header>
      <Modal.Body>{children}</Modal.Body>
      <Modal.Footer>
        <Button
          size="lg"
          variant="primary"
          onClick={handleConfirm}
          className="text-uppercase"
          disabled={isBusy}
        >
          {!isBusy ? (
            <span>{confirmButtonText || "OK"}</span>
          ) : (
            <span>
              <span
                className="spinner-grow spinner-grow-sm"
                role="status"
              ></span>
              <span>{confirmButtonTextBusy || confirmButtonText}</span>
            </span>
          )}
        </Button>
        <Button
          size="lg"
          variant="secondary"
          onClick={handleClose}
          className="text-uppercase"
          disabled={isBusy}
        >
          {cancelButtonText || "Cancel"}
        </Button>
      </Modal.Footer>
    </Modal>
  );
}
