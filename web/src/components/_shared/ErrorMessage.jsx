import { useEffect, useState } from "react";
import { Toast, ToastContainer } from "react-bootstrap";
import { useSelector } from "react-redux";
import { setErrorMessages } from "../../store/errorMessagesSlice";
import { useDispatch } from "react-redux";

export default function ErrorMessage() {
  const dispatch = useDispatch();
  const [showErrors, setShowErrors] = useState(false);
  const [errors, setErrors] = useState([]);
  const { messages } = useSelector((state) => state.errorMessages);

  const onClose = () => {
    setShowErrors(false);
    dispatch(setErrorMessages([]));
  };

  useEffect(() => {
    setErrors(messages);
    if (messages.length) {
      setShowErrors(true);
      setTimeout(() => {
        setShowErrors(false);
        dispatch(setErrorMessages([]));
      }, 3000);
    }
  }, [messages, dispatch]);

  return (
    <ToastContainer
      className="p-3"
      position="bottom-end"
      style={{ zIndex: 9999 }}
    >
      <Toast bg="danger" show={showErrors} onClose={() => onClose()}>
        <Toast.Header>
          <img src="holder.js/20x20?text=%20" className="rounded me-2" alt="" />
          <strong className="me-auto">Oops!</strong>
        </Toast.Header>
        <Toast.Body className="text-light">
          <ul className="m-0">
            {errors?.map((err) => (
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
