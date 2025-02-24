import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useState } from "react";
import { Button, Container, Modal, Spinner } from "react-bootstrap";
import { faCoins } from "@fortawesome/free-solid-svg-icons";
import SpinnerComponent from "../_shared/SpinnerComponent";
import { getBalance, getCreditHistory } from "../../services/creditsService";
import { useDispatch } from "react-redux";
import { setErrorMessages } from "../../store/messagesSlice";
import {
  formatUTCToLocalDate,
  formatUTCToLocalTime,
} from "../../utils/dateFormatter";

export default function CreditHistory({ show, handleClose, currency, locale }) {
  const dispatch = useDispatch();
  const [isLoadingBalance, setIsLoadingBalance] = useState(true);
  const [isLoading, setIsLoading] = useState(true);
  const [balance, setBalance] = useState(0);
  const [history, setHistory] = useState([]);

  const handleOnShow = async () => {
    setIsLoading(true);
    setIsLoadingBalance(true);

    const { operatorToken } = JSON.parse(
      localStorage.getItem("user")?.toString()
    );

    try {
      const { data } = await getBalance(operatorToken);
      setBalance(data.result.amount);
      setIsLoadingBalance(false);
    } catch (error) {
      dispatch(setErrorMessages(["Unable to get the balance."]));
    }

    try {
      const { data } = await getCreditHistory(1, 10);
      const sortedList = data.result.list.reduce((acc, transaction) => {
        const { amount, transactionDate } = transaction;
        const formattedDate = formatUTCToLocalDate(transactionDate);
        const time = formatUTCToLocalTime(transactionDate);

        let existingGroup = acc.find((group) => group.date === formattedDate);

        if (existingGroup) {
          existingGroup.transactions.push({ amount, time });
        } else {
          acc.push({ date: formattedDate, transactions: [{ amount, time }] });
        }

        return acc;
      }, []);

      setHistory(sortedList);
    } catch (error) {
      dispatch(setErrorMessages(["Unable to load credit history."]));
    }

    setIsLoading(false);
  };

  return (
    <Modal
      show={show}
      onHide={handleClose}
      backdrop="static"
      keyboard={false}
      centered
      animation={false}
      onShow={handleOnShow}
    >
      <Modal.Header closeButton>
        <Modal.Title>HISTORY</Modal.Title>
      </Modal.Header>
      <Modal.Body className="p-0 pt-3">
        <>
          <Container className="mb-1 d-flex justify-content-between">
            <h5>
              <i style={{ color: "gold" }}>
                <FontAwesomeIcon icon={faCoins} />
              </i>{" "}
              BALANCE
            </h5>
            {isLoadingBalance ? (
              <Spinner animation="border" variant="success" size="sm" />
            ) : (
              <h5>
                <span className="text-success">
                  {balance?.toLocaleString(locale || "en-US", {
                    style: "currency",
                    currency: currency || "USD",
                  })}
                </span>
              </h5>
            )}
          </Container>
          {isLoading ? (
            <SpinnerComponent />
          ) : (
            <Container style={{ overflowY: "auto", maxHeight: "400px" }}>
              {history.map((item) => (
                <div key={item.date} className="p-0 mb-0">
                  <div className="bg-light p-1 fw-bold">
                    <small>{item.date}</small>
                  </div>
                  <div>
                    <table className="table table-striped table-borderless">
                      <tbody>
                        {item.transactions.map((tran) => (
                          <tr key={tran.time}>
                            <td className="d-flex flex-column">
                              <span>
                                {item.amount < 0 ? "CASH OUT" : "CASH IN"}
                              </span>
                              <small className="text-muted">{tran.time}</small>
                            </td>
                            <td className="text-end align-middle">
                              {tran.amount.toLocaleString(locale || "en-US", {
                                style: "currency",
                                currency: currency || "USD",
                              })}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </div>
              ))}
            </Container>
          )}
        </>
      </Modal.Body>
      <Modal.Footer>
        <Button size="lg" variant="secondary" onClick={handleClose}>
          CLOSE
        </Button>
      </Modal.Footer>
    </Modal>
  );
}
