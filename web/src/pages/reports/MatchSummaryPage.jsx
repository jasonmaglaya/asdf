import { NavLink, useParams } from "react-router-dom";
import Betting from "../../components/event/Betting";
import { Fragment, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { getEvent } from "../../services/eventsService";
import { getMatch, getTotalBets } from "../../services/matchesService";
import { Breadcrumb, Container, Table } from "react-bootstrap";
import SpinnerComponent from "../../components/_shared/SpinnerComponent";
import {
  getMatchSummary,
  getPlayerBetSummary,
} from "../../services/reportsService";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMinus, faPlus } from "@fortawesome/free-solid-svg-icons";
import TeamAvatar from "../../components/event/TeamAvatar";

export default function MatchSummaryPage() {
  const [isLoading, setIsLoading] = useState(true);
  const { eventId, matchId } = useParams();
  const [event, setEvent] = useState({});
  const [match, setMatch] = useState({});
  const [totalBets, setTotalBets] = useState([]);
  const [commission, setCommission] = useState(0);
  const [summary, setSummary] = useState([]);
  const { appSettings } = useSelector((state) => state.appSettings);
  const { currency, locale } = appSettings;

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
    const loadMatch = async (id) => {
      try {
        const getMatchResult = await getMatch(id);
        if (!getMatchResult.data.isSuccessful) {
          return;
        }

        const match = getMatchResult.data.result;
        setMatch(match);

        const getEventResult = await getEvent(match.eventId);
        if (!getEventResult.data.isSuccessful) {
          return;
        }

        const event = getEventResult.data.result;
        setEvent(event);

        const getTotalBetsResult = await getTotalBets(id);
        setTotalBets(getTotalBetsResult.data.result.totalBets);
        setCommission(getTotalBetsResult.data.result.commission);

        const getMatchSummaryResult = await getMatchSummary(id);
        setSummary(getMatchSummaryResult.data.result);

        setIsLoading(false);
      } catch {}
    };

    loadMatch(matchId);
  }, [matchId]);

  const showHide = async (id) => {
    document.getElementById(`${id}-2`).classList.toggle("d-none");
    document.getElementById(`${id}-2-plus`).classList.toggle("d-none");
    document.getElementById(`${id}-2-minus`).classList.toggle("d-none");

    const item = summary.find((x) => x.userId === id);
    if (!item.bets) {
      const getPlayerBetSummaryResult = await getPlayerBetSummary(matchId, id);

      item.bets = getPlayerBetSummaryResult.data.result;

      setSummary([...summary]);
    }
  };

  return (
    <Container className="mt-2 text-light px-3" fluid>
      <h3>Match Summary</h3>
      <Breadcrumb>
        <Breadcrumb.Item>
          <NavLink to="/reports/events">Events</NavLink>
        </Breadcrumb.Item>
        <Breadcrumb.Item>
          <NavLink to={`/reports/events/${event.id}`}>{event.title}</NavLink>
        </Breadcrumb.Item>
        <Breadcrumb.Item className="text-light" active>
          {match?.number}
        </Breadcrumb.Item>
      </Breadcrumb>
      {isLoading ? (
        <SpinnerComponent />
      ) : (
        <>
          <Betting
            eventId={eventId}
            title={event.title}
            minimumBet={event.minimumBet}
            maximumBet={event.maximumBet}
            allowDraw={event.allowDraw}
            maxDrawBet={event.maxDrawBet}
            drawMultiplier={event.drawMultiplier}
            allowAdminBetting={false}
            allowAgentBetting={false}
            currency={currency}
            locale={locale}
            canControlMatch={true}
            allowBetting={false}
            match={match}
            totalBets={totalBets}
            commission={commission}
            status={match?.status}
            winners={match?.winners}
            forReport={true}
          ></Betting>
          <Table striped bordered hover responsive variant="dark">
            <thead>
              <tr>
                <th style={{ width: "0.1%" }}></th>
                <th className="align-middle" style={{ width: "60%" }}>
                  PLAYER
                </th>
                <th className="align-middle text-end" style={{ width: "40%" }}>
                  AMOUNT
                </th>
              </tr>
            </thead>
            <tbody>
              {summary?.map((item) => {
                return (
                  <Fragment key={`${item.userId}`}>
                    <tr key={`${item.userId}-1`}>
                      <td
                        className="text-center align-middle"
                        onClick={() => {
                          showHide(item.userId);
                        }}
                        style={{ cursor: "pointer" }}
                      >
                        <FontAwesomeIcon
                          icon={faPlus}
                          id={`${item.userId}-2-plus`}
                          className="d-block"
                        />
                        <FontAwesomeIcon
                          icon={faMinus}
                          id={`${item.userId}-2-minus`}
                          className="d-none"
                        />
                      </td>
                      <td className="align-middle">{item.userName}</td>
                      <td className="text-end align-middle">
                        {item.amount.toLocaleString(locale || "en-US", {
                          style: "currency",
                          currency: currency || "USD",
                        })}
                      </td>
                    </tr>
                    <tr id={`${item.userId}-2`} className="d-none">
                      <td colSpan="8" className="p-3">
                        {item.bets ? (
                          <Table size="sm" bordered striped hover responsive>
                            <thead>
                              <tr>
                                <th
                                  className="align-middle"
                                  style={{ width: "30%" }}
                                >
                                  Team
                                </th>
                                <th
                                  style={{ width: "30%" }}
                                  className="align-middle text-end"
                                >
                                  Amount
                                </th>
                                <th
                                  className="align-middle"
                                  style={{ width: "20%" }}
                                >
                                  Betting Date
                                </th>
                                <th
                                  className="align-middle"
                                  style={{ width: "20%" }}
                                >
                                  IP Address
                                </th>
                              </tr>
                            </thead>
                            <tbody>
                              {item.bets.map((bet, index) => {
                                return (
                                  <tr key={`${item.userId}-2-${bet.teamCode}`}>
                                    <td className="align-middle">
                                      {bet.teamCode?.split(",").map((code) => {
                                        const { color, text } = legend.find(
                                          (x) => x.code === code
                                        );
                                        return (
                                          <div
                                            key={code}
                                            className="d-flex align-items-center"
                                          >
                                            <TeamAvatar
                                              key={code}
                                              color={color}
                                            >
                                              {code}
                                            </TeamAvatar>
                                            {text}
                                          </div>
                                        );
                                      })}
                                    </td>
                                    <td className="align-middle text-end">
                                      {bet.amount.toLocaleString(
                                        locale || "en-US",
                                        {
                                          style: "currency",
                                          currency: currency || "USD",
                                        }
                                      )}
                                    </td>
                                    <td className="align-middle">
                                      {new Date(
                                        `${bet.betTimeStamp}Z`
                                      )?.toLocaleString()}
                                    </td>
                                    <td className="align-middle">
                                      {bet.ipAddress}
                                    </td>
                                  </tr>
                                );
                              })}
                            </tbody>
                          </Table>
                        ) : (
                          <SpinnerComponent />
                        )}
                      </td>
                    </tr>
                  </Fragment>
                );
              })}
            </tbody>
          </Table>
        </>
      )}
    </Container>
  );
}
