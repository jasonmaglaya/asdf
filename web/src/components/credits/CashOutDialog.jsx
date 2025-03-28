import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useState } from "react";
import { Button, Container, Modal } from "react-bootstrap";
import { faCoins } from "@fortawesome/free-solid-svg-icons";
import { cashOut } from "../../services/creditsService";
import { useDispatch, useSelector } from "react-redux";
import {
  setErrorMessages,
  setSuccessMessages,
} from "../../store/messagesSlice";
import { setCredits } from "../../store/userSlice";

export default function CashOutDialog({ show, handleClose, currency, locale }) {
  const [isBusy, setIsBusy] = useState(false);
  const dispatch = useDispatch();
  const { credits } = useSelector((state) => state.user);

  const processCashOut = () => {
    setIsBusy(true);

    const userString = localStorage.getItem("user")?.toString();
    if (!userString) {
      return;
    }

    const { operatorToken } = JSON.parse(userString);

    cashOut(operatorToken)
      .then(({ data }) => {
        dispatch(setSuccessMessages(["Cash out successful."]));
        dispatch(setCredits(data.newBalance));
        handleClose();
      })
      .catch(() => {
        dispatch(setErrorMessages(["Unable to cash out."]));
      })
      .finally(() => {
        setIsBusy(false);
      });
  };

  return (
    <Modal
      show={show}
      onHide={handleClose}
      backdrop="static"
      keyboard={false}
      centered
      animation={false}
    >
      <Modal.Header closeButton>
        <Modal.Title>CASH OUT</Modal.Title>
      </Modal.Header>
      <Modal.Body className="p-0 pt-3">
        <Container className="mb-4">
          <h5>
            <i style={{ color: "gold" }}>
              <FontAwesomeIcon icon={faCoins} />
            </i>{" "}
            BALANCE:{" "}
            <span className="text-success">
              {credits?.toLocaleString(locale || "en-US", {
                style: "currency",
                currency: currency || "USD",
              })}
            </span>
          </h5>
        </Container>
      </Modal.Body>
      <Modal.Footer>
        <Button
          size="lg"
          variant="secondary"
          onClick={handleClose}
          disabled={isBusy}
        >
          CANCEL
        </Button>
        <Button
          variant="primary"
          size="lg"
          onClick={processCashOut}
          disabled={credits === 0 || isBusy}
        >
          {!isBusy ? (
            <span>CASH OUT</span>
          ) : (
            <span>
              <span
                className="spinner-grow spinner-grow-sm"
                role="status"
              ></span>
              <span>CASHING OUT...</span>
            </span>
          )}
        </Button>
      </Modal.Footer>
    </Modal>
  );
}
