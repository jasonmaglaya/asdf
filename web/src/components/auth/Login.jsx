import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { Button, Form } from "react-bootstrap";
import AuthContainer from "./AuthContainer";
import { login as userLogin } from "../../services/authService";
import { useNavigate, useSearchParams } from "react-router-dom";
import LoadingScreen from "../_shared/LoadingScreen";
import { setErrorMessages } from "../../store/messagesSlice";
import { useDispatch } from "react-redux";

export default function Login() {
  const [showLoading, setShowLoading] = useState(true);
  const [isBusy, setIsBusy] = useState(false);
  const { register, handleSubmit } = useForm();
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const [searchParams] = useSearchParams();

  const loginUser = (form) => {
    setIsBusy(true);
    const { username, password } = form;

    userLogin(username, password)
      .then((response) => {
        const {
          isSuccessful,
          accessToken,
          refreshToken,
          errors,
          validationResults,
        } = response?.data;

        if (isSuccessful) {
          window.localStorage.setItem(
            "user",
            JSON.stringify({ accessToken, refreshToken })
          );

          const returnUrl = searchParams.get("returnUrl");
          if (returnUrl) {
            navigate(returnUrl);
          } else {
            navigate("/");
          }
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

  useEffect(() => {
    const accessToken = searchParams.get("accessToken");
    const refreshToken = searchParams.get("refreshToken");
    const operatorToken = searchParams.get("operatorToken");

    if (accessToken || refreshToken || operatorToken) {
      const user = { accessToken, refreshToken, operatorToken };

      window.localStorage.setItem("user", JSON.stringify(user));

      navigate("/");
    } else {
      setShowLoading(false);
    }
  }, [searchParams, navigate]);

  return showLoading ? (
    <LoadingScreen />
  ) : (
    <AuthContainer>
      <Form onSubmit={handleSubmit(loginUser)}>
        <Form.Group className="mb-2" controlId="username">
          <Form.Label className="text-center">Username</Form.Label>
          <Form.Control
            placeholder="Username"
            size="lg"
            maxLength={20}
            autoComplete="off"
            autoCapitalize="none"
            {...register("username", {
              required: true,
              maxLength: 20,
            })}
          />
        </Form.Group>
        <Form.Group className="mb-4" controlId="password">
          <Form.Label>Password</Form.Label>
          <Form.Control
            type="password"
            placeholder="Password"
            size="lg"
            maxLength={50}
            {...register("password", {
              required: true,
              minLength: 4,
              maxLength: 50,
            })}
          />
        </Form.Group>
        <div className="d-grid">
          <Button variant="primary" type="submit" size="lg" disabled={isBusy}>
            {!isBusy ? (
              <span>Login</span>
            ) : (
              <span>
                <span
                  className="spinner-grow spinner-grow-sm"
                  role="status"
                ></span>
                <span>Logging in</span>
              </span>
            )}
          </Button>
        </div>
      </Form>
    </AuthContainer>
  );
}
