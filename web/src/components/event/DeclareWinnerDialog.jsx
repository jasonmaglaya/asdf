import { useState } from "react";
import { Modal, Row, Col, Button } from "react-bootstrap";
import { TeamToggle } from "./TeamToggle";

export default function DeclareWinnerDialog({
  handleConfirm,
  handleClose,
  show,
  teams,
  maxWinners,
  title,
  allowDraw,
  exclude = [],
}) {
  const [winners, setWinners] = useState([]);
  const [isBusy, setIsBusy] = useState();

  const onCheckChange = (e, code) => {
    const isChecked = e.target.checked;
    if (isChecked) {
      if (winners.includes(code)) {
        return;
      }

      if (winners.length === maxWinners) {
        if (maxWinners > 1) {
          return;
        }
        setWinners([code]);
        return;
      }
      setWinners([...winners, code]);
    } else {
      const index = winners.indexOf(code);
      if (index >= 0 && maxWinners === 1) {
        return;
      }

      var temp = [...winners];
      temp.splice(index, 1);
      setWinners(temp);
    }
  };

  const confirm = () => {
    if (winners.length === 0) {
      return;
    }

    setIsBusy(true);

    handleConfirm(winners, () => {
      setWinners([]);
      setIsBusy(false);
    });
  };

  return (
    <Modal
      show={show}
      backdrop="static"
      keyboard={false}
      centered
      animation={false}
    >
      <Modal.Header>
        <Modal.Title>{title}</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Row className="gx-1 gy-1" xs={1} lg={1}>
          {teams
            ?.filter((x) => !exclude?.includes(x.code))
            .map((t) => {
              const { name, color, code } = t;

              return (
                <Col key={name} className="col-xs-auto">
                  <TeamToggle color={color}>
                    <input
                      type="checkbox"
                      className="btn-check"
                      id={name}
                      onChange={(e) => onCheckChange(e, code)}
                      checked={winners.includes(code)}
                    />
                    <label
                      className="form-control btn btn-outline-primary btn-lg text-uppercase"
                      htmlFor={name}
                    >
                      {name}
                    </label>
                  </TeamToggle>
                </Col>
              );
            })}
          {allowDraw && !exclude?.includes("D") && (
            <Col className="col-xs-auto">
              <TeamToggle color="green">
                <input
                  type="checkbox"
                  className="btn-check"
                  id="DRAW"
                  onChange={(e) => onCheckChange(e, "D")}
                  checked={winners.includes("D")}
                />
                <label
                  className="form-control btn btn-outline-primary btn-lg text-uppercase"
                  htmlFor="DRAW"
                >
                  DRAW
                </label>
              </TeamToggle>
            </Col>
          )}
        </Row>
      </Modal.Body>
      <Modal.Footer>
        <Button
          size="lg"
          variant="primary"
          onClick={confirm}
          className="text-uppercase"
          disabled={isBusy}
        >
          {!isBusy ? (
            <span>DECLARE</span>
          ) : (
            <span>
              <span
                className="spinner-grow spinner-grow-sm"
                role="status"
              ></span>
              <span>DECLARING...</span>
            </span>
          )}
        </Button>
        <Button
          size="lg"
          variant="secondary"
          onClick={handleClose}
          className="text-uppercase"
          disabled={isBusy}
        >
          CANCEL
        </Button>
      </Modal.Footer>
    </Modal>
  );
}
