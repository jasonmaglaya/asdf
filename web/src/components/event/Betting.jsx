import { useEffect, useState, useContext } from "react";
import { Card, Row, Col, Badge } from "react-bootstrap";
import Team from "./Team";
import PlaceBet from "./PlaceBet";
import { MatchStatus, AdminRoles } from "../../constants";
import { getBets } from "../../services/matchesService";
import { BetsContext } from "../_shared/BetsContext";
import { MatchContext } from "../_shared/MatchContext";
import PlaceBetDraw from "./PlaceBetDraw";
import { useSelector } from "react-redux";
import SpinnerComponent from "../_shared/SpinnerComponent";

export default function Betting({
  eventId,
  title,
  minimumBet,
  maximumBet,
  allowDraw,
  maxDrawBet,
  drawMultiplier,
  allowBetting,
  currency,
  locale,
}) {
  const [bets, setBets] = useState([]);
  const [disabled, setDisabled] = useState(true);
  const { match, totalBets, commission, status, winners } =
    useContext(MatchContext);

  const { role, credits } = useSelector((state) => state.user);

  const getUserBets = (matchId) => {
    getBets(matchId).then(({ data }) => {
      setBets(data.result);
    });
  };

  useEffect(() => {
    setDisabled(status !== MatchStatus.Open);

    if (match?.id) {
      getUserBets(match.id);
    }
  }, [match, status]);

  if (!eventId) {
    return <SpinnerComponent />;
  }

  return (
    <>
      <Card bg="dark" text="white" className="mb-1">
        <Card.Header className="p-2 pb-0">
          <h5 className="text-center">{title}</h5>
          <div className="d-flex justify-content-between align-items-center">
            <h4>
              <Badge
                className="text-uppercase"
                bg={
                  status === MatchStatus.Open
                    ? "success"
                    : status === MatchStatus.Cancelled
                    ? "secondary"
                    : "danger"
                }
              >
                {status === MatchStatus.Open
                  ? MatchStatus.Open
                  : status === MatchStatus.Cancelled
                  ? MatchStatus.Cancelled
                  : "Closed"}
              </Badge>
            </h4>
            {status === MatchStatus.Open ? (
              <h4 className="text-center text-danger fw-bolder blink">
                PLACE YOUR BETS!
              </h4>
            ) : [MatchStatus.Declared, MatchStatus.Completed].includes(
                status
              ) ? (
              <h4 className="text-center fw-bolder blink">
                {[...match?.teams, { code: "D", name: "DRAW", color: "green" }]
                  ?.filter((x) => winners.includes(x.code))
                  .map((x) => {
                    const { name, color } = x;
                    return (
                      <span key={name} className="fw-bolder" style={{ color }}>
                        {name}
                      </span>
                    );
                  })}{" "}
                WINS
              </h4>
            ) : (
              <></>
            )}

            <h5 className="d-flex align-items-center">
              <span className="p-1">FIGHT#</span>
              <Badge bg="info" className="p-1">
                <span className="h5">{match?.number}</span>
              </Badge>
            </h5>
          </div>
        </Card.Header>
        <Card.Body className="p-1">
          <BetsContext.Provider value={[bets, setBets]}>
            <Row className="gx-1 gy-1" xs={2} sm={2} md={2} lg={2}>
              {match?.teams?.map((t, index) => {
                const userBet = bets.find((x) => x.code === t.code)?.amount;
                const teamTotalBets = totalBets.find(
                  (x) => x.code === t.code
                )?.amount;
                const total = totalBets
                  .map((x) => (x.code === "D" ? 0 : x.amount))
                  .reduce((a, b) => a + b, 0);

                const odd = (total - total * commission) / teamTotalBets;

                return (
                  <Col key={t.name}>
                    <Team
                      matchId={match?.id}
                      name={t.name}
                      code={t.code}
                      color={t.color}
                      minimumBet={minimumBet}
                      maximumBet={maximumBet}
                      bet={userBet || 0}
                      totalBets={teamTotalBets}
                      odd={odd ?? 0}
                      disabled={disabled || credits < minimumBet}
                      allowBetting={allowBetting}
                      currency={currency}
                      locale={locale}
                      isRolling={
                        status === MatchStatus.Open &&
                        !AdminRoles.includes(role)
                      }
                      animationDuration={15 + (index + 1) * 5}
                    />
                  </Col>
                );
              })}
              {!allowBetting && (
                <Col key="DRAW">
                  <Team
                    matchId={match?.id}
                    name="DRAW"
                    code="D"
                    color="green"
                    totalBets={totalBets.find((x) => x.code === "D")?.amount}
                    disabled={disabled || credits < minimumBet}
                    allowBetting={allowBetting}
                    currency={currency}
                    locale={locale}
                    drawMultiplier={drawMultiplier}
                  />
                </Col>
              )}
            </Row>
            {allowBetting && (
              <>
                <Row className="pt-1 d-none d-md-block">
                  <Col>
                    <PlaceBet
                      matchId={match?.id}
                      teams={match?.teams}
                      minimumBet={minimumBet}
                      maximumBet={maximumBet}
                      allowDraw={allowDraw}
                      disabled={disabled}
                      currency={currency}
                      locale={locale}
                    />
                  </Col>
                </Row>
                <Row className="pt-1">
                  <Col>
                    {allowDraw ? (
                      <PlaceBetDraw
                        minimumBet={minimumBet}
                        maxDrawBet={maxDrawBet}
                        drawMultiplier={drawMultiplier}
                        disabled={disabled}
                        currency={currency}
                        locale={locale}
                        isRolling={
                          status === MatchStatus.Open &&
                          !AdminRoles.includes(role)
                        }
                      />
                    ) : (
                      <></>
                    )}
                  </Col>
                </Row>
              </>
            )}
          </BetsContext.Provider>
        </Card.Body>
      </Card>
    </>
  );
}
