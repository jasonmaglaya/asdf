import { useEffect, useState } from "react";
import { Toast, ToastContainer } from "react-bootstrap";
import { useSelector } from "react-redux";
import {
  setErrorMessages,
  setSuccessMessages,
} from "../../store/messagesSlice";
import { useDispatch } from "react-redux";

export default function Messages() {
  const dispatch = useDispatch();
  const [showErrors, setShowErrors] = useState(false);
  const [showSuccess, setShowSuccess] = useState(false);
  const [errors, setErrors] = useState([]);
  const [success, setSuccess] = useState([]);
  const { errorMessages, successMessages } = useSelector(
    (state) => state.messages
  );

  const onClose = () => {
    setShowErrors(false);
    dispatch(setErrorMessages([]));
  };

  useEffect(() => {
    setErrors(errorMessages);
    if (errorMessages.length) {
      setShowErrors(true);
      setTimeout(() => {
        setShowErrors(false);
        dispatch(setErrorMessages([]));
      }, 5000);
    }

    setSuccess(successMessages);
    if (successMessages.length) {
      setShowSuccess(true);
      setTimeout(() => {
        setShowSuccess(false);
        dispatch(setSuccessMessages([]));
      }, 5000);
    }
  }, [errorMessages, successMessages, dispatch]);

  return (
    <ToastContainer
      className="p-3"
      position="bottom-end"
      style={{ zIndex: 9999 }}
    >
      <Toast bg="danger" show={showErrors} onClose={() => onClose()}>
        <Toast.Header>
          <strong className="me-auto">Oops!</strong>
        </Toast.Header>
        <Toast.Body className="text-light h6">
          <ul className="m-0">
            {errors?.map((err) => (
              <li key={err} style={{ listStyleType: "none" }}>
                {err}
              </li>
            ))}
          </ul>
        </Toast.Body>
      </Toast>
      <Toast bg="success" show={showSuccess} onClose={() => onClose()}>
        <Toast.Header>
          <strong className="me-auto">Hurray!</strong>
        </Toast.Header>
        <Toast.Body className="text-light h6">
          <ul className="m-0">
            {success?.map((err) => (
              <li key={err} style={{ listStyleType: "none" }}>
                {err}
              </li>
            ))}
          </ul>
        </Toast.Body>
      </Toast>
    </ToastContainer>
  );
}
