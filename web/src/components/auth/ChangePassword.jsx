import { useState } from "react";
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import { userChangePassword } from "../../services/authService";
import { setErrorMessages } from "../../store/messagesSlice";
import { Button, Card, Form } from "react-bootstrap";

export default function ChangePassword() {
  const [isBusy, setIsBusy] = useState(false);
  const { register, handleSubmit } = useForm();
  const navigate = useNavigate();
  const dispatch = useDispatch();

  const handleChangePassword = async () => {
    await handleSubmit(changePassword)();
  };

  const changePassword = (form) => {
    setIsBusy(true);
    const { oldPassword, newPassword, confirmNewPassword, securityCode } = form;

    userChangePassword(
      oldPassword,
      newPassword,
      confirmNewPassword,
      securityCode
    )
      .then((response) => {
        const { isSuccessful, errors, validationResults } = response?.data;

        if (isSuccessful) {
          navigate("/login");
        } else {
          dispatch(setErrorMessages([...validationResults, ...errors]));
        }
      })
      .catch(({ response }) => {
        const errors = [
          ...(response?.data?.validationResults || []),
          ...(response?.data?.errors || []),
        ];
        dispatch(setErrorMessages(errors));
      })
      .finally(() => {
        setIsBusy(false);
      });
  };

  return (
    <Card bg="dark" text="white">
      <Card.Header>
        <h4>Change Password</h4>
      </Card.Header>
      <Card.Body>
        <Form className="mb-2">
          <Form.Group className="mb-2" controlId="password">
            <Form.Label>Old Password</Form.Label>
            <Form.Control
              type="password"
              placeholder="Old Password"
              size="lg"
              maxLength={100}
              {...register("oldPassword", {
                required: true,
                maxLength: 100,
              })}
            />
          </Form.Group>
          <Form.Group className="mb-2" controlId="password">
            <Form.Label>New Password</Form.Label>
            <Form.Control
              type="password"
              placeholder="New Password"
              size="lg"
              maxLength={100}
              {...register("newPassword", {
                required: true,
                minLength: 8,
                maxLength: 100,
              })}
            />
          </Form.Group>
          <Form.Group className="mb-2" controlId="confirmPassword">
            <Form.Label>Confirm New Password</Form.Label>
            <Form.Control
              type="password"
              placeholder="Confirm New Password"
              size="lg"
              maxLength={100}
              {...register("confirmNewPassword", {
                required: true,
                minLength: 8,
                maxLength: 100,
              })}
            />
          </Form.Group>
          <Form.Group className="mb-4" controlId="contactNumber">
            <Form.Label>Security Code</Form.Label>
            <Form.Control
              type="password"
              placeholder="Security COde"
              size="lg"
              maxLength={100}
              {...register("securityCode", {
                required: true,
                maxLength: 100,
              })}
            />
          </Form.Group>
          <div className="d-grid">
            <Button
              variant="primary"
              type="button"
              size="lg"
              onClick={handleChangePassword}
              disabled={isBusy}
            >
              {!isBusy ? (
                <span>Change Password</span>
              ) : (
                <span>
                  <span
                    className="spinner-grow spinner-grow-sm"
                    role="status"
                  ></span>
                  <span>Changing Password</span>
                </span>
              )}
            </Button>
          </div>
        </Form>
      </Card.Body>
    </Card>
  );
}
