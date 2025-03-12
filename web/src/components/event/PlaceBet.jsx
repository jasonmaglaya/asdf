import { useState, useRef, useEffect, useContext } from "react";
import { useForm } from "react-hook-form";
import { Row, Col, Button, Card, InputGroup } from "react-bootstrap";
import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCoins } from "@fortawesome/free-solid-svg-icons";
import CurrencyInput from "react-currency-input-field";
import ConfirmDialog from "../_shared/ConfirmDialog";
import { addBet } from "../../services/matchesService";
import { BetsContext } from "../_shared/BetsContext";
import { StyledCard } from "../_shared/StyledCard";
import { useDispatch, useSelector } from "react-redux";
import { setCredits } from "../../store/userSlice";
import { setErrorMessages } from "../../store/messagesSlice";

const StyledButton = styled(Button)`
  margin: 3px;
  color: #fff;
  background-color: #202428;
  box-shadow: 1px 1px #06be59;

  :disabled {
    color: #ffffffb3;
    background-color: #202428;
    box-shadow: 1px 1px #06be59;
    opacity: 0.5;
  }
`;

export default function PlaceBet({
  matchId,
  teams,
  minimumBet,
  maximumBet,
  disabled,
  currency,
  locale,
}) {
  const dispatch = useDispatch();
  const { credits } = useSelector((state) => state.user);
  const [, setBets] = useContext(BetsContext);
  const [balance, setBalance] = useState(credits);
  const amountRef = useRef();
  const betForm = useForm();
  const [selectedTeam, setSelectedTeam] = useState(null);
  const [showConfirmBetDialog, setShowConfirmBetDialog] = useState(false);
  const [bet, setBet] = useState("");
  const [isBusy, setIsBusy] = useState();

  const onValueChange = (amount) => {
    if (
      isNaN(amount) ||
      amount < minimumBet ||
      amount > maximumBet ||
      amount > credits
    ) {
      setBalance(credits);
      betForm.setError("amount");
      return;
    }
    betForm.clearErrors("amount");

    setBalance(credits - amount);
  };

  const setAmount = async (amount) => {
    if (amount > credits) {
      return;
    }

    betForm.setValue(
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

  const handlePlaceBet = async (team) => {
    setSelectedTeam(team);
    await betForm.handleSubmit(confirmBet)();
  };

  const confirmBet = (data) => {
    var amount = Number(data.amount?.replace(/[^0-9.-]+/g, ""));
    if (
      isNaN(amount) ||
      amount < minimumBet ||
      amount > maximumBet ||
      amount > credits
    ) {
      betForm.setError("amount", { type: "focus" }, { shouldFocus: true });
      return;
    }

    betForm.clearErrors("amount");
    setBet(data.amount);
    setShowConfirmBetDialog(true);
  };

  const placeBet = async () => {
    setIsBusy(true);
    var amount = Number(bet?.replace(/[^0-9.-]+/g, ""));
    addBet(matchId, selectedTeam.code, amount)
      .then(async ({ data }) => {
        dispatch(setCredits(data.credits));
        setBalance(data.credits);
        setBets(data.bets);
        betForm.reset({ amount: "" });
        betForm.clearErrors("amount");
      })
      .catch(({ response }) => {
        const errors = [
          ...(response?.data?.validationResults || []),
          ...(response?.data?.errors || []),
        ];
        dispatch(setErrorMessages(errors));
      })
      .finally(() => {
        setIsBusy(false);
      });
  };

  const clearAmount = async () => {
    await betForm.reset({ amount: "" });
    betForm.setFocus("amount");
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
      <StyledCard bg="dark" text="white">
        <Card.Body className="p-1">
          <div>
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
          </div>
          <div>
            <InputGroup className="mb-1">
              <CurrencyInput
                autoComplete="off"
                onValueChange={onValueChange}
                className={
                  betForm.formState.errors?.amount
                    ? "form-control form-control-lg invalid"
                    : "form-control form-control-lg"
                }
                placeholder="ENTER AMOUNT"
                decimalScale={2}
                allowNegativeValue={false}
                intlConfig={{ locale, currency }}
                {...betForm.register("amount", {
                  required: true,
                  ref: amountRef,
                })}
                disabled={disabled || credits <= 0}
              />
              <Button
                variant="secondary"
                onClick={clearAmount}
                disabled={disabled || credits <= 0}
              >
                CLEAR
              </Button>
            </InputGroup>
            <Row className="mb-1">
              <Col className="d-flex flex-wrap justify-content-center">
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
                  disabled={
                    disabled ||
                    credits > maximumBet ||
                    credits < (minimumBet || 1)
                  }
                >
                  ALL IN
                </StyledButton>
              </Col>
            </Row>
          </div>
          <Row className="gx-1 gy-1" xs={2} lg={2}>
            {teams?.map((t) => {
              const { name, color: backgroundColor } = t;
              return (
                <Col key={name} className="col-xs-auto">
                  <Button
                    variant="dark"
                    size="lg"
                    className="form-control text-uppercase"
                    style={{ backgroundColor }}
                    onClick={() => handlePlaceBet(t)}
                    disabled={disabled || credits < minimumBet}
                  >
                    {name}
                  </Button>
                </Col>
              );
            })}
          </Row>
        </Card.Body>
      </StyledCard>
      <ConfirmDialog
        confirmButtonText="Place Bet"
        confirmButtonTextBusy="Placing Bet..."
        handleConfirm={() => {
          setShowConfirmBetDialog(false);
          placeBet();
        }}
        handleClose={() => {
          setShowConfirmBetDialog(false);
        }}
        show={showConfirmBetDialog}
        isBusy={isBusy}
      >
        <span className="h5">
          Bet <span className="h4">{bet}</span> to{" "}
          <span
            className="h4 text-uppercase"
            style={{ color: selectedTeam?.color }}
          >
            {selectedTeam?.name}
          </span>
          ?
        </span>
      </ConfirmDialog>
    </>
  );
}
