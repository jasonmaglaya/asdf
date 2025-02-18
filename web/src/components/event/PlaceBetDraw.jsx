import { useRef, useContext, useState, useEffect } from "react";
import { Card, Row, Col, InputGroup, Button } from "react-bootstrap";
import CurrencyInput from "react-currency-input-field";
import { MatchContext } from "../_shared/MatchContext";
import { BetsContext } from "../_shared/BetsContext";
import { useForm } from "react-hook-form";
import { addBet } from "../../services/matchesService";
import ConfirmDialog from "../_shared/ConfirmDialog";
import { StyledCard } from "../_shared/StyledCard";
import { useDispatch, useSelector } from "react-redux";
import { setCredits } from "../../store/userSlice";
import { animate } from "framer-motion";

export default function PlaceBetDraw({
  minimumBet,
  maxDrawBet,
  drawMultiplier,
  disabled,
  currency,
  locale,
  isRolling,
}) {
  const drawAmountRef = useRef();
  const drawForm = useForm();
  const dispatch = useDispatch();
  const { credits } = useSelector((state) => state.user);
  const { match, totalBets } = useContext(MatchContext);
  const [bets, setBets] = useContext(BetsContext);
  const [selectedTeam, setSelectedTeam] = useState(null);
  const [showConfirmBetDialog, setShowConfirmBetDialog] = useState(false);
  const [bet, setBet] = useState("");
  const [isBusy, setIsBusy] = useState();

  const onValueChangeDraw = (amount) => {
    if (isNaN(amount) || amount < minimumBet || amount > credits) {
      drawForm.setError("amount");
      return;
    }

    drawForm.clearErrors("amount");
  };

  const clearDrawAmount = async () => {
    await drawForm.reset({ amount: "" });
    drawForm.setFocus("amount");
  };

  const handlePlaceDrawBet = async (team) => {
    setSelectedTeam(team);
    await drawForm.handleSubmit(confirmBetDraw)();
  };

  const confirmBetDraw = (data) => {
    var amount = Number(data.amount?.replace(/[^0-9.-]+/g, ""));
    if (
      isNaN(amount) ||
      amount < minimumBet ||
      amount > credits ||
      amount > maxDrawBet
    ) {
      drawForm.setError("amount", { type: "focus" }, { shouldFocus: true });
      return;
    }

    drawForm.clearErrors("amount");
    setBet(data.amount);
    setShowConfirmBetDialog(true);
  };

  const placeBet = async () => {
    setIsBusy(true);
    var amount = Number(bet?.replace(/[^0-9.-]+/g, ""));
    addBet(match?.id, selectedTeam.code, amount)
      .then(async ({ data }) => {
        dispatch(setCredits(data.credits));
        setBets(data.bets);
        await drawForm.reset({ amount: "" });
        drawForm.clearErrors("amount");
        setShowConfirmBetDialog(false);
      })
      .finally(() => {
        setIsBusy(false);
      });
  };
  const [total, setTotal] = useState(0);

  const [displayTotalBets, setDisplayTotalBets] = useState(0);

  useEffect(() => {
    setTotal(totalBets?.find((x) => x.code === "D")?.amount || 0);

    const controls = animate(
      0,
      totalBets?.find((x) => x.code === "D")?.amount || 0,
      {
        duration: 15,
        onUpdate(value) {
          setDisplayTotalBets(value);
        },
      }
    );

    return () => controls.stop();
  }, [totalBets]);

  return (
    <>
      <StyledCard bg="dark" text="white">
        <Card.Body className="p-1">
          <h6 className="text-white text-center">
            <span className="text-success">x{drawMultiplier}</span> IF DRAW -
            MAX BET:{" "}
            <span className="text-warning">
              {maxDrawBet?.toLocaleString(locale || "en-US", {
                style: "currency",
                currency: currency || "USD",
              })}{" "}
            </span>{" "}
            / FIGHT
          </h6>
          <Row className="text-center" xs={2} md={2}>
            <Col>
              <small className="text-secondary">YOUR BET</small>
              <h5 className="text text-info">
                {bets
                  .find((x) => x.code === "D")
                  ?.amount?.toLocaleString(locale || "en-US", {
                    style: "currency",
                    currency: currency || "USD",
                  }) ||
                  (0).toLocaleString(locale, {
                    style: "currency",
                    currency: currency || "USD",
                  })}
              </h5>
            </Col>
            <Col>
              <small className="text-secondary">TOTAL BETS</small>
              {isRolling ? (
                <h5>
                  {displayTotalBets?.toLocaleString(locale || "en-US", {
                    style: "currency",
                    currency: currency || "USD",
                  })}
                </h5>
              ) : (
                <h5>
                  {total?.toLocaleString(locale || "en-US", {
                    style: "currency",
                    currency: currency || "USD",
                  })}
                </h5>
              )}
            </Col>
          </Row>
          <Row className="gx-1 gy-1" xs={1} md={2}>
            <Col className="col-xs-auto">
              <InputGroup className="mb-1">
                <CurrencyInput
                  onValueChange={onValueChangeDraw}
                  className={
                    drawForm.errors?.amount
                      ? "form-control form-control-lg invalid"
                      : "form-control form-control-lg"
                  }
                  placeholder="ENTER AMOUNT"
                  decimalScale={2}
                  allowNegativeValue={false}
                  intlConfig={{ locale, currency }}
                  {...drawForm.register("amount", {
                    required: true,
                    ref: drawAmountRef,
                  })}
                  autoComplete="off"
                  disabled={disabled || credits < minimumBet}
                />
                <Button
                  variant="secondary"
                  onClick={clearDrawAmount}
                  disabled={disabled || credits < minimumBet}
                >
                  CLEAR
                </Button>
              </InputGroup>
            </Col>
            <Col className="col-xs-auto">
              <Button
                variant="dark"
                size="lg"
                className="form-control text-uppercase"
                style={{ backgroundColor: "green" }}
                onClick={() =>
                  handlePlaceDrawBet({
                    name: "DRAW",
                    code: "D",
                    color: "green",
                  })
                }
                disabled={disabled || credits < minimumBet}
              >
                DRAW
              </Button>
            </Col>
          </Row>
        </Card.Body>
      </StyledCard>
      <ConfirmDialog
        confirmButtonText="Place Bet"
        confirmButtonTextBusy="Placing Bet..."
        handleConfirm={placeBet}
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
