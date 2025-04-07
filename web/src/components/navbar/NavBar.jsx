import { useNavigate, NavLink } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCoins } from "@fortawesome/free-solid-svg-icons";
import {
  Navbar,
  Nav,
  NavDropdown,
  Offcanvas,
  Container,
} from "react-bootstrap";
import { logout as userLogout } from "../../services/authService";
import { useSelector } from "react-redux";
import { Features, pathFeatureMappings } from "../../constants";
import { useEffect, useState } from "react";
import CashInDialog from "../credits/CashInDialog";
import CashOutDialog from "../credits/CashOutDialog";
import CreditHistory from "../credits/CreditHistory";

export default function NavBar() {
  const navigate = useNavigate();
  const { user, credits, features } = useSelector((state) => state.user);
  const { appSettings } = useSelector((state) => state.appSettings);
  const { currency, locale } = appSettings;
  const [dateTime, setDateTime] = useState(new Date().toLocaleString());
  const [showCashInDialog, setShowCashInDialog] = useState(false);
  const [showCashOutDialog, setShowCashOutDialog] = useState(false);
  const [showCreditHistoryDialog, setShowCreditHistoryDialog] = useState(false);

  const handleCashInDialogClose = () => {
    setShowCashInDialog(false);
  };

  const handleCashOutDialogClose = () => {
    setShowCashOutDialog(false);
  };

  const handleCreditHistoryDialogClose = () => {
    setShowCreditHistoryDialog(false);
  };

  const logoutUser = () => {
    userLogout()
      .then(() => {})
      .catch(() => {})
      .finally(() => {
        localStorage.removeItem("user");
        navigate("/login");
      });
  };

  const changePassword = () => {
    navigate("/change-password");
  };

  useEffect(() => {
    const interval = setInterval(() => {
      setDateTime(new Date().toLocaleString());
    }, 1000);

    return () => clearInterval(interval);
  }, []);

  return (
    <>
      <Navbar
        bg="dark"
        variant="dark"
        expand="lg"
        className="justify-content-between px-2"
      >
        <Container fluid>
          <Navbar.Toggle aria-controls="basic-navbar-nav" />
          <Navbar.Offcanvas id="basic-navbar-nav">
            <Offcanvas.Header closeButton>
              <Offcanvas.Title>Menu</Offcanvas.Title>
            </Offcanvas.Header>
            <Offcanvas.Body>
              <Nav className="me-auto">
                <NavLink to="/" className="nav-link">
                  Home
                </NavLink>
                <NavLink to="/events/summary" className="nav-link">
                  Event History
                </NavLink>
                {features.includes(pathFeatureMappings["/events"]) && (
                  <NavLink to="/events" className="nav-link">
                    Events
                  </NavLink>
                )}
                {features.includes(pathFeatureMappings["/users"]) && (
                  <NavLink to="/users" className="nav-link">
                    Users
                  </NavLink>
                )}
                {features.includes(pathFeatureMappings["/reports"]) && (
                  <NavDropdown title="Reports">
                    <NavDropdown.Item to="/reports/events" as={NavLink}>
                      Events Summary
                    </NavDropdown.Item>
                  </NavDropdown>
                )}
                <NavDropdown
                  title={user?.username}
                  id="basic-nav-dropdown"
                  className="d-block d-lg-none"
                >
                  {features.includes(Features.ChangeOwnPassword) && (
                    <NavDropdown.Item
                      href="#"
                      onClick={() => {
                        changePassword();
                      }}
                    >
                      Change Password
                    </NavDropdown.Item>
                  )}
                  <NavDropdown.Item
                    href="#"
                    onClick={() => {
                      logoutUser();
                    }}
                  >
                    Logout
                  </NavDropdown.Item>
                </NavDropdown>
              </Nav>
            </Offcanvas.Body>
          </Navbar.Offcanvas>
          <div className="position-absolute start-50 translate-middle-x text-light d-none d-sm-block">
            <h5>{dateTime}</h5>
          </div>
          <Nav>
            <Nav className="d-none d-lg-block">
              <NavDropdown title={user?.username} id="basic-nav-dropdown">
                {features.includes(Features.ChangeOwnPassword) && (
                  <NavDropdown.Item
                    href="#"
                    onClick={() => {
                      changePassword();
                    }}
                  >
                    Change Password
                  </NavDropdown.Item>
                )}
                <NavDropdown.Item
                  href="#"
                  onClick={() => {
                    logoutUser();
                  }}
                >
                  Logout
                </NavDropdown.Item>
              </NavDropdown>
            </Nav>
            <Nav>
              <NavDropdown
                title={
                  <span className="fw-bolder">
                    <i style={{ color: "gold" }} className="p-2">
                      <FontAwesomeIcon icon={faCoins} />
                    </i>
                    <span className="text-success">
                      {credits?.toLocaleString(locale || "en-US", {
                        style: "currency",
                        currency: currency || "USD",
                      })}
                    </span>
                  </span>
                }
                align="end"
              >
                <NavDropdown.Item
                  href="#"
                  onClick={() => setShowCashInDialog(true)}
                >
                  <span>Cash In</span>
                </NavDropdown.Item>
                <NavDropdown.Item
                  href="#"
                  onClick={() => setShowCashOutDialog(true)}
                >
                  <span>Cash Out</span>
                </NavDropdown.Item>
                <NavDropdown.Divider />
                <NavDropdown.Item
                  href="#"
                  onClick={() => setShowCreditHistoryDialog(true)}
                >
                  <span>History</span>
                </NavDropdown.Item>
              </NavDropdown>
            </Nav>
          </Nav>
        </Container>
      </Navbar>
      <CashInDialog
        show={showCashInDialog}
        currency={currency}
        locale={locale}
        handleClose={handleCashInDialogClose}
      />
      <CashOutDialog
        show={showCashOutDialog}
        currency={currency}
        locale={locale}
        handleClose={handleCashOutDialogClose}
      />
      <CreditHistory
        show={showCreditHistoryDialog}
        currency={currency}
        locale={locale}
        handleClose={handleCreditHistoryDialogClose}
      />
    </>
  );
}
