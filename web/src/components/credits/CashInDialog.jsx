import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useEffect, useRef, useState } from "react";
import { Button, Container, InputGroup, Modal } from "react-bootstrap";
import { useForm } from "react-hook-form";
import { faCoins } from "@fortawesome/free-solid-svg-icons";
import CurrencyInput from "react-currency-input-field";
import SpinnerComponent from "../_shared/SpinnerComponent";
import { getBalance } from "../../services/creditsService";

export default function CashInDialog({ show, handleClose, currency, locale }) {
  const {
    register,
    handleSubmit,
    setFocus,
    formState: { errors },
    reset,
    setError,
    clearErrors,
  } = useForm();

  const amountRef = useRef();

  const [credits, setCredits] = useState(0);
  const [isBusy, setIsBusy] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  const onValueChange = (amount) => {
    if (isNaN(amount) || amount > credits) {
      setError("amount");
      return;
    }

    clearErrors("amount");
  };

  const cashIn = (data) => {
    var amount = Number(data.amount?.replace(/[^0-9.-]+/g, ""));
    if (amount > credits) {
      setError("amount", { type: "focus" }, { shouldFocus: true });
      return;
    }

    setIsBusy(true);
  };

  const clearAmount = async () => {
    reset({ amount: "" });
    setFocus("amount");
  };

  const handleOnShow = () => {
    setIsLoading(true);

    const { operatorToken } = JSON.parse(localStorage.getItem("user"));

    getBalance(operatorToken)
      .then(({ data }) => {
        const { amount } = data.result;

        setCredits(amount);
      })
      .finally(() => {
        setIsLoading(false);
        setFocus("amount");
      });
  };

  useEffect(() => {
    const { operatorToken } = JSON.parse(localStorage.getItem("user"));
    getBalance(operatorToken)
      .then(({ data }) => {
        const { amount } = data.result;

        setCredits(amount);
      })
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
      onExited={reset}
    >
      <Modal.Header closeButton>
        <Modal.Title>CASH IN</Modal.Title>
      </Modal.Header>
      <form onSubmit={handleSubmit(cashIn)}>
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
                    autoComplete="off"
                    onValueChange={onValueChange}
                    className={
                      errors.amount
                        ? "form-control form-control-lg invalid"
                        : "form-control form-control-lg"
                    }
                    placeholder="ENTER AMOUNT"
                    decimalScale={2}
                    allowNegativeValue={false}
                    intlConfig={{ locale, currency }}
                    {...register("amount", {
                      required: true,
                      ref: amountRef,
                    })}
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
          <Button type="submit" variant="primary" size="lg" disabled={isBusy}>
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
      </form>
    </Modal>
  );
}
