import { Container } from "react-bootstrap";
import EventForm from "../components/event/EventForm";
import { useSelector } from "react-redux";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { getEvent } from "../services/eventsService";
import SpinnerComponent from "../components/_shared/SpinnerComponent";
import { Features } from "../constants";

export default function NewEventPage() {
  const [isBusy, setIsBusy] = useState(true);
  const { appSettings } = useSelector((state) => state.appSettings);
  const { features, loadingFeatures } = useSelector((state) => state.user);
  const { currency, locale } = appSettings;
  const [event, setEvent] = useState();
  const { eventId } = useParams();
  const navigate = useNavigate();

  if (!features.includes(Features.MaintainEvents) && !loadingFeatures) {
    navigate("/", { replace: true });
  }

  useEffect(() => {
    if (eventId) {
      getEvent(eventId).then(({ data }) => {
        if (!data.isSuccessful) {
          return;
        }
        setEvent(data.result);
        setIsBusy(false);
      });
    } else {
      setIsBusy(false);
    }
  }, [eventId]);

  return (
    <Container className="mt-2">
      {isBusy ? (
        <SpinnerComponent />
      ) : (
        <EventForm event={event} currency={currency} locale={locale} />
      )}
    </Container>
  );
}
