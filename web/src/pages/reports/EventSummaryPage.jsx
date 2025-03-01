import { Breadcrumb, Container, Tab, Tabs } from "react-bootstrap";
import SpinnerComponent from "../../components/_shared/SpinnerComponent";
import { useEffect, useState } from "react";
import { NavLink, useNavigate, useParams } from "react-router-dom";
import { useSelector } from "react-redux";
import { getEventSummary } from "../../services/reportsService";
import EventSummary from "../../components/reports/EventSummary";
import Trend from "../../components/event/Trend";
import { getEvent, getWinners } from "../../services/eventsService";

export default function EventSummaryPage() {
  const [isLoading, setIsLoading] = useState(true);
  const [summary, setSummary] = useState([]);
  const { appSettings } = useSelector((state) => state.appSettings);
  const { currency, locale } = appSettings;
  const { eventId } = useParams();
  const navigate = useNavigate();
  const [winnersList, setWinnersList] = useState([]);
  const [event, setEvent] = useState({});

  if (!eventId) {
    navigate("/reports/events");
  }

  const loadEvent = async (id) => {
    try {
      const { data } = await getEvent(id);
      setEvent(data.result);
    } catch {}
  };

  const getWinnersList = async (id) => {
    try {
      const { data } = await getWinners(id);
      setWinnersList(data?.result);
    } catch {}
  };

  const getSummary = async (id) => {
    try {
      setIsLoading(true);

      const { data } = await getEventSummary(id);

      const sum = data.result.reduce((acc, item) => {
        const {
          matchId,
          number,
          winners,
          totalBets,
          commission,
          totalDraw,
          totalDrawPayout,
          drawNet,
          declaredBy,
          declareDate,
          ipAddress,
        } = item;
        const existingGroup = acc.find((x) => x.matchId === matchId);

        if (existingGroup) {
          existingGroup.winners = winners;
          existingGroup.totalBets = totalBets;
          existingGroup.commission = commission;
          existingGroup.totalDraw = totalDraw;
          existingGroup.totalDrawPayout = totalDrawPayout;
          existingGroup.drawNet = drawNet;
          existingGroup.declarations.push({
            winners,
            declaredBy,
            declareDate,
            ipAddress,
            totalBets,
            commission,
            totalDraw,
            totalDrawPayout,
            drawNet,
          });
        } else {
          acc.push({
            matchId,
            number,
            winners,
            totalBets,
            commission,
            totalDraw,
            totalDrawPayout,
            drawNet,
            declarations: [
              {
                winners,
                declaredBy,
                declareDate,
                ipAddress,
                totalBets,
                commission,
                totalDraw,
                totalDrawPayout,
                drawNet,
              },
            ],
          });
        }

        return acc;
      }, []);

      setSummary(sum);
    } catch {
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (eventId) {
      loadEvent(eventId);
      getSummary(eventId);
      getWinnersList(eventId);
    }
  }, [eventId]);

  return (
    <Container className="mt-2 text-light">
      <h3>Events Summary</h3>
      <Breadcrumb>
        <Breadcrumb.Item>
          <NavLink to="/reports/events">Events</NavLink>
        </Breadcrumb.Item>
        <Breadcrumb.Item className="text-light" active>
          {event.title}
        </Breadcrumb.Item>
      </Breadcrumb>
      {isLoading ? (
        <SpinnerComponent />
      ) : (
        <Tabs
          defaultActiveKey="summary"
          id="event-summary-tabs"
          className="mb-3"
        >
          <Tab eventKey="summary" title="Summary">
            <EventSummary
              summary={summary}
              currency={currency}
              locale={locale}
              event={event}
            />
          </Tab>
          <Tab eventKey="trend" title="Trend">
            <Trend winners={winnersList} />
          </Tab>
        </Tabs>
      )}
    </Container>
  );
}
