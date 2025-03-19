import { Card, Row, Col, Button } from "react-bootstrap";
import { NavLink } from "react-router-dom";
import { StyledCard } from "../_shared/StyledCard";
import { EventStatus } from "../../constants";
import { getAllEvents, updateEventStatus } from "../../services/eventsService";
import { useNavigate } from "react-router-dom";
import ConfirmDialog from "../_shared/ConfirmDialog";
import { useEffect, useState } from "react";
import SpinnerComponent from "../_shared/SpinnerComponent";

export default function EventsMaintenance() {
  const navigate = useNavigate();
  const [events, setEvents] = useState([]);
  const [showSpinner, setShowSpinner] = useState(true);
  const [isBusy, setIsBusy] = useState(false);
  const [showConfirmDialog, setShowConfirmDialog] = useState(false);
  const [eventId, setEventId] = useState();
  const [title, setTitle] = useState();
  const [confirmationMessage, setConfirmationMessage] = useState();
  const [buttonText, setButtonText] = useState();
  const [confirmationAction, setConfirmationAction] = useState(() => {});

  const goLive = (id) => {
    setIsBusy(true);
    updateEventStatus(id, EventStatus.Active).then(() => {
      setIsBusy(false);
      setShowConfirmDialog(false);
      navigate(`/events/${id}`);
    });
  };

  const closeEvent = (id) => {
    setIsBusy(true);
    updateEventStatus(id, EventStatus.Closed).then(() => {
      setIsBusy(false);
      setShowConfirmDialog(false);
      loadEvents();
    });
  };

  const lockEvent = (id) => {
    setIsBusy(true);
    updateEventStatus(id, EventStatus.Final).then(() => {
      setIsBusy(false);
      setShowConfirmDialog(false);
      loadEvents();
    });
  };

  const confirmLive = (eventId, eventTitle) => {
    setShowConfirmDialog(true);
    setEventId(eventId);
    setTitle(eventTitle);
    setConfirmationMessage("GO LIVE");
    setButtonText("GO LIVE");
    setConfirmationAction(() => goLive);
  };

  const confirmClose = (eventId, eventTitle) => {
    setEventId(eventId);
    setTitle(eventTitle);
    setConfirmationMessage("CLOSE EVENT");
    setButtonText("CLOSE EVENT");
    setConfirmationAction(() => closeEvent);
    setShowConfirmDialog(true);
  };

  const confirmLock = (eventId, eventTitle) => {
    setEventId(eventId);
    setTitle(eventTitle);
    setConfirmationMessage("LOCK EVENT");
    setButtonText("LOCK EVENT");
    setConfirmationAction(() => lockEvent);
    setShowConfirmDialog(true);
  };

  const loadEvents = async () => {
    try {
      const { data } = await getAllEvents(null, 1, 10);
      setEvents(data?.result?.list || []);
      setIsBusy(false);
      setShowSpinner(false);
    } catch {
    } finally {
      setIsBusy(false);
      setShowSpinner(false);
    }
  };

  useEffect(() => {
    loadEvents();
  }, []);

  return showSpinner ? (
    <SpinnerComponent />
  ) : (
    events?.map((event) => {
      return (
        <div key={event.id}>
          <StyledCard
            bg="dark"
            text="white"
            className="h-100 m-1"
            key={event.id}
          >
            <Card.Header className="d-flex justify-content-between align-items-center p-1">
              {event?.status === EventStatus.Active ? (
                <span className="text-success fw-bolder">
                  LIVE{" "}
                  <span
                    className="spinner-grow spinner-grow-sm text-danger"
                    role="status"
                  ></span>
                </span>
              ) : [EventStatus.Closed, EventStatus.Final].includes(
                  event?.status
                ) ? (
                <span className="text-danger fw-bolder text-uppercase">
                  {event?.status}
                </span>
              ) : (
                <span className="text-warning fw-bolder text-uppercase">
                  {event?.status}
                </span>
              )}
              <span className="text-secondary text-uppercase">
                {event.subTitle}
              </span>
            </Card.Header>
            <Card.Body>
              <Row>
                <Col md={8} className="d-flex align-items-center">
                  <NavLink
                    to={`/events/${event.id}`}
                    className={
                      "link-light link-underline link-underline-opacity-0 link-underline-opacity-75-hover"
                    }
                  >
                    <span className="h5">{event.title}</span>
                  </NavLink>
                </Col>
                <Col md={4}>
                  <Row>
                    {event?.status !== EventStatus.Closed && (
                      <Col className="mt-2 col-12 col-xl-3">
                        <NavLink
                          to={`/events/${event.id}/edit`}
                          className="btn btn-secondary btn-lg align-center form-control h-100"
                        >
                          EDIT
                        </NavLink>
                      </Col>
                    )}
                    <Col className="mt-2 col-12 col-xl-3">
                      <NavLink
                        to={`/events/${event.id}/matches`}
                        className="btn btn-info btn-lg align-center form-control"
                      >
                        MATCHES
                      </NavLink>
                    </Col>
                    <Col className="mt-2 col-12 col-xl-3">
                      {[EventStatus.New, EventStatus.Closed].includes(
                        event?.status
                      ) && (
                        <Button
                          className="btn btn-success btn-lg align-center form-control h-100"
                          onClick={() => confirmLive(event.id, event.title)}
                        >
                          GO LIVE
                        </Button>
                      )}
                      {event?.status === EventStatus.Active && (
                        <Button
                          className="btn btn-danger btn-lg align-center form-control h-100 text-light"
                          onClick={() => confirmClose(event.id, event.title)}
                        >
                          CLOSE
                        </Button>
                      )}
                    </Col>
                    <Col className="mt-2 col-12 col-xl-3">
                      {[EventStatus.Active, EventStatus.Closed].includes(
                        event?.status
                      ) && (
                        <Button
                          className="btn btn-danger btn-lg align-center form-control h-100 text-light"
                          onClick={() => confirmLock(event.id, event.title)}
                        >
                          LOCK
                        </Button>
                      )}
                    </Col>
                  </Row>
                </Col>
              </Row>
            </Card.Body>
          </StyledCard>
          <ConfirmDialog
            title={title}
            confirmButtonText={buttonText}
            confirmButtonTextBusy="Please wait..."
            handleConfirm={() => confirmationAction(eventId)}
            handleClose={() => {
              setShowConfirmDialog(false);
            }}
            show={showConfirmDialog}
            isBusy={isBusy}
          >
            <span className="h5">
              Are you sure you want to{" "}
              <span className="text-danger">{confirmationMessage}</span>?
            </span>
          </ConfirmDialog>
        </div>
      );
    })
  );
}
