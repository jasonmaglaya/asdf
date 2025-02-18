import EventsMaintenance from "../components/event/EventsMaintenance";
import { NavLink } from "react-router-dom";

export default function EventsMaintenancePage() {
  return (
    <div className="p-2">
      <div className="d-flex justify-content-between align-items-center text-light">
        <h4>Events Maintenance</h4>
        <NavLink to="/events/new" className="btn btn-primary mb-1 btn-lg">
          NEW EVENT
        </NavLink>
      </div>
      <EventsMaintenance />
    </div>
  );
}
