import { Modal, InputGroup, Button, Form, Row, Col } from "react-bootstrap";
import CurrencyInput from "react-currency-input-field";
import { useForm } from "react-hook-form";
import { useSelector } from "react-redux";
import { Features } from "../../constants";

export default function TransferCreditDialog({
  show,
  isBusy,
  handleClose,
  handleSubmit,
  currency,
  locale,
}) {
  const transferCreditForm = useForm();
  const { credits, features } = useSelector((state) => state.user);

  const onValueChange = (amount) => {
    if (
      isNaN(amount) ||
      (amount > credits && !features.includes(Features.PropagateCredits))
    ) {
      transferCreditForm.setError("amount");
      return;
    }

    transferCreditForm.clearErrors("amount");
  };

  const clearAmount = async () => {
    await transferCreditForm.reset({ amount: "" });
    transferCreditForm.setFocus("amount");
  };

  const onSubmit = () => {
    transferCreditForm.handleSubmit(confirmTransfer)();
  };

  const confirmTransfer = (form) => {
    var amount = Number(form.amount?.replace(/[^0-9.-]+/g, ""));
    if (
      isNaN(amount) ||
      (amount > credits && !features.includes(Features.PropagateCredits))
    ) {
      transferCreditForm.setError(
        "amount",
        { type: "focus" },
        { shouldFocus: true }
      );
      return;
    }

    if (handleSubmit) {
      handleSubmit(form);
    }
  };

  return (
    <>
      <Modal
        show={show}
        backdrop="static"
        keyboard={false}
        centered
        animation={false}
      >
        <Modal.Header>
          <Modal.Title>Top Up</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form.Group className="mb-2" controlId="amount">
            <Form.Label>Amount</Form.Label>
            <InputGroup className="mb-1">
              <CurrencyInput
                onValueChange={onValueChange}
                className={
                  transferCreditForm.formState.errors?.amount
                    ? "form-control form-control-lg invalid"
                    : "form-control form-control-lg"
                }
                decimalScale={2}
                allowNegativeValue={false}
                intlConfig={{ locale, currency }}
                {...transferCreditForm.register("amount", {
                  required: true,
                })}
                autoComplete="off"
              />
              <Button variant="secondary" onClick={clearAmount}>
                CLEAR
              </Button>
            </InputGroup>
          </Form.Group>
          <Row className="pt-3 gx-3">
            <Col>
              <Button
                variant="primary"
                size="lg"
                className="form-control"
                disabled={isBusy}
                onClick={onSubmit}
              >
                {!isBusy ? (
                  <span>SUBMIT</span>
                ) : (
                  <span>
                    <span
                      className="spinner-grow spinner-grow-sm"
                      role="status"
                    ></span>
                    <span>SUBMITTING</span>
                  </span>
                )}
              </Button>
            </Col>
            <Col>
              <Button
                variant="secondary"
                size="lg"
                className="form-control"
                disabled={isBusy}
                onClick={handleClose}
              >
                CANCEL
              </Button>
            </Col>
          </Row>
        </Modal.Body>
      </Modal>
    </>
  );
}
