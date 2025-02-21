import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useRef, useState } from "react";
import { Button, Container, InputGroup, Modal } from "react-bootstrap";
import { faCoins } from "@fortawesome/free-solid-svg-icons";
import CurrencyInput from "react-currency-input-field";
import { cashOut } from "../../services/creditsService";
import { useDispatch, useSelector } from "react-redux";
import { setErrorMessages } from "../../store/errorMessagesSlice";

export default function CashOutDialog({ show, handleClose, currency, locale }) {
  const [isBusy, setIsBusy] = useState(false);
  const [hasError, setHasError] = useState(true);
  const dispatch = useDispatch();
  const amountRef = useRef();
  const { credits } = useSelector((state) => state.user);
  const [amount, setAmount] = useState();

  const onValueChange = (amount) => {
    if (isNaN(amount) || amount > credits) {
      setAmount(0);
      setHasError(true);
      return;
    }

    setHasError(false);
    setAmount(amount);
  };

  const clearAmount = () => {
    setAmount(0);
    setHasError(false);
  };

  const handleOnShow = () => {
    setTimeout(() => amountRef.current.focus(), 0);
  };

  const processCashOut = () => {
    if (amount > credits) {
      dispatch(setErrorMessages(["Invalid amount."]));
      return;
    }

    setIsBusy(true);

    const { operatorToken } = JSON.parse(localStorage.getItem("user"));

    cashOut(operatorToken, amount, currency)
      .then(() => {
        // notify user
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
      onShow={handleOnShow}
    >
      <Modal.Header closeButton>
        <Modal.Title>CASH OUT</Modal.Title>
      </Modal.Header>
      <Modal.Body className="p-0 pt-3">
        <>
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
          <Container className="mb-2">
            <InputGroup>
              <CurrencyInput
                onValueChange={onValueChange}
                autoComplete="off"
                className={
                  hasError
                    ? "form-control form-control-lg invalid"
                    : "form-control form-control-lg"
                }
                placeholder="ENTER AMOUNT"
                decimalScale={2}
                allowNegativeValue={false}
                intlConfig={{ locale, currency }}
                ref={amountRef}
              />
              <Button variant="secondary" onClick={clearAmount}>
                CLEAR
              </Button>
            </InputGroup>
          </Container>
        </>
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
