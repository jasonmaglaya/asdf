import { useState, useEffect } from "react";
import { useParams, useNavigate, NavLink } from "react-router-dom";
import { useForm } from "react-hook-form";
import { Button, Form, Alert } from "react-bootstrap";
import AuthContainer from "./AuthContainer";
import { checkReferralCode, signUp } from "../../services/authService";
import { getAppSettings } from "../../services/settingsService";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faInfoCircle } from "@fortawesome/free-solid-svg-icons";

export default function Login() {
  const [isBusy, setIsBusy] = useState(false);
  const [errors, setErrors] = useState([]);
  const [showErrors, setShowErrors] = useState(false);
  const [signedUp, setSignedUp] = useState(false);

  const navigate = useNavigate();
  const { register, handleSubmit } = useForm();
  const { referralCode } = useParams();

  const handleSignUp = async () => {
    await handleSubmit(signUpUser)();
  };

  const signUpUser = async (form) => {
    setIsBusy(true);
    const { username, password, confirmPassword, contactNumber } = form;
    signUp(username, password, confirmPassword, contactNumber, referralCode)
      .then(({ data }) => {
        if (data.isSuccessful) {
          setSignedUp(true);
        } else {
          setErrors(data.errors);
          setShowErrors(data.errors?.length);
        }
      })
      .catch(({ response }) => {
        setErrors(response?.data.errors);
        setShowErrors(response?.data.errors?.length);
      })
      .finally(() => {
        setIsBusy(false);
      });
  };

  useEffect(() => {
    getAppSettings().then(({ data }) => {
      if (!data?.result?.allowOrphanSignUp && !referralCode) {
        navigate("/login", { replace: true });
      }

      if (referralCode) {
        checkReferralCode(referralCode).then(({ data }) => {
          if (!data.result) {
            navigate("/login", { replace: true });
          }
        });
      }
    });
  }, [referralCode, navigate]);

  return (
    <AuthContainer logoHeight="100">
      <div className="text-center mt-3 mb-5">
        <h4>Already have an account?</h4>
        <NavLink to="/login" className="btn btn-primary btn-lg">
          CLICK HERE TO LOGIN
        </NavLink>
      </div>
      {!signedUp ? (
        <>
          <h4>Register New Account</h4>
          <Form className="mb-2">
            <Form.Group className="mb-2" controlId="username">
              <Form.Label className="text-center">Username</Form.Label>
              <Form.Control
                placeholder="Username"
                size="lg"
                maxLength={20}
                autoCapitalize="none"
                autoComplete="off"
                {...register("username", {
                  required: true,
                  maxLength: 20,
                })}
              />
            </Form.Group>
            <Form.Group className="mb-2" controlId="password">
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
            <Form.Group className="mb-2" controlId="confirmPassword">
              <Form.Label>Confirm Password</Form.Label>
              <Form.Control
                type="password"
                placeholder="Confirm Password"
                size="lg"
                maxLength={50}
                {...register("confirmPassword", {
                  required: true,
                  minLength: 4,
                  maxLength: 50,
                })}
              />
            </Form.Group>
            <Form.Group className="mb-4" controlId="contactNumber">
              <Form.Label>Contact Number</Form.Label>
              <Form.Control
                placeholder="Contact Number"
                size="lg"
                maxLength={15}
                {...register("contactNumber", {
                  required: true,
                  minLength: 8,
                  maxLength: 15,
                })}
              />
            </Form.Group>
            <div className="d-grid">
              <Button
                variant="primary"
                type="button"
                size="lg"
                onClick={handleSignUp}
                disabled={isBusy}
              >
                {!isBusy ? (
                  <span>Submit</span>
                ) : (
                  <span>
                    <span
                      className="spinner-grow spinner-grow-sm"
                      role="status"
                    ></span>
                    <span>Submitting</span>
                  </span>
                )}
              </Button>
            </div>
          </Form>
          <Alert
            variant="danger"
            show={showErrors}
            onClose={() => setShowErrors(false)}
            dismissible
          >
            <ul className="m-0">
              {errors?.map((err) => (
                <li key={err} style={{ listStyleType: "none" }}>
                  {err}
                </li>
              ))}
            </ul>
          </Alert>
        </>
      ) : (
        <Alert variant="success">
          <FontAwesomeIcon icon={faInfoCircle} />
          <p className="h6">
            You have successfully signed up. However, your account is still
            inactive. Please contact your agent to activate your account.
          </p>
        </Alert>
      )}
    </AuthContainer>
  );
}
