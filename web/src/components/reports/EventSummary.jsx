import { Card, Col, Row, Table } from "react-bootstrap";
import TeamAvatar from "../../components/event/TeamAvatar";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMinus, faPlus } from "@fortawesome/free-solid-svg-icons";
import { useMemo, Fragment } from "react";

export default function EventSummary({ summary, currency, locale, event }) {
  const legend = useMemo(
    () => [
      { code: "M", color: "#e50914", text: "MERON", count: 0 },
      { code: "W", color: "#3d9ae8", text: "WALA", count: 0 },
      { code: "D", color: "green", text: "DRAW", count: 0 },
      { code: "C", color: "grey", text: "CANCELLED", count: 0 },
    ],
    []
  );

  const showHide = (id) => {
    document.getElementById(id).classList.toggle("d-none");
    document.getElementById(`${id}-plus`).classList.toggle("d-none");
    document.getElementById(`${id}-minus`).classList.toggle("d-none");
  };

  return (
    <>
      <Card bg="dark" text="white" className="mb-2">
        <Card.Header className="text-center">
          <h5>{event?.title}</h5>
        </Card.Header>
        <Card.Body>
          <Row>
            <Col md={4} className="text-md-center">
              <h6>
                Date:{" "}
                {new Date(event.eventDate).toLocaleDateString(locale, {
                  year: "numeric",
                  month: "2-digit",
                  day: "2-digit",
                })}
              </h6>
            </Col>
            <Col md={4} className="text-md-center">
              <h6>
                Commission:{" "}
                <span className="text-info">
                  {(event?.commission * 100).toFixed(2) || 0}%
                </span>
              </h6>
            </Col>
            <Col md={4} className="text-md-center">
              <h6>
                Draw Multiplier:{" "}
                <span className="text-info">x{event?.drawMultiplier}</span>
              </h6>
            </Col>
          </Row>
        </Card.Body>
      </Card>
      <Table striped bordered hover responsive variant="dark">
        <thead>
          <tr>
            <th style={{ width: "0.1%" }}></th>
            <th className="align-middle" style={{ width: "2%" }}>
              Fight#
            </th>
            <th className="align-middle" style={{ width: "5%" }}>
              Winner
            </th>
            <th className="align-middle text-end" style={{ width: "8%" }}>
              Total Bets
            </th>
            <th style={{ width: "8%" }} className="align-middle text-end">
              Commission
            </th>
            <th style={{ width: "8%" }} className="text-end align-middle">
              Total Draw
            </th>
            <th style={{ width: "8%" }} className="text-end align-middle">
              Draw Payouts
            </th>
            <th style={{ width: "8%" }} className="text-end align-middle">
              Draw Net
            </th>
          </tr>
        </thead>
        <tbody>
          {summary?.map((item) => {
            return (
              <Fragment key={`${item.matchId}`}>
                <tr>
                  <td
                    className="text-center align-middle"
                    onClick={() => {
                      showHide(`${item.matchId}-2`);
                    }}
                    style={{ cursor: "pointer" }}
                  >
                    <FontAwesomeIcon
                      icon={faPlus}
                      id={`${item.matchId}-2-plus`}
                      className="d-block"
                    />
                    <FontAwesomeIcon
                      icon={faMinus}
                      id={`${item.matchId}-2-minus`}
                      className="d-none"
                    />
                  </td>
                  <td className="align-middle">{item.number}</td>
                  <td className="align-middle">
                    {item.winners?.split(",").map((code) => {
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
                    {item.totalBets.toLocaleString(locale || "en-US", {
                      style: "currency",
                      currency: currency || "USD",
                    })}
                  </td>
                  <td
                    className={
                      item.commission === 0
                        ? "text-end align-middle text-warning"
                        : item.commission > 0
                        ? "text-end align-middle text-success"
                        : "text-end align-middle text-danger"
                    }
                  >
                    {item.commission.toLocaleString(locale || "en-US", {
                      style: "currency",
                      currency: currency || "USD",
                    })}
                  </td>
                  <td className="text-end align-middle">
                    {item.totalDraw?.toLocaleString(locale || "en-US", {
                      style: "currency",
                      currency: currency || "USD",
                    })}
                  </td>
                  <td className="text-end align-middle">
                    {item.totalDrawPayout?.toLocaleString(locale || "en-US", {
                      style: "currency",
                      currency: currency || "USD",
                    })}
                  </td>
                  <td
                    className={
                      item.drawNet === 0
                        ? "text-end align-middle text-warning"
                        : item.drawNet > 0
                        ? "text-end align-middle text-success"
                        : "text-end align-middle text-danger"
                    }
                  >
                    {item.drawNet?.toLocaleString(locale || "en-US", {
                      style: "currency",
                      currency: currency || "USD",
                    })}
                  </td>
                </tr>
                <tr id={`${item.matchId}-2`} className="d-none">
                  <td colSpan="8" className="p-3">
                    <Table size="sm" bordered striped hover responsive>
                      <thead>
                        <tr>
                          <th className="align-middle" style={{ width: "10%" }}>
                            Winner
                          </th>
                          <th className="align-middle" style={{ width: "15%" }}>
                            Declare/Cancel Date
                          </th>
                          <th className="align-middle" style={{ width: "15%" }}>
                            Declared/Cancelled By
                          </th>
                          <th className="align-middle" style={{ width: "10%" }}>
                            IP Address
                          </th>
                          <th
                            className="align-middle text-end"
                            style={{ width: "10%" }}
                          >
                            Total Bets
                          </th>
                          <th
                            style={{ width: "10%" }}
                            className="align-middle text-end"
                          >
                            Commission
                          </th>
                          <th
                            style={{ width: "10%" }}
                            className="text-end align-middle"
                          >
                            Total Draw
                          </th>
                          <th
                            style={{ width: "10%" }}
                            className="text-end align-middle"
                          >
                            Draw Payouts
                          </th>
                          <th
                            style={{ width: "10%" }}
                            className="text-end align-middle"
                          >
                            Draw Net
                          </th>
                        </tr>
                      </thead>
                      <tbody>
                        {item.declarations.map((declaration) => {
                          return (
                            <tr
                              key={`${item.matchId}-2-${declaration.winners}`}
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
                              <td className="align-middle">
                                {new Date(
                                  declaration.declareDate
                                )?.toLocaleString()}
                              </td>
                              <td className="align-middle">
                                {declaration.declaredBy}
                              </td>
                              <td className="align-middle">
                                {declaration.ipAddress}
                              </td>
                              <td className="text-end align-middle">
                                {declaration.totalBets.toLocaleString(
                                  locale || "en-US",
                                  {
                                    style: "currency",
                                    currency: currency || "USD",
                                  }
                                )}
                              </td>
                              <td
                                className={
                                  declaration.commission === 0
                                    ? "text-end align-middle text-warning"
                                    : declaration.drawNet > 0
                                    ? "text-end align-middle text-success"
                                    : "text-end align-middle text-danger"
                                }
                              >
                                {declaration.commission.toLocaleString(
                                  locale || "en-US",
                                  {
                                    style: "currency",
                                    currency: currency || "USD",
                                  }
                                )}
                              </td>
                              <td className="text-end align-middle">
                                {declaration.totalDraw?.toLocaleString(
                                  locale || "en-US",
                                  {
                                    style: "currency",
                                    currency: currency || "USD",
                                  }
                                )}
                              </td>
                              <td className="text-end align-middle">
                                {declaration.totalDrawPayout?.toLocaleString(
                                  locale || "en-US",
                                  {
                                    style: "currency",
                                    currency: currency || "USD",
                                  }
                                )}
                              </td>
                              <td
                                className={
                                  declaration.drawNet === 0
                                    ? "text-end align-middle text-warning"
                                    : declaration.drawNet > 0
                                    ? "text-end align-middle text-success"
                                    : "text-end align-middle text-danger"
                                }
                              >
                                {declaration.drawNet?.toLocaleString(
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
              </Fragment>
            );
          })}
        </tbody>
      </Table>
    </>
  );
}
