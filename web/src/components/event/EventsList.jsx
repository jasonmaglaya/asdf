import { Card, Row, Col } from "react-bootstrap";
import { NavLink } from "react-router-dom";
import { StyledCard } from "../_shared/StyledCard";
import { EventStatus } from "../../constants";

export default function EventsList({ events }) {
  return events?.map((event) => {
    return (
      <StyledCard bg="dark" text="white" className="h-100 m-1" key={event.id}>
        <Card.Header className="d-flex justify-content-between align-items-center p-1">
          {event.status === EventStatus.Active ? (
            <span className="text-danger fw-bolder">
              LIVE
              <span
                className="spinner-grow spinner-grow-sm text-success"
                role="status"
              ></span>
            </span>
          ) : (
            <span></span>
          )}
          <span className="text-secondary text-uppercase">
            {event.subTitle}
          </span>
        </Card.Header>
        <Card.Body>
          <Row>
            <Col md={8} className="d-flex align-items-center">
              <span className="h5">{event.title}</span>
            </Col>
            <Col md={2} className="d-flex align-items-center">
              <span className="h5">
                {new Date(event.eventDate).toLocaleDateString()}
              </span>
            </Col>
            <Col md={2} className="d-flex align-items-center mt-1">
              <NavLink
                to={`/events/${event.id}`}
                className="btn btn-primary btn-lg align-center form-control h-100"
              >
                {event.status === EventStatus.New ? "PREVIEW" : "ENTER"}
              </NavLink>
            </Col>
          </Row>
        </Card.Body>
      </StyledCard>
    );
  });
}
