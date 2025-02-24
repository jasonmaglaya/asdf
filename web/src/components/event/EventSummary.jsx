import { Badge, Card, Col, Row, Table } from "react-bootstrap";
import TeamAvatar from "./TeamAvatar";
import { useEffect, useMemo, useState } from "react";

export default function EventSummary({ history, currency, locale }) {
  const [summary, setSummary] = useState([]);
  const [bettings, setBettings] = useState({});
  const [totalWinnings, setTotalWinnings] = useState(0);
  const [totalLosses, setTotalLosses] = useState(0);
  const [winRate, setWinRate] = useState(0);
  const [wins, setWins] = useState({});

  const legend = useMemo(
    () => [
      { code: "M", color: "#e50914", text: "MERON", count: 0 },
      { code: "W", color: "#3d9ae8", text: "WALA", count: 0 },
      { code: "D", color: "green", text: "DRAW", count: 0 },
    ],
    []
  );

  useEffect(() => {
    const sum = history?.reduce((acc, item) => {
      const { matchNumber, betOn, bet, odds } = item;
      let existingGroup = acc.find(
        (x) => x.matchNumber === matchNumber && x.betOn === betOn
      );

      const { winners, gainLoss, notes } = item;
      if (existingGroup) {
        existingGroup.declarations.push({ winners, gainLoss, notes });
      } else {
        acc.push({
          matchNumber,
          betOn,
          bet,
          odds,
          declarations: [{ winners, gainLoss, notes }],
        });
      }
      return acc;
    }, []);

    setSummary(sum);

    let winnings = 0;
    let losses = 0;
    const bets = {};
    const wins = {};
    let totalWins = 0;

    sum.forEach((x) => {
      if (bets[x.betOn]) {
        bets[x.betOn] += 1;
      } else {
        bets[x.betOn] = 1;
      }

      const { gainLoss, winners } = x.declarations[x.declarations.length - 1];

      if (gainLoss > 0) {
        winnings += gainLoss;
      } else {
        losses += gainLoss;
      }

      if (winners.split(",").includes(x.betOn)) {
        wins[x.betOn] = wins[x.betOn] ? wins[x.betOn] + 1 : 1;

        totalWins += 1;
      }
    });

    setTotalWinnings(winnings);
    setTotalLosses(losses);
    setBettings(bets);
    setWinRate((totalWins / sum.length) * 100);
    setWins(wins);
  }, [history]);

  return (
    <>
      <Card bg="dark" text="white" className="mb-2">
        <Card.Header>
          <div className="d-flex overflow-auto mb-2 align-items-center justify-content-center">
            {legend.map(({ code, text }) => {
              const color = legend.find((x) => x.code === code)?.color;
              return (
                <div
                  key={text}
                  className="d-flex flex-row align-items-center"
                  style={{ marginRight: "1em" }}
                >
                  <TeamAvatar color={color}>{bettings[code] || 0}</TeamAvatar>
                  <span>
                    {text} ({wins[code] || 0})
                  </span>
                </div>
              );
            })}
          </div>
        </Card.Header>
        <Card.Body>
          <Row>
            <Col>
              <h6>
                Total Winnings:{" "}
                <span className="text-success">
                  {totalWinnings?.toLocaleString(locale || "en-US", {
                    style: "currency",
                    currency: currency || "USD",
                  })}
                </span>
              </h6>
            </Col>
            <Col>
              <h6>
                Total Losses:{" "}
                <span className="text-danger">
                  {totalLosses?.toLocaleString(locale || "en-US", {
                    style: "currency",
                    currency: currency || "USD",
                  })}
                </span>
              </h6>
            </Col>
          </Row>
          <Row>
            <Col>
              <h6>
                Net:{" "}
                <span
                  className={
                    totalWinnings + totalLosses > 0
                      ? "text-success"
                      : "text-danger"
                  }
                >
                  {(totalWinnings + totalLosses)?.toLocaleString(
                    locale || "en-US",
                    {
                      style: "currency",
                      currency: currency || "USD",
                    }
                  )}
                </span>
              </h6>
            </Col>
            <Col>
              <h6>
                Win Rate:{" "}
                <span className="text-info">{winRate.toFixed(2) || 0}%</span>
              </h6>
            </Col>
          </Row>
        </Card.Body>
      </Card>

      <Table striped bordered hover responsive variant="dark">
        <thead>
          <tr>
            <th style={{ width: "6%" }}>Fight #</th>
            <th style={{ width: "20%" }}>Bet On</th>
            <th style={{ width: "15%" }} className="text-end">
              Amount
            </th>
            <th className="text-center">Odds/Multiplier</th>
          </tr>
        </thead>
        <tbody>
          {summary?.map((item) => {
            return (
              <>
                <tr key={`${item.fightNumber}-${item.gainLossDate}-1`}>
                  <td className="align-middle">{item.matchNumber}</td>
                  <td className="align-middle">
                    {item.betOn?.split(",").map((code) => {
                      const { color, text } = legend.find(
                        (x) => x.code === code
                      );
                      return (
                        <div key={code} className="d-flex align-items-center">
                          <TeamAvatar key={code} color={color}>
                            {code}
                          </TeamAvatar>
                          {text}
                        </div>
                      );
                    })}
                  </td>
                  <td className="text-end align-middle">
                    {item.bet.toLocaleString(locale || "en-US", {
                      style: "currency",
                      currency: currency || "USD",
                    })}
                  </td>
                  <td className="text-center align-middle">
                    {(item.odds * 100).toFixed(2)}%
                  </td>
                </tr>
                <tr key={`${item.fightNumber}-${item.gainLossDate}-2`}>
                  <td colSpan="4" className="p-3">
                    <Table size="sm" bordered striped hover responsive>
                      <thead>
                        <tr>
                          <th style={{ width: "20%" }}>Winner</th>
                          <th
                            style={{ width: "10%" }}
                            className="text-center align-middle"
                          >
                            Result
                          </th>
                          <th
                            style={{ width: "15%" }}
                            className="text-end align-middle"
                          >
                            Winning/Loss
                          </th>
                          <th>Description</th>
                        </tr>
                      </thead>
                      <tbody>
                        {item.declarations.map((declaration) => {
                          return (
                            <tr>
                              <td className="align-middle">
                                {declaration.winners?.split(",").map((code) => {
                                  const { color, text } = legend.find(
                                    (x) => x.code === code
                                  );
                                  return (
                                    <div
                                      key={code}
                                      className="d-flex align-items-center"
                                    >
                                      <TeamAvatar key={code} color={color}>
                                        {code}
                                      </TeamAvatar>
                                      {text}
                                    </div>
                                  );
                                })}
                              </td>
                              <td className="text-center align-middle">
                                {declaration.winners
                                  ?.split(",")
                                  .includes(item.betOn) ? (
                                  <Badge bg="success">WIN</Badge>
                                ) : (
                                  <Badge bg="danger">LOSE</Badge>
                                )}
                              </td>
                              <td className="text-end align-middle">
                                {declaration.gainLoss.toLocaleString(
                                  locale || "en-US",
                                  {
                                    style: "currency",
                                    currency: currency || "USD",
                                  }
                                )}
                              </td>
                              <td className="align-middle">
                                {declaration.notes}
                              </td>
                            </tr>
                          );
                        })}
                      </tbody>
                    </Table>
                  </td>
                </tr>
              </>
            );
          })}
        </tbody>
      </Table>
    </>
  );
}
