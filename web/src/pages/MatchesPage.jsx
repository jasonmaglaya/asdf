import { useEffect, useState } from "react";
import { getEvent } from "../services/eventsService";
import MatchList from "../components/match/MatchList";
import { Container } from "react-bootstrap";
import SpinnerComponent from "../components/_shared/SpinnerComponent";
import { useParams } from "react-router-dom";

export default function MatchesPage() {
  const { eventId } = useParams();
  const [event, setEvent] = useState();
  const [showSpinner, setShowSpinner] = useState(true);

  useEffect(() => {
    const loadEvent = async () => {
      try {
        const { data } = await getEvent(eventId);
        setEvent(data?.result);
      } catch {
      } finally {
        setShowSpinner(false);
      }
    };

    loadEvent();
  }, [eventId]);

  return (
    <Container className="mt-3">
      <h5 className="text-light">{event?.title}</h5>
      {showSpinner ? (
        <SpinnerComponent />
      ) : (
        <MatchList
          eventId={eventId}
          teams={event?.teams}
          maxWinners={event?.maxWinners}
          allowDraw={event?.allowDraw}
        />
      )}
    </Container>
  );
}
