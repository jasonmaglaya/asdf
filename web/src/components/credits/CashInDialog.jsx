import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useEffect, useState } from "react";
import { Button, Container, InputGroup, Modal } from "react-bootstrap";
import { faCoins } from "@fortawesome/free-solid-svg-icons";
import CurrencyInput from "react-currency-input-field";
import SpinnerComponent from "../_shared/SpinnerComponent";
import { getBalance } from "../../services/creditsService";
import { set } from "react-hook-form";

export default function CashInDialog({ show, handleClose, currency, locale }) {
  const [credits, setCredits] = useState(0);
  const [isBusy, setIsBusy] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  const onValueChange = (amount) => {
    if (isNaN(amount) || amount > credits) {
      return;
    }
  };

  const cashIn = (data) => {
    var amount = Number(data.amount?.replace(/[^0-9.-]+/g, ""));
    if (amount > credits) {
      return;
    }

    setIsBusy(true);
  };

  const clearAmount = async () => {};

  const handleOnShow = () => {
    setIsLoading(true);

    const { operatorToken } = JSON.parse(localStorage.getItem("user"));

    getBalance(operatorToken)
      .then(({ data }) => {
        const { amount } = data.result;

        setCredits(amount);
      })
      .catch(() => {})
      .finally(() => {
        setIsLoading(false);
      });
  };

  useEffect(() => {
    const { operatorToken } = JSON.parse(localStorage.getItem("user"));
    getBalance(operatorToken)
      .then(({ data }) => {
        const { amount } = data.result;

        setCredits(amount);
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
                CREDITS:{" "}
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
                  className="form-control form-control-lg"
                  placeholder="ENTER AMOUNT"
                  decimalScale={2}
                  allowNegativeValue={false}
                  intlConfig={{ locale, currency }}
                />
                <Button variant="secondary" onClick={clearAmount}>
                  CLEAR
                </Button>
              </InputGroup>
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
        <Button variant="primary" size="lg" disabled={true}>
          {!isBusy ? (
            <span>CASH IN</span>
          ) : (
            <span>
              <span
                className="spinner-grow spinner-grow-sm"
                role="status"
              ></span>
              <span>PLEASE WAIT...</span>
            </span>
          )}
        </Button>
      </Modal.Footer>
    </Modal>
  );
}
