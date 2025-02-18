import { useState, useEffect, useRef } from "react";
import { Card, Form, Row, Col, InputGroup, Button } from "react-bootstrap";
import CurrencyInput from "react-currency-input-field";
import { useForm } from "react-hook-form";
import ConfirmDialog from "../components/_shared/ConfirmDialog";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCoins } from "@fortawesome/free-solid-svg-icons";
import { getUser, transferCredits } from "../services/usersService";
import { useParams } from "react-router-dom";
import { NavLink, useNavigate } from "react-router-dom";
import SearchUserDialog from "../components/user/SearchUserDialog";
import { useSelector } from "react-redux";

export default function TransferCreditPage() {
  const [isBusy, setIsBusy] = useState();
  const [showConfirmDialog, setShowConfirmDialog] = useState(false);
  const [showSearchUserDialog, setShowSearchUserDialog] = useState(false);
  const [handleConfirm, setHandleConfirm] = useState();
  const [user, setUser] = useState();
  const [from, setFrom] = useState();
  const { userId } = useParams();
  const amountRef = useRef();
  const transferCreditForm = useForm();
  const navigate = useNavigate();
  const { appSettings } = useSelector((state) => state.appSettings);
  const { currency, locale } = appSettings;

  const onValueChange = (amount) => {
    if (isNaN(amount) || amount > (from?.credits ?? 0)) {
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
    if (isNaN(amount) || amount > from?.credits) {
      transferCreditForm.setError(
        "amount",
        { type: "focus" },
        { shouldFocus: true }
      );
      return;
    }

    handleTransferCredits(form);
  };

  const handleTransferCredits = (form) => {
    setHandleConfirm(() => () => {
      setIsBusy(true);
      setShowConfirmDialog(false);
      var amount = Number(form.amount?.replace(/[^0-9.-]+/g, ""));
      transferCredits(user?.id, amount, from?.id).then(() => {
        navigate(`/users/${user?.id}`);
      });
    });

    setShowConfirmDialog(true);
  };

  const handleUserClick = (user) => {
    transferCreditForm.setValue("from", user.username);
    transferCreditForm.clearErrors("from");
    setFrom(user);
    setShowSearchUserDialog(false);
  };

  useEffect(() => {
    getUser(userId).then(({ data }) => {
      setUser(data.result);
    });
  }, [userId]);

  return (
    <>
      <Row className="gx-0">
        <Col md={6} className="p-1">
          <Card bg="dark" text="white">
            <Card.Header className="d-flex align-items-center p-2">
              <i className="text-warning p-1">
                <FontAwesomeIcon icon={faCoins} />
              </i>
              Transfer Credits
            </Card.Header>
            <Card.Body>
              <Form.Group className="mb-2" controlId="username">
                <Form.Label>Username</Form.Label>
                <Form.Label className="form-control form-control-lg">
                  {user?.username}
                </Form.Label>
              </Form.Group>
              <Form.Group className="mb-2" controlId="credits">
                <Form.Label>Credits</Form.Label>
                <Form.Label className="form-control form-control-lg">
                  {user?.credits?.toLocaleString(locale || "en-US", {
                    style: "currency",
                    currency: currency || "USD",
                  })}
                </Form.Label>
              </Form.Group>
              <Form.Group className="mb-2" controlId="credits">
                <Form.Label>From</Form.Label>
                <InputGroup>
                  <Form.Control
                    readOnly
                    className={
                      transferCreditForm.formState.errors?.from
                        ? "form-control form-control-lg invalid"
                        : "form-control form-control-lg"
                    }
                    {...transferCreditForm.register("from", {
                      required: true,
                      ref: amountRef,
                    })}
                  />
                  <Button
                    variant="secondary"
                    onClick={() => setShowSearchUserDialog(true)}
                  >
                    SEARCH
                  </Button>
                </InputGroup>
              </Form.Group>
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
                  <Button
                    id="btnClear"
                    variant="secondary"
                    onClick={clearAmount}
                  >
                    CLEAR
                  </Button>
                </InputGroup>
                {from ? (
                  <div className="form-text">
                    Amount must be less than or equal to{" "}
                    <span className="text-info">
                      {from?.credits?.toLocaleString(locale || "en-US", {
                        style: "currency",
                        currency: currency || "USD",
                      })}
                    </span>
                  </div>
                ) : (
                  <></>
                )}
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
                  <NavLink
                    to={`/users/${user?.id}`}
                    disabled={isBusy}
                    className="btn btn-secondary btn-lg form-control mb-2"
                  >
                    CANCEL
                  </NavLink>
                </Col>
              </Row>
            </Card.Body>
          </Card>
        </Col>
      </Row>
      <ConfirmDialog
        confirmButtonText="Confirm"
        handleConfirm={handleConfirm}
        handleClose={() => {
          setShowConfirmDialog(false);
        }}
        show={showConfirmDialog}
        isBusy={isBusy}
      >
        <h5>Are you sure?</h5>
      </ConfirmDialog>
      <SearchUserDialog
        show={showSearchUserDialog}
        handleUserClick={handleUserClick}
        handleClose={() => {
          setShowSearchUserDialog(false);
        }}
        currency={currency}
        locale={locale}
      />
    </>
  );
}
