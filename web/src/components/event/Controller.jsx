import { useState, useEffect, useContext } from "react";
import { Card, Button, Row, Col } from "react-bootstrap";
import { MatchStatus } from "../../constants";
import {
  updateStatus,
  declareWinner,
  cancelMatch,
  reDeclareWinner,
} from "../../services/matchesService";
import ConfirmDialog from "../_shared/ConfirmDialog";
import DeclareWinnerDialog from "./DeclareWinnerDialog";
import { MatchContext } from "../_shared/MatchContext";
import { nextMatch } from "../../services/eventsService";

export default function Controller({
  eventId,
  maxWinners,
  canReDeclare,
  allowDraw,
}) {
  const [showConfirmBetDialog, setShowConfirmBetDialog] = useState(false);
  const [confirmMessage, setConfirmMessage] = useState();
  const [handleConfirm, setHandleConfirm] = useState();
  const [isBusy, setIsBusy] = useState();
  const [showDeclareWinnerDialog, setShowDeclareWinnerDialog] = useState(false);
  const [showReDeclareWinnerDialog, setShowReDeclareWinnerDialog] =
    useState(false);
  const { match, setMatch, status, setStatus } = useContext(MatchContext);
  const [winners, setWinners] = useState([]);

  const confirm = (message, next) => {
    setConfirmMessage(message);

    setHandleConfirm(() => () => {
      next();
      setShowConfirmBetDialog(false);
    });

    setShowConfirmBetDialog(true);
  };

  const changeStatus = (status) => {
    setIsBusy(true);
    updateStatus(eventId, match?.id, status)
      .then(({ data }) => {
        if (!data.isSuccessful) {
          return;
        }

        setStatus(status);
      })
      .finally(() => {
        setIsBusy(false);
      });
  };

  const openBetting = (e) => {
    confirm(
      <span className="h5">
        Do you want to <span className="text-success">OPEN</span> the betting?
      </span>,
      () => changeStatus(MatchStatus.Open)
    );
    e.target.blur();
  };

  const closeBetting = (e) => {
    confirm(
      <span className="h5">
        Do you want to <span className="text-danger">CLOSE</span> the betting?
      </span>,
      () => changeStatus(MatchStatus.Closed)
    );
    e.target.blur();
  };

  const finalizeBetting = (e) => {
    confirm(
      <span className="h5">
        Do you want to <span className="text-info">FINALIZE</span> the betting?
      </span>,
      () => changeStatus(MatchStatus.Finalized)
    );
    e.target.blur();
  };

  const showDeclareDialog = (e) => {
    setShowDeclareWinnerDialog(true);
    e.target.blur();
  };

  const showReDeclareDialog = (e) => {
    setShowReDeclareWinnerDialog(true);
    e.target.blur();
  };

  const handleDeclareWinner = (newWinners, callback) => {
    declareWinner(eventId, match?.id, newWinners)
      .then(() => {
        setShowDeclareWinnerDialog(false);
        setWinners(newWinners);
      })
      .finally(() => {
        if (callback) {
          callback();
        }
      });
  };

  const handleReDeclareWinner = (newWinners, callback) => {
    reDeclareWinner(eventId, match?.id, newWinners)
      .then(() => {
        setShowReDeclareWinnerDialog(false);
        setWinners(newWinners);
      })
      .finally(() => {
        if (callback) {
          callback();
        }
      });
  };

  const confirmWinner = (e) => {
    confirm(<span className="h5">Do you confirm the winner?</span>, () =>
      changeStatus(MatchStatus.Completed)
    );

    e.target.blur();
  };

  const nextFight = (e) => {
    confirm(<span className="h5">Proceed to the next fight?</span>, () => {
      setIsBusy(true);
      nextMatch(eventId)
        .then(({ data }) => {
          if (!data.isSuccessful || !data.match) {
            return;
          }

          setMatch(data.match);
        })
        .finally(() => {
          setIsBusy(false);
        });
    });
    e.target.blur();
  };

  const cancelMatchHandler = (e) => {
    confirm(
      <span className="h5">
        Do you really want to <span className="text-danger">CANCEL</span> the
        fight?
      </span>,
      () => {
        setIsBusy(true);
        cancelMatch(eventId, match?.id)
          .then(({ data }) => {
            if (!data.isSuccessful) {
              return;
            }
          })
          .finally(() => {
            setIsBusy(false);
          });
      }
    );
    e.target.blur();
  };

  const refreshUsers = () => {};

  const refreshVideo = () => {};

  const renderStatusButton = () => {
    switch (status) {
      case MatchStatus.New:
        return (
          <Button
            variant="success"
            size="lg"
            className="form-control text-white"
            onClick={openBetting}
            disabled={isBusy}
          >
            OPEN
          </Button>
        );
      case MatchStatus.Open:
        return (
          <Button
            variant="danger"
            size="lg"
            className="form-control text-white"
            onClick={closeBetting}
            disabled={isBusy}
          >
            CLOSE
          </Button>
        );
      case MatchStatus.Closed:
        return (
          <Button
            variant="info"
            size="lg"
            className="form-control"
            onClick={finalizeBetting}
            disabled={isBusy}
          >
            FINALIZE BETS
          </Button>
        );
      case MatchStatus.Finalized:
        return (
          <Button
            variant="success"
            size="lg"
            className="form-control text-white"
            onClick={showDeclareDialog}
            disabled={isBusy}
          >
            DECLARE WINNER
          </Button>
        );
      case MatchStatus.Declared:
        return (
          <Row>
            {canReDeclare && (
              <Col xs={12} sm={4}>
                <Button
                  variant="danger"
                  size="lg"
                  className="form-control text-white mb-2 mb-sm-0"
                  onClick={showReDeclareDialog}
                  disabled={isBusy}
                >
                  RE-DECLARE WINNER
                </Button>
              </Col>
            )}
            <Col xs={12} sm={canReDeclare ? 8 : 12}>
              <Button
                variant="primary"
                size="lg"
                className="form-control"
                onClick={confirmWinner}
                disabled={isBusy}
              >
                CONFIRM WINNER
              </Button>
            </Col>
          </Row>
        );
      case MatchStatus.Completed:
      case MatchStatus.Cancelled:
        return (
          <Button
            variant="secondary"
            size="lg"
            className="form-control text-white"
            onClick={nextFight}
            disabled={isBusy}
          >
            NEXT FIGHT
          </Button>
        );
      default:
        <></>;
    }
  };

  useEffect(() => {
    setStatus(match?.status);
    setWinners(match?.winners);
  }, [setStatus, match]);

  return (
    <>
      <Card bg="dark">
        <Card.Body className="p-2">
          <Row className="gy-2">
            <Col>{renderStatusButton()}</Col>
          </Row>
          <Row className="mt-2 gy-2">
            <Col xs={12} sm={4}>
              <Button
                variant="outline-danger"
                size="lg"
                className="form-control h-100"
                onClick={cancelMatchHandler}
                disabled={
                  status === MatchStatus.Completed ||
                  status === MatchStatus.Cancelled ||
                  status === MatchStatus.Declared ||
                  isBusy
                }
              >
                CANCEL FIGHT
              </Button>
            </Col>
            <Col>
              <Button
                variant="outline-warning"
                size="lg"
                className="form-control h-100"
                onClick={refreshVideo}
                disabled={isBusy}
              >
                REFRESH VIDEO
              </Button>
            </Col>
            <Col>
              <Button
                variant="outline-info"
                size="lg"
                className="form-control h-100"
                onClick={refreshUsers}
                disabled={isBusy}
              >
                REFRESH USERS
              </Button>
            </Col>
          </Row>
        </Card.Body>
      </Card>
      <ConfirmDialog
        confirmButtonText="CONFIRM"
        handleConfirm={handleConfirm}
        handleClose={() => {
          setShowConfirmBetDialog(false);
        }}
        show={showConfirmBetDialog}
        isBusy={isBusy}
      >
        {confirmMessage}
      </ConfirmDialog>
      <DeclareWinnerDialog
        title="Declare Winner"
        show={showDeclareWinnerDialog}
        handleClose={() => {
          setShowDeclareWinnerDialog(false);
        }}
        teams={match?.teams}
        maxWinners={maxWinners}
        handleConfirm={handleDeclareWinner}
        allowDraw={allowDraw}
      ></DeclareWinnerDialog>
      {canReDeclare && (
        <DeclareWinnerDialog
          title="Re-Declare Winner"
          show={showReDeclareWinnerDialog}
          handleClose={() => {
            setShowReDeclareWinnerDialog(false);
          }}
          teams={match?.teams}
          maxWinners={maxWinners}
          handleConfirm={handleReDeclareWinner}
          allowDraw={allowDraw}
          exclude={winners}
        ></DeclareWinnerDialog>
      )}
    </>
  );
}
