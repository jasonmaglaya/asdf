import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useEffect, useState } from "react";
import { Button, Container, Modal } from "react-bootstrap";
import { faCoins } from "@fortawesome/free-solid-svg-icons";
import CurrencyInput from "react-currency-input-field";
import SpinnerComponent from "../_shared/SpinnerComponent";
import { getBalance, cashIn } from "../../services/creditsService";
import { useDispatch } from "react-redux";
import {
  setErrorMessages,
  setSuccessMessages,
} from "../../store/messagesSlice";
import { setCredits } from "../../store/userSlice";

export default function CashInDialog({ show, handleClose, currency, locale }) {
  const [isBusy, setIsBusy] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [hasError, setHasError] = useState(false);
  const dispatch = useDispatch();
  const [balance, setBalance] = useState(0);
  const [amount, setAmount] = useState();

  const onValueChange = (amount) => {
    if (isNaN(amount) || amount > balance || amount === 0) {
      setAmount(0);
      setHasError(true);
      return;
    }

    setHasError(false);
    setAmount(amount);
  };

  const handleOnShow = () => {
    setIsLoading(true);

    const { operatorToken } = JSON.parse(localStorage.getItem("user"));

    getBalance(operatorToken)
      .then(({ data }) => {
        setBalance(data.result.amount);
      })
      .catch(() => {
        dispatch(setErrorMessages(["Unable to get the balance."]));
      })
      .finally(() => {
        setAmount(0);
        setIsLoading(false);
      });
  };

  const processCashIn = () => {
    if (isNaN(amount) || amount === 0 || amount > balance) {
      dispatch(setErrorMessages(["Invalid amount."]));
      return;
    }

    setIsBusy(true);

    const { operatorToken } = JSON.parse(localStorage.getItem("user"));

    cashIn(operatorToken, amount, currency)
      .then(({ data }) => {
        dispatch(setSuccessMessages(["Cash in successful."]));
        dispatch(setCredits(data.newBalance));
        handleClose();
      })
      .catch(() => {
        dispatch(setErrorMessages(["Unable to cash in."]));
      })
      .finally(() => {
        setIsBusy(false);
      });
  };

  useEffect(() => {
    const { operatorToken } = JSON.parse(localStorage.getItem("user"));
    getBalance(operatorToken)
      .then(({ data }) => {
        setBalance(data.result.amount);
      })
      .catch(() => {})
      .finally(() => {
        setIsLoading(false);
      });
  }, []);

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
        <Modal.Title>CASH IN</Modal.Title>
      </Modal.Header>
      <Modal.Body className="p-0 pt-3">
        {isLoading ? (
          <SpinnerComponent />
        ) : (
          <>
            <Container className="mb-4">
              <h5>
                <i style={{ color: "gold" }}>
                  <FontAwesomeIcon icon={faCoins} />
                </i>{" "}
                MAIN BALANCE:{" "}
                <span className="text-success">
                  {balance?.toLocaleString(locale || "en-US", {
                    style: "currency",
                    currency: currency || "USD",
                  })}
                </span>
              </h5>
            </Container>
            <Container className="mb-2">
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
                autoFocus
              />
            </Container>
          </>
        )}
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
          onClick={processCashIn}
          disabled={isNaN(amount) || amount === 0 || balance === 0 || isBusy}
        >
          {!isBusy ? (
            <span>CASH IN</span>
          ) : (
            <span>
              <span
                className="spinner-grow spinner-grow-sm"
                role="status"
              ></span>
              <span>CASHING IN...</span>
            </span>
          )}
        </Button>
      </Modal.Footer>
    </Modal>
  );
}
