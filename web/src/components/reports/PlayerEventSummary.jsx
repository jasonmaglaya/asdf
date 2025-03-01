import { Badge, Card, Col, Row, Table } from "react-bootstrap";
import TeamAvatar from "../event/TeamAvatar";
import { useEffect, useMemo, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus, faMinus } from "@fortawesome/free-solid-svg-icons";

export default function PlayerEventSummary({ history, currency, locale }) {
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
      { code: "C", color: "grey", text: "CANCELLED", count: 0 },
    ],
    []
  );

  useEffect(() => {
    const sum = history?.reduce((acc, item) => {
      const { matchNumber, betOn, bet, odds, betTimeStamp } = item;
      const existingGroup = acc.find(
        (x) =>
          x.matchNumber === matchNumber &&
          x.betOn === betOn &&
          x.betTimeStamp === betTimeStamp
      );

      const { winners, gainLoss, notes, gainLossDate } = item;
      if (existingGroup) {
        existingGroup.declarations.push({
          winners,
          gainLoss,
          notes,
          gainLossDate,
        });

        existingGroup.gainLoss += gainLoss;
      } else {
        acc.push({
          matchNumber,
          betOn,
          bet,
          odds,
          betTimeStamp,
          declarations: [{ winners, gainLoss, notes, gainLossDate }],
          gainLoss,
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

      const { winners } = x.declarations[x.declarations.length - 1];

      if (x.gainLoss > 0) {
        winnings += x.gainLoss;
      } else {
        losses += x.gainLoss;
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

  const showHide = (id) => {
    document.getElementById(id).classList.toggle("d-none");
    document.getElementById(`${id}-plus`).classList.toggle("d-none");
    document.getElementById(`${id}-minus`).classList.toggle("d-none");
  };

  const getClassName = (amount) => {
    return amount === 0
      ? "text-end align-middle text-warning"
      : amount > 0
      ? "text-end align-middle text-success"
      : "text-end align-middle text-danger";
  };

  return (
    <>
      <Card bg="dark" text="white" className="mb-2">
        <Card.Header>
          <div className="d-flex overflow-auto align-items-center justify-content-center">
            {legend.map(({ code, text }) => {
              const color = legend.find((x) => x.code === code)?.color;
              return (
                <div
                  key={text}
                  className="d-flex flex-column align-items-center mx-3"
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
                  {(totalLosses
                    ? totalLosses * -1
                    : totalLosses
                  )?.toLocaleString(locale || "en-US", {
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
                <span className="text-info">
                  {(winRate || 0).toFixed(2) || 0}%
                </span>
              </h6>
            </Col>
          </Row>
        </Card.Body>
      </Card>

      <Table striped bordered hover responsive variant="dark">
        <thead>
          <tr>
            <th></th>
            <th className="align-middle" style={{ width: "6%" }}>
              Fight #
            </th>
            <th className="align-middle" style={{ width: "10%" }}>
              Bet On
            </th>
            <th className="align-middle" style={{ width: "20%" }}>
              Date & Time
            </th>
            <th className="align-middle" style={{ width: "10%" }}>
              Odds/Multiplier
            </th>
            <th className="align-middle text-center" style={{ width: "6%" }}>
              Result
            </th>
            <th style={{ width: "24%" }} className="align-middle text-end">
              Amount
            </th>
            <th style={{ width: "24%" }} className="text-end align-middle">
              Winnings/Losses
            </th>
          </tr>
        </thead>
        <tbody>
          {summary?.map((item) => {
            return (
              <>
                <tr key={`${item.fightNumber}-${item.betTimeStamp}-1`}>
                  <td
                    className="text-center align-middle"
                    onClick={() => {
                      showHide(`${item.fightNumber}-${item.betTimeStamp}-2`);
                    }}
                    style={{ cursor: "pointer" }}
                  >
                    <FontAwesomeIcon
                      icon={faPlus}
                      id={`${item.fightNumber}-${item.betTimeStamp}-2-plus`}
                      className="d-block"
                    />
                    <FontAwesomeIcon
                      icon={faMinus}
                      id={`${item.fightNumber}-${item.betTimeStamp}-2-minus`}
                      className="d-none"
                    />
                  </td>
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
                  <td className="align-middle">
                    {new Date(item.betTimeStamp)?.toLocaleString()}
                  </td>
                  <td className="align-middle">
                    {(item.odds * 100).toFixed(2)}%
                  </td>
                  <td className="text-center align-middle">
                    {item.declarations[item.declarations.length - 1].winners
                      ?.split(",")
                      .includes(item.betOn) ? (
                      <Badge bg="success">WIN</Badge>
                    ) : item.declarations[item.declarations.length - 1]
                        .winners === "D" ? (
                      <Badge bg="warning">DRAW</Badge>
                    ) : item.declarations[item.declarations.length - 1]
                        .winners === "C" ? (
                      <Badge bg="warning">CANCELLED</Badge>
                    ) : (
                      <Badge bg="danger">LOSE</Badge>
                    )}
                  </td>
                  <td className="text-end align-middle">
                    {item.bet.toLocaleString(locale || "en-US", {
                      style: "currency",
                      currency: currency || "USD",
                    })}
                  </td>
                  <td className={getClassName(item.gainLoss)}>
                    {item.gainLoss?.toLocaleString(locale || "en-US", {
                      style: "currency",
                      currency: currency || "USD",
                    })}
                  </td>
                </tr>
                <tr
                  id={`${item.fightNumber}-${item.betTimeStamp}-2`}
                  className="d-none"
                >
                  <td colSpan="8" className="p-3">
                    <Table size="sm" bordered striped hover responsive>
                      <thead>
                        <tr>
                          <th className="align-middle" style={{ width: "20%" }}>
                            Winner
                          </th>
                          <th
                            style={{ width: "6%" }}
                            className="text-center align-middle"
                          >
                            Result
                          </th>
                          <th className="align-middle" style={{ width: "20%" }}>
                            Declare Date & Time
                          </th>
                          <th className="align-middle" style={{ width: "20%" }}>
                            Description
                          </th>
                          <th className="text-end align-middle">
                            Winnings/Losses
                          </th>
                        </tr>
                      </thead>
                      <tbody>
                        {item.declarations.map((declaration) => {
                          return (
                            <tr
                              key={`${declaration.winners}-${declaration.gainLossDate}-${declaration.notes}`}
                            >
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
                                ) : declaration.winners === "D" ? (
                                  <Badge bg="warning">DRAW</Badge>
                                ) : declaration.winners === "C" ? (
                                  <Badge bg="warning">CANCELLED</Badge>
                                ) : (
                                  <Badge bg="danger">LOSE</Badge>
                                )}
                              </td>
                              <td className="align-middle">
                                {new Date(
                                  declaration.gainLossDate
                                )?.toLocaleString()}
                              </td>
                              <td className="align-middle">
                                {declaration.notes}
                              </td>
                              <td
                                className={getClassName(declaration.gainLoss)}
                              >
                                {declaration.gainLoss.toLocaleString(
                                  locale || "en-US",
                                  {
                                    style: "currency",
                                    currency: currency || "USD",
                                  }
                                )}
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
