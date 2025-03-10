import styled from "styled-components";
import { Card, Row, Col, Badge } from "react-bootstrap";
import PlaceBetDialog from "./PlaceBetDialog";
import { StyledCard } from "../_shared/StyledCard";
import { animate } from "framer-motion";
import { useEffect, useState } from "react";

const StyledBadge = styled(Badge)`
  background-color: ${(props) => props.bgcolor + "!important"};
  color: white;
`;

export default function Team({
  matchId,
  name,
  code,
  color,
  minimumBet,
  maximumBet,
  totalBets,
  odd,
  bet,
  disabled,
  allowBetting,
  currency,
  locale,
  drawMultiplier,
  isRolling,
  animationDuration,
}) {
  const [displayTotalBets, setDisplayTotalBets] = useState(0);
  const [displayOdd, setDisplayOdd] = useState(0);
  const [payOut, setPayOut] = useState(0);
  const [displayPayOut, setDisplayPayOut] = useState(0);

  useEffect(() => {
    if (isRolling) {
      const controls = animate(0, totalBets ?? 0, {
        duration: animationDuration ?? 15,
        onUpdate(value) {
          setDisplayTotalBets(value);
        },
      });

      return () => controls.stop();
    }
  }, [totalBets, animationDuration, isRolling]);

  useEffect(() => {
    if (isRolling) {
      const controls = animate(0, odd, {
        duration: animationDuration + 10 ?? 25,
        onUpdate(value) {
          setDisplayOdd(value);
        },
      });

      return () => controls.stop();
    }
  }, [odd, animationDuration, isRolling]);

  useEffect(() => {
    setPayOut(bet * odd);

    if (isRolling) {
      const controls = animate(0, bet * odd, {
        duration: animationDuration ?? 15,
        onUpdate(value) {
          setDisplayPayOut(value);
        },
      });

      return () => controls.stop();
    }
  }, [bet, odd, animationDuration, isRolling]);

  return (
    <StyledCard bg="dark" text="white" className="h-100">
      <Card.Header className="text-center p-1">
        <h3 style={{ textTransform: "uppercase" }}>
          <StyledBadge bgcolor={color} pill>
            {name}
          </StyledBadge>
        </h3>
        <Row xs={1} md={2}>
          <Col>
            <small className="d-none d-lg-block">TOTAL BETS</small>
            {isRolling ? (
              <h5>
                {(displayTotalBets || 0).toLocaleString(locale || "en-US", {
                  style: "currency",
                  currency: currency || "USD",
                })}
              </h5>
            ) : (
              <h5>
                {(totalBets || 0).toLocaleString(locale || "en-US", {
                  style: "currency",
                  currency: currency || "USD",
                })}
              </h5>
            )}
          </Col>
          {code !== "D" && (
            <Col>
              <small className="d-none d-lg-block">ODDS</small>
              {isRolling ? (
                <h5 className="text text-warning">
                  {displayOdd && displayOdd !== Infinity
                    ? (displayOdd * 100).toFixed(2)
                    : "0.00"}
                  %
                </h5>
              ) : (
                <h5 className="text text-warning">
                  {odd && odd !== Infinity ? (odd * 100).toFixed(2) : "0.00"}%
                </h5>
              )}
            </Col>
          )}
          {code === "D" && (
            <Col>
              <small className="d-none d-lg-block">MULTIPLIER</small>
              <h5 className="text text-warning">x{drawMultiplier}</h5>
            </Col>
          )}
        </Row>
      </Card.Header>
      {allowBetting ? (
        <Card.Body className="p-1 text-center">
          <Row xs={1} md={2}>
            <Col>
              <small className="text-secondary">YOUR BET</small>
              <h5 className="text text-info">
                {bet
                  ? bet?.toLocaleString(locale || "en-US", {
                      style: "currency",
                      currency: currency || "USD",
                    })
                  : "0.00"}
              </h5>
            </Col>
            <Col>
              <small className="text-secondary">PAYOUT</small>
              {isRolling ? (
                <h5 className="text text-success">
                  {(displayPayOut || 0).toLocaleString(locale || "en-US", {
                    style: "currency",
                    currency: currency || "USD",
                  })}
                </h5>
              ) : (
                <h5 className="text text-success">
                  {(payOut || 0).toLocaleString(locale || "en-US", {
                    style: "currency",
                    currency: currency || "USD",
                  })}
                </h5>
              )}
            </Col>
          </Row>
          <Row className="d-block d-md-none">
            <Col key={name}>
              <PlaceBetDialog
                matchId={matchId}
                color={color}
                name={name}
                code={code}
                minimumBet={minimumBet}
                maximumBet={maximumBet}
                disabled={disabled}
                currency={currency}
                locale={locale}
              />
            </Col>
          </Row>
        </Card.Body>
      ) : (
        <></>
      )}
    </StyledCard>
  );
}
