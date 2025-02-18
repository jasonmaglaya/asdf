import { useState, useRef, useEffect } from "react";
import { useForm } from "react-hook-form";
import { Button, Form, Alert, Row, Col, Card } from "react-bootstrap";
import CurrencyInput from "react-currency-input-field";
import { addEvent, updateEvent } from "../../services/eventsService";
import { NavLink, useNavigate } from "react-router-dom";
import ConfirmDialog from "../_shared/ConfirmDialog";
import { EventStatus } from "../../constants";

export default function EventForm({ event, currency, locale }) {
  const [isBusy, setIsBusy] = useState(false);
  const [readOnly, setReadOnly] = useState(false);
  const [errors, setErrors] = useState([]);
  const [showErrors, setShowErrors] = useState(false);
  const [allowDraw, setAllowDraw] = useState(true);
  const {
    register,
    handleSubmit,
    formState,
    setError,
    clearErrors,
    setValue,
    reset,
    trigger,
  } = useForm();
  const minBetRef = useRef();
  const maxBetRef = useRef();
  const minDrawBetRef = useRef();
  const maxDrawBetRef = useRef();
  const navigate = useNavigate();
  const [showConfirmDialog, setShowConfirmDialog] = useState(false);

  const onSubmit = async (e) => {
    e.preventDefault();
    var title = await trigger("title");
    var eventDate = await trigger("eventDate");
    var video = await trigger("video");
    var commission = await trigger("commission");
    var minimumBet = await trigger("minimumBet");
    var maximumBet = await trigger("maximumBet");
    var minDrawBet = await trigger("minDrawBet");
    var maxDrawBet = await trigger("maxDrawBet");
    var drawMultiplier = await trigger("drawMultiplier");

    if (
      title &&
      eventDate &&
      video &&
      commission &&
      minimumBet &&
      maximumBet &&
      minDrawBet &&
      maxDrawBet &&
      maxDrawBet &&
      drawMultiplier
    ) {
      setShowConfirmDialog(true);
    }
  };

  const saveEvent = async (form) => {
    setIsBusy(true);
    setErrors([]);

    form.minimumBet = Number(form.minimumBet?.replace(/[^0-9.-]+/g, ""));
    form.maximumBet = Number(form.maximumBet?.replace(/[^0-9.-]+/g, ""));

    if (form.allowDraw) {
      if (form.minDrawBet) {
        form.minDrawBet = Number(form.minDrawBet?.replace(/[^0-9.-]+/g, ""));
      }

      if (form.maxDrawBet) {
        form.maxDrawBet = Number(form.maxDrawBet?.replace(/[^0-9.-]+/g, ""));
      }
    } else {
      form.minDrawBet = null;
      form.maxDrawBet = null;
    }

    form.commission = Number(form.commission) / 100;

    // Defaults - tech dept
    form.maxWinners = 1;
    form.allowAgentBetting = false;
    form.allowAdminBetting = false;
    form.padding = 0;

    const teams = [
      {
        code: "M",
        name: "MERON",
        color: "#e50914",
        sequence: 1,
      },
      {
        code: "W",
        name: "WALA",
        color: "#3d9ae8",
        sequence: 2,
      },
    ];

    const request = {
      event: form,
      teams,
    };

    if (event) {
      request.event.id = event.id;

      updateEvent(request)
        .then(({ data }) => {
          if (data.isSuccessful) {
            navigate("/events");
          }
        })
        .catch(({ response }) => {
          setErrors(response?.data.errors);
          setShowErrors(true);
        })
        .finally(() => {
          setIsBusy(false);
        });
    } else {
      addEvent(request)
        .then(({ data }) => {
          if (data.isSuccessful) {
            navigate("/events");
          }
        })
        .catch(({ response }) => {
          setErrors(response?.data.errors);
          setShowErrors(true);
        })
        .finally(() => {
          setIsBusy(false);
        });
    }
  };

  const onMinBetChanged = (amount) => {
    if (isNaN(amount) || amount <= 0) {
      setError("minimumBet");
      return;
    }

    clearErrors("minimumBet");
  };

  const onMaxBetChanged = (amount) => {
    if (isNaN(amount) || amount <= 0) {
      setError("maximumBet");
      return;
    }

    clearErrors("maximumBet");
  };

  const onMinDrawBetChanged = (amount) => {
    if (isNaN(amount) || amount <= 0) {
      setError("minDrawBet");
      return;
    }

    clearErrors("minDrawBet");
  };

  const onMaxDrawBetChanged = (amount) => {
    if (isNaN(amount) || amount <= 0) {
      setError("maxDrawBet");
      return;
    }

    clearErrors("maxDrawBet");
  };

  const onAllowDrawChanged = (allowDraw) => {
    if (!allowDraw) {
      clearErrors("minDrawBet");
      clearErrors("maxDrawBet");
      clearErrors("drawMultiplier");
      setValue("minDrawBet", "");
      setValue("maxDrawBet", "");
      setValue("drawMultiplier", "");
    }

    setAllowDraw(allowDraw);
  };

  useEffect(() => {
    if (event) {
      setValue("title", event.title);
      setValue("subTitle", event.subTitle);
      const date = new Date(event.eventDate);
      const formattedDate = `${date.getFullYear()}-${String(
        date.getMonth() + 1
      ).padStart(2, "0")}-${String(date.getDate()).padStart(2, "0")}`;
      setValue("eventDate", formattedDate);
      setValue("video", event.video);
      setValue(
        "minimumBet",
        event.minimumBet?.toLocaleString(locale || "en-US", {
          style: "currency",
          currency: currency || "USD",
        })
      );
      setValue(
        "maximumBet",
        event.maximumBet?.toLocaleString(locale || "en-US", {
          style: "currency",
          currency: currency || "USD",
        })
      );
      setValue("commission", event.commission * 100);
      setValue("allowDraw", event.allowDraw);
      setValue(
        "minDrawBet",
        event.minDrawBet?.toLocaleString(locale || "en-US", {
          style: "currency",
          currency: currency || "USD",
        })
      );
      setValue(
        "maxDrawBet",
        event.maxDrawBet?.toLocaleString(locale || "en-US", {
          style: "currency",
          currency: currency || "USD",
        })
      );
      setValue("drawMultiplier", event.drawMultiplier);
    } else {
      reset();
    }

    setReadOnly(event?.status === EventStatus.Closed);
  }, [event, setValue, reset, currency, locale]);

  return (
    <>
      <Card bg="dark" text="white">
        <Card.Header>
          <h4>New Event</h4>
        </Card.Header>
        <Card.Body>
          <Alert
            variant="danger"
            show={showErrors}
            onClose={() => setShowErrors(false)}
            dismissible
          >
            <ul className="m-0">
              {errors?.map((err) => (
                <li key={err} style={{ listStyleType: "none" }}>
                  {err}
                </li>
              ))}
            </ul>
          </Alert>
          <Form onSubmit={onSubmit}>
            <Row className="mb-4">
              <Col md={6}>
                <Form.Group className="mb-2" controlId="title">
                  <Form.Label className="text-center">Title</Form.Label>
                  <Form.Control
                    size="lg"
                    maxLength={100}
                    autoComplete="off"
                    autoCapitalize="none"
                    readOnly={readOnly}
                    {...register("title", {
                      required: true,
                      maxLength: 100,
                    })}
                    className={
                      formState.errors?.title
                        ? "form-control form-control-lg invalid"
                        : "form-control form-control-lg"
                    }
                  />
                </Form.Group>
                <Form.Group className="mb-2" controlId="title">
                  <Form.Label className="text-center">Sub-Title</Form.Label>
                  <Form.Control
                    size="lg"
                    maxLength={100}
                    autoComplete="off"
                    autoCapitalize="none"
                    readOnly={readOnly}
                    {...register("subTitle", {
                      maxLength: 100,
                    })}
                    className={
                      formState.errors?.subTitle
                        ? "form-control form-control-lg invalid"
                        : "form-control form-control-lg"
                    }
                  />
                </Form.Group>
                <Form.Group className="mb-2" controlId="title">
                  <Form.Label className="text-center">Date</Form.Label>
                  <Form.Control
                    type="date"
                    size="lg"
                    autoComplete="off"
                    autoCapitalize="none"
                    readOnly={readOnly}
                    {...register("eventDate", { required: true })}
                    className={
                      formState.errors?.eventDate
                        ? "form-control form-control-lg invalid"
                        : "form-control form-control-lg"
                    }
                  />
                </Form.Group>
                <Form.Group className="mb-2" controlId="video">
                  <Form.Label className="text-center">
                    Video Embed Script
                  </Form.Label>
                  <Form.Control
                    as="textarea"
                    rows={4}
                    size="lg"
                    maxLength={1000}
                    autoComplete="off"
                    autoCapitalize="none"
                    readOnly={readOnly}
                    {...register("video", {
                      required: true,
                      maxLength: 1000,
                    })}
                    className={
                      formState.errors?.video
                        ? "form-control form-control-lg invalid"
                        : "form-control form-control-lg"
                    }
                  />
                </Form.Group>
                <Form.Group className="mb-2" controlId="commission">
                  <Form.Label>Commission (%)</Form.Label>
                  <Form.Control
                    type="number"
                    step="0.01"
                    max={100}
                    min={1}
                    size="lg"
                    readOnly={readOnly}
                    {...register("commission", {
                      required: true,
                      min: 1,
                      max: 100,
                    })}
                    className={
                      formState.errors?.commission
                        ? "form-control form-control-lg invalid"
                        : "form-control form-control-lg"
                    }
                  />
                </Form.Group>
              </Col>
              <Col md={6}>
                <Form.Group className="mb-2" controlId="minimumBet">
                  <Form.Label>Minimum Bet</Form.Label>
                  <CurrencyInput
                    autoComplete="off"
                    onValueChange={onMinBetChanged}
                    readOnly={readOnly}
                    className={
                      formState.errors?.minimumBet
                        ? "form-control form-control-lg invalid"
                        : "form-control form-control-lg"
                    }
                    decimalScale={2}
                    allowNegativeValue={false}
                    intlConfig={{ locale, currency }}
                    {...register("minimumBet", {
                      required: true,
                      ref: minBetRef,
                    })}
                  />
                </Form.Group>
                <Form.Group className="mb-2" controlId="maxBet">
                  <Form.Label>Maximum Bet</Form.Label>
                  <CurrencyInput
                    autoComplete="off"
                    onValueChange={onMaxBetChanged}
                    readOnly={readOnly}
                    className={
                      formState.errors?.maximumBet
                        ? "form-control form-control-lg invalid"
                        : "form-control form-control-lg"
                    }
                    decimalScale={2}
                    allowNegativeValue={false}
                    intlConfig={{ locale, currency }}
                    {...register("maximumBet", {
                      required: true,
                      ref: maxBetRef,
                    })}
                  />
                </Form.Group>
                <Form.Group className="mb-2" controlId="allowDraw">
                  <Form.Check
                    type="switch"
                    label="Alow Bets on Draw"
                    disabled={readOnly}
                    {...register("allowDraw", {
                      value: true,
                    })}
                    onChange={(e) => {
                      onAllowDrawChanged(e.target.checked);
                    }}
                  />
                </Form.Group>
                <Form.Group className="mb-2" controlId="minDrawBet">
                  <Form.Label>Minimum Bet on Draw</Form.Label>
                  <CurrencyInput
                    autoComplete="off"
                    readOnly={readOnly}
                    onValueChange={onMinDrawBetChanged}
                    className={
                      formState.errors?.minDrawBet
                        ? "form-control form-control-lg invalid"
                        : "form-control form-control-lg"
                    }
                    decimalScale={2}
                    allowNegativeValue={false}
                    intlConfig={{ locale, currency }}
                    {...register("minDrawBet", {
                      required: allowDraw,
                      ref: minDrawBetRef,
                    })}
                  />
                </Form.Group>
                <Form.Group className="mb-2" controlId="maxDrawBet">
                  <Form.Label>Maximum Bet on Draw</Form.Label>
                  <CurrencyInput
                    readOnly={readOnly}
                    autoComplete="off"
                    onValueChange={onMaxDrawBetChanged}
                    className={
                      formState.errors?.maxDrawBet
                        ? "form-control form-control-lg invalid"
                        : "form-control form-control-lg"
                    }
                    decimalScale={2}
                    allowNegativeValue={false}
                    intlConfig={{ locale, currency }}
                    {...register("maxDrawBet", {
                      required: allowDraw,
                      ref: maxDrawBetRef,
                    })}
                  />
                </Form.Group>
                <Form.Group className="mb-2" controlId="drawMultiplier">
                  <Form.Label>Draw Bet Multiplier</Form.Label>
                  <Form.Control
                    readOnly={readOnly}
                    type="number"
                    step="0.01"
                    max={100}
                    min={1}
                    size="lg"
                    {...register("drawMultiplier", {
                      required: allowDraw,
                      min: 1,
                      max: 100,
                    })}
                    className={
                      formState.errors?.drawMultiplier
                        ? "form-control form-control-lg invalid"
                        : "form-control form-control-lg"
                    }
                  />
                </Form.Group>
              </Col>
            </Row>
            <Row>
              <Col md={6}>
                <div className="d-grid">
                  <NavLink to="/events" className="btn btn-secondary btn-lg">
                    Back
                  </NavLink>
                </div>
              </Col>
              <Col md={6}>
                {event?.status !== EventStatus.Closed && (
                  <div className="d-grid mt-2 mt-md-0">
                    <Button
                      variant="primary"
                      type="submit"
                      size="lg"
                      disabled={isBusy}
                    >
                      {!isBusy ? (
                        <span>Save</span>
                      ) : (
                        <span>
                          <span
                            className="spinner-grow spinner-grow-sm"
                            role="status"
                          ></span>
                          <span>Saving...</span>
                        </span>
                      )}
                    </Button>
                  </div>
                )}
              </Col>
            </Row>
          </Form>
        </Card.Body>
      </Card>
      <ConfirmDialog
        title={"Save Event"}
        confirmButtonText="Save"
        confirmButtonTextBusy="Saving..."
        handleConfirm={() => handleSubmit(saveEvent)()}
        handleClose={() => {
          setShowConfirmDialog(false);
        }}
        show={showConfirmDialog}
        isBusy={isBusy}
      >
        <span className="h5">Do you want to save?</span>
      </ConfirmDialog>
    </>
  );
}
