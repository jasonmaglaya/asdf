import { useState, useEffect } from "react";
import EventsList from "../event/EventsList";
import { getEvents } from "../../services/eventsService";
import SpinnerComponent from "../_shared/SpinnerComponent";

export default function Home() {
  const [events, setEvents] = useState([]);
  const [isBusy, setIsBusy] = useState(true);

  useEffect(() => {
    getEvents(1, 10)
      .then(({ data }) => {
        setEvents(data?.result?.list || []);
        setIsBusy(false);
      })
      .catch(() => {});
  }, []);

  return isBusy ? <SpinnerComponent /> : <EventsList events={events} />;
}
