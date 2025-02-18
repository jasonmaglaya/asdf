import { useState, useRef, useEffect, useContext } from "react";
import { useForm } from "react-hook-form";
import {
  Row,
  Col,
  Modal,
  Container,
  Button,
  InputGroup,
} from "react-bootstrap";
import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCoins } from "@fortawesome/free-solid-svg-icons";
import CurrencyInput from "react-currency-input-field";
import { addBet } from "../../services/matchesService";
import { useDispatch, useSelector } from "react-redux";
import { setCredits } from "../../store/userSlice";
import { BetsContext } from "../_shared/BetsContext";

const StyledButton = styled(Button)`
  margin: 3px;
  color: #fff;
  background-color: #202428;
  box-shadow: 1px 1px #06be59;

  &:disabled {
    color: #ffffffb3;
    background-color: #202428;
    box-shadow: 1px 1px #06be59;
    opacity: 0.5;
  }
`;

export default function PlaceBetDialog({
  matchId,
  color,
  name,
  code,
  minimumBet,
  maximumBet,
  disabled,
  currency,
  locale,
}) {
  const [show, setShow] = useState(false);
  const [isBusy, setIsBusy] = useState(false);
  const dispatch = useDispatch();
  const { credits } = useSelector((state) => state.user);

  const {
    register,
    handleSubmit,
    setFocus,
    setValue,
    formState: { errors },
    reset,
    setError,
    clearErrors,
  } = useForm();

  const [, setBets] = useContext(BetsContext);
  const [balance, setBalance] = useState(credits);
  const amountRef = useRef();

  const onValueChange = (amount) => {
    if (
      isNaN(amount) ||
      amount < minimumBet ||
      amount > maximumBet ||
      amount > credits
    ) {
      setBalance(credits);
      setError("amount");
      return;
    }
    clearErrors("amount");
    setBalance(credits - amount);
  };

  const handleClose = () => setShow(false);
  const handleShow = () => setShow(true);
  const handleOnShow = () => {
    setFocus("amount");
  };
  const backgroundColor = color;

  const placeBet = (data) => {
    var amount = Number(data.amount?.replace(/[^0-9.-]+/g, ""));
    if (amount < minimumBet || amount > maximumBet || amount > credits) {
      setError("amount", { type: "focus" }, { shouldFocus: true });
      return;
    }

    setIsBusy(true);
    addBet(matchId, code, amount)
      .then(async ({ data }) => {
        dispatch(setCredits(data.credits));
        setBalance(data.credits);
        setBets(data.bets);
        await reset({ amount: "" });
        clearErrors("amount");
      })
      .finally(() => {
        setShow(false);
        setIsBusy(false);
      });
  };

  const setAmount = async (amount) => {
    await setValue(
      "amount",
      amount.toLocaleString(locale || "en-US", {
        style: "currency",
        currency: currency || "USD",
      }),
      {
        shouldValidate: true,
        shouldDirty: true,
      }
    );

    setBalance(credits - amount);
  };

  const clearAmount = async () => {
    await reset({ amount: "" });
    setFocus("amount");
    setBalance(credits);
  };

  const presetAmounts = [
    { name: "50", amount: 50 },
    { name: "100", amount: 100 },
    { name: "500", amount: 500 },
    { name: "1K", amount: 1000 },
    { name: "2K", amount: 2000 },
    { name: "5K", amount: 5000 },
    { name: "10K", amount: 10000 },
  ];

  useEffect(() => {
    setBalance(credits);
  }, [credits]);

  return (
    <>
      <Button
        variant="dark"
        size="lg"
        style={{ backgroundColor }}
        className="form-control text-uppercase"
        onClick={handleShow}
        disabled={disabled}
      >
        {name}
      </Button>
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
        <form onSubmit={handleSubmit(placeBet)}>
          <Modal.Body className="p-0 pt-3">
            <Container>
              <h5>
                <i style={{ color: "gold" }}>
                  <FontAwesomeIcon icon={faCoins} />
                </i>{" "}
                BALANCE:{" "}
                <span className="text-success">
                  {balance?.toLocaleString(locale || "en-US", {
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
            <Container>
              <Row className="mb-2">
                <Col className="d-flex flex-wrap">
                  {presetAmounts.map(({ name, amount }) => {
                    return (
                      <StyledButton
                        key={name}
                        size="lg"
                        onClick={() => setAmount(amount)}
                        disabled={credits < amount || disabled}
                      >
                        {name}
                      </StyledButton>
                    );
                  })}
                  <StyledButton
                    size="lg"
                    onClick={() => setAmount(credits)}
                    disabled={disabled || credits > maximumBet}
                  >
                    ALL IN
                  </StyledButton>
                </Col>
              </Row>
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
              type="submit"
              variant="dark"
              size="lg"
              style={{ backgroundColor }}
              disabled={credits === balance || isBusy}
            >
              {!isBusy ? (
                <span>BET {name?.toUpperCase()}</span>
              ) : (
                <span>
                  <span
                    className="spinner-grow spinner-grow-sm"
                    role="status"
                  ></span>
                  <span>PLACING BET...</span>
                </span>
              )}
            </Button>
          </Modal.Footer>
        </form>
      </Modal>
    </>
  );
}
