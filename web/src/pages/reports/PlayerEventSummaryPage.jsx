import { useState, useEffect } from "react";
import { getLastEvents, getWinners } from "../../services/eventsService";
import { Container, Form, Tab, Tabs } from "react-bootstrap";
import SpinnerComponent from "../../components/_shared/SpinnerComponent";
import PlayerEventSummary from "../../components/reports/PlayerEventSummary";
import Trend from "../../components/event/Trend";
import { useSelector } from "react-redux";
import { getPlayerEventSummary } from "../../services/reportsService";

export default function PlayerEventSummaryPage() {
  const [events, setEvents] = useState([]);
  const [summary, setSummary] = useState([]);
  const [eventId, setEventId] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [winnersList, setWinnersList] = useState([]);
  const { appSettings } = useSelector((state) => state.appSettings);
  const { currency, locale } = appSettings;

  const onEventChange = (e) => {
    setEventId(e.target.value);
  };

  const loadSummary = async (id) => {
    try {
      setIsLoading(true);

      const { data } = await getPlayerEventSummary(id);

      setSummary(data.result);
    } catch {
    } finally {
      setIsLoading(false);
    }
  };

  const getWinnersList = async (id) => {
    try {
      const { data } = await getWinners(id);
      setWinnersList(data?.result);
    } catch {}
  };

  useEffect(() => {
    const loadLastEvents = async () => {
      try {
        const { data } = await getLastEvents();
        setEvents(data.result.list);
        setEventId(data.result.list[0]?.id);
      } catch {}
    };

    loadLastEvents();
  }, []);

  useEffect(() => {
    if (eventId) {
      loadSummary(eventId);
      getWinnersList(eventId);
    }
  }, [eventId]);

  return (
    <Container className="mt-2 text-light px-3" fluid>
      <h3>Event Summary</h3>
      <Form.Select onChange={onEventChange} className="mb-3">
        {events.map((event) => (
          <option key={event.id} value={event.id}>
            {event.title}
          </option>
        ))}
      </Form.Select>
      {isLoading ? (
        <SpinnerComponent />
      ) : (
        <Tabs
          defaultActiveKey="summary"
          id="event-summary-tabs"
          className="mb-3"
        >
          <Tab eventKey="summary" title="Summary">
            <PlayerEventSummary
              history={summary}
              currency={currency}
              locale={locale}
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
