import { Card, Form, Row, Col, Button } from "react-bootstrap";
import { useEffect, useState } from "react";
import {
  updateStatus,
  updateRole,
  updateAgency,
  getUser,
  transferCredits,
} from "../../services/usersService";
import { getRoles } from "../../services/rolesService";
import { DownLines, Features } from "../../constants";
import { useSelector, useDispatch } from "react-redux";
import { useForm } from "react-hook-form";
import ConfirmDialog from "../_shared/ConfirmDialog";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faUser, faUsers } from "@fortawesome/free-solid-svg-icons";
import TransferCreditDialog from "./TransferCreditDialog";
import { NavLink } from "react-router-dom";
import { setCredits } from "../../store/userSlice";

export default function User({ user, setUser }) {
  const [isBusy, setIsBusy] = useState();
  const [isBusyActivating, setIsBusyActivating] = useState();
  const [isActive, setIsActive] = useState(false);
  const [isAgent, setIsAgent] = useState(true);
  const [isActivatingAgency, setIsActivatingAgency] = useState();
  const { user: currentUser, features } = useSelector((state) => state.user);
  const agencyForm = useForm();
  const dispatch = useDispatch();
  const [showConfirmDialog, setShowConfirmDialog] = useState(false);
  const [handleConfirm, setHandleConfirm] = useState();
  const [showTransferDialog, setShowTransferDialog] = useState();
  const [roles, setRoles] = useState([]);
  const [selectedRole, setSelectedRole] = useState();
  const { appSettings } = useSelector((state) => state.appSettings);

  const onCheckChange = (e) => {
    const status = e.target.checked;
    setHandleConfirm(() => () => updateUserStatus(status));
    setShowConfirmDialog(true);
  };

  const handleSubmit = (form) => {
    let commission = form.commission;
    if (!commission) {
      agencyForm.setError(
        "commission",
        { type: "focus" },
        { shouldFocus: true }
      );
      return;
    }

    commission = commission / 100;
    if (
      commission >= (user?.uplineUser?.commission ?? appSettings?.commission)
    ) {
      agencyForm.setError(
        "commission",
        { type: "focus" },
        { shouldFocus: true }
      );
      return;
    }

    setHandleConfirm(() => () => activateAgency());
    setShowConfirmDialog(true);
  };

  const updateUserStatus = (status) => {
    setIsBusy(true);
    updateStatus(user?.id, status)
      .then(() => {
        setIsActive(status);
      })
      .finally(() => {
        setIsBusy(false);
        setShowConfirmDialog(false);
      });
  };

  const activateAgency = () => {
    setShowConfirmDialog(false);
    setIsBusyActivating(true);

    const commission = agencyForm.getValues("commission") / 100;
    updateAgency(user?.id, commission).then(() => {
      getUser(user?.id)
        .then(({ data }) => {
          setIsAgent(true);
          setIsActivatingAgency(false);
          setUser(data.result);
        })
        .finally(() => {
          setIsBusyActivating(false);
        });
    });
  };

  const handleTopUpCredits = (e) => {
    setShowTransferDialog(true);
    e.target.blur();
  };

  const handleTransferCredits = (form) => {
    setHandleConfirm(() => () => {
      setIsBusy(true);
      setShowConfirmDialog(false);
      var amount = Number(form.amount?.replace(/[^0-9.-]+/g, ""));
      transferCredits(user?.id, amount).then(() => {
        getUser(user?.id)
          .then(({ data }) => {
            setUser(data.result);
            setShowTransferDialog(false);
          })
          .finally(() => {
            setIsBusy(false);
          });

        getUser().then(({ data }) => {
          dispatch(setCredits(data.result?.credits));
        });
      });
    });

    setShowConfirmDialog(true);
  };

  const handleRoleChanged = (e) => {
    setSelectedRole(e.target.value);
  };

  const handleUpdateClicked = (e) => {
    setHandleConfirm(() => () => {
      setIsBusy(true);
      updateRole(user?.id, selectedRole).then(() => {
        getUser(user?.id)
          .then(({ data }) => {
            setUser(data.result);
            setShowConfirmDialog(false);
          })
          .finally(() => {
            setIsBusy(false);
          });
      });
    });

    setShowConfirmDialog(true);
    e.target.blur();
  };

  useEffect(() => {
    setIsActive(user?.isActive);
    setSelectedRole(user?.role);
    setIsAgent(user?.userRole.isAgent);

    if (user?.commission) {
      agencyForm.setValue("commission", (user?.commission * 100).toFixed(2), {
        shouldValidate: true,
        shouldDirty: true,
      });
    }

    getRoles().then(({ data }) => {
      setRoles(data.result.filter((x) => !x.isAgent));
    });
  }, [user, agencyForm]);

  return (
    <>
      <Row className="gx-0">
        <Col md={6} className="p-1">
          <Card bg="dark" text="white">
            <Card.Header className="d-flex align-items-center p-2">
              <i className="text-info p-1">
                <FontAwesomeIcon icon={faUser} />
              </i>
              User
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
                  {user?.credits?.toLocaleString(
                    appSettings.locale || "en-US",
                    {
                      style: "currency",
                      currency: appSettings.currency || "USD",
                    }
                  )}
                </Form.Label>
              </Form.Group>
              {user?.id !== currentUser?.id &&
              (features.includes(Features.ActivateUser) ||
                user?.uplineUser.id === currentUser?.id) ? (
                <Form.Group className="mb-2" controlId="active">
                  <Form.Label>Active</Form.Label>
                  <div
                    className="form-check form-switch form-control-lg"
                    style={{ paddingTop: 0 }}
                  >
                    <input
                      className="form-check-input"
                      type="checkbox"
                      id="isActive"
                      checked={isActive ?? false}
                      onChange={(e) => onCheckChange(e)}
                      disabled={isBusy}
                    />
                    {isBusy ? (
                      <span
                        className="spinner-grow spinner-grow-sm text-primary"
                        role="status"
                      ></span>
                    ) : (
                      <></>
                    )}
                  </div>
                </Form.Group>
              ) : (
                <></>
              )}

              {user?.id !== currentUser?.id &&
              !user?.userRole?.isAgent &&
              !user?.uplineUser &&
              !isAgent &&
              features?.includes(Features.UpdateRole) ? (
                <>
                  <Form.Group className="mb-2" controlId="active">
                    <Form.Label>Role</Form.Label>
                    <Form.Select
                      size="lg"
                      className="form-control"
                      onChange={handleRoleChanged}
                      value={selectedRole}
                    >
                      {roles?.map((r) => {
                        return (
                          <option value={r.code} key={r.code}>
                            {r.name}
                          </option>
                        );
                      })}
                    </Form.Select>
                  </Form.Group>
                  <Button
                    className="btn btn-primary btn-lg form-control mb-2"
                    onClick={handleUpdateClicked}
                    disabled={isBusy}
                  >
                    UPDATE
                  </Button>
                </>
              ) : (
                <></>
              )}
              <NavLink
                className="btn btn-secondary btn-lg form-control mb-2"
                to="/users"
                disabled={isBusy}
              >
                BACK
              </NavLink>
              {/* {user?.uplineUser?.id === currentUser?.id ||
              features.includes(Features.TopUp) ? (
                <Button
                  className="btn btn-success btn-lg form-control mb-2"
                  disabled={isBusy}
                  onClick={handleTopUpCredits}
                >
                  TOP UP
                </Button>
              ) : (
                <></>
              )}
              {features.includes(Features.TransferCredits) ? (
                <NavLink
                  to="transfer-credits"
                  disabled={isBusy}
                  className="btn btn-info btn-lg form-control mb-2"
                >
                  TRANSFER CREDITS
                </NavLink>
              ) : (
                <></>
              )}
              {user?.uplineUser?.id === currentUser?.id ||
              features.includes(Features.DeleteUser) ? (
                <Button
                  className="btn btn-danger btn-lg form-control"
                  disabled={isBusy}
                >
                  DELETE
                </Button>
              ) : (
                <></>
              )} */}
            </Card.Body>
          </Card>
        </Col>
        {/* <Col md={6} className="p-1">
          {isActive &&
          user?.id !== currentUser?.id &&
          (DownLines[user?.uplineUser?.role] ||
            features.includes(Features.ActivateAgency)) ? (
            <Card bg="dark" text="white">
              <Card.Header className="d-flex align-items-center p-2">
                <i className="text-info p-1">
                  <FontAwesomeIcon icon={faUsers} />
                </i>
                Agency
              </Card.Header>
              <Card.Body>
                <Row>
                  <Col>
                    {!isActivatingAgency && !isAgent ? (
                      <Button
                        className="btn btn-primary btn-lg form-control"
                        onClick={() => {
                          setIsActivatingAgency(true);
                          setIsAgent(true);
                          setTimeout(() => {
                            agencyForm.setFocus("commission", {
                              shouldSelect: true,
                            });
                          }, 0);
                        }}
                      >
                        ACTIVATE AGENCY
                      </Button>
                    ) : (
                      <></>
                    )}
                    {isAgent ? (
                      <Form onSubmit={agencyForm.handleSubmit(handleSubmit)}>
                        <Form.Group className="mb-2" controlId="commission">
                          <Form.Label className="text-center">Role</Form.Label>
                          <Form.Label className="form-control form-control-lg">
                            {DownLines[user?.uplineUser?.role] ??
                              DownLines[currentUser?.role]}
                          </Form.Label>
                        </Form.Group>
                        <Form.Group className="mb-2" controlId="commission">
                          <Form.Label className="text-center">
                            Commission %
                          </Form.Label>
                          <Form.Control
                            type="number"
                            step="0.01"
                            max={
                              (user?.uplineUser?.commission ??
                                appSettings?.commission) *
                                100 -
                              0.01
                            }
                            min={0.01}
                            size="lg"
                            {...agencyForm.register("commission", {
                              required: true,
                            })}
                          />
                          <div className="form-text">
                            Agent's commission must be less than your commission{" "}
                            <span className="text-info">
                              {user?.uplineUser?.commission
                                ? (user?.uplineUser?.commission * 100).toFixed(
                                    2
                                  )
                                : (appSettings?.commission * 100).toFixed(2)}
                              %
                            </span>
                          </div>
                        </Form.Group>
                        {isActivatingAgency ? (
                          <Row>
                            <Col>
                              <Button
                                variant="primary"
                                type="submit"
                                size="lg"
                                disabled={isBusyActivating}
                                className="form-control"
                              >
                                {!isBusyActivating ? (
                                  <span>ACTIVATE</span>
                                ) : (
                                  <span>
                                    <span
                                      className="spinner-grow spinner-grow-sm"
                                      role="status"
                                    ></span>
                                    <span>ACTIVATING</span>
                                  </span>
                                )}
                              </Button>
                            </Col>
                            <Col>
                              <Button
                                variant="secondary"
                                size="lg"
                                disabled={isBusyActivating}
                                className="form-control h-100"
                                onClick={() => {
                                  setIsAgent(false);
                                  setIsActivatingAgency(false);
                                }}
                              >
                                CANCEL
                              </Button>
                            </Col>
                          </Row>
                        ) : (
                          <Row>
                            <Col>
                              <Button
                                variant="primary"
                                type="submit"
                                size="lg"
                                disabled={isBusyActivating}
                                className="form-control mt-2"
                              >
                                {!isBusyActivating ? (
                                  <span>UPDATE</span>
                                ) : (
                                  <span>
                                    <span
                                      className="spinner-grow spinner-grow-sm"
                                      role="status"
                                    ></span>
                                    <span>UPDATING</span>
                                  </span>
                                )}
                              </Button>
                            </Col>
                          </Row>
                        )}
                      </Form>
                    ) : (
                      <></>
                    )}
                  </Col>
                </Row>
              </Card.Body>
            </Card>
          ) : (
            <></>
          )}
        </Col> */}
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
      <TransferCreditDialog
        show={showTransferDialog}
        isBusy={isBusy}
        handleClose={() => {
          setShowTransferDialog(false);
        }}
        handleSubmit={(data) => {
          handleTransferCredits(data);
        }}
        currency={appSettings.currency}
        locale={appSettings.locale}
      ></TransferCreditDialog>
    </>
  );
}
