import { useNavigate, NavLink } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCoins } from "@fortawesome/free-solid-svg-icons";
import { Navbar, Nav, NavDropdown } from "react-bootstrap";
import { logout as userLogout } from "../services/authService";
import { useSelector } from "react-redux";
import { pathFeatureMappings } from "../constants";
import { useEffect, useState } from "react";
import CashInDialog from "../components/credits/CashInDialog";

export default function NavBar() {
  const navigate = useNavigate();
  const { user, credits, features } = useSelector((state) => state.user);
  const { appSettings } = useSelector((state) => state.appSettings);
  const { currency, locale } = appSettings;
  const [dateTime, setDateTime] = useState(new Date().toLocaleString());
  const [showCashInDialog, setShowCashInDialog] = useState(false);

  const handleCashInDialogClose = () => {
    setShowCashInDialog(false);
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
        className="justify-content-between"
        sticky="top"
      >
        <Nav>
          <NavLink to="/" className="nav-link">
            Home
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
        </Nav>
        <div className="position-absolute start-50 translate-middle-x text-light d-none d-sm-block">
          <h5>{dateTime}</h5>
        </div>
        <Nav>
          <Nav>
            <NavDropdown title={user?.username} id="basic-nav-dropdown">
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
                <>
                  <FontAwesomeIcon icon="fa-solid fa-right-to-bracket" />
                  <span>Cash In</span>
                </>
              </NavDropdown.Item>
              <NavDropdown.Item href="#">
                <>
                  <FontAwesomeIcon icon="fa-solid fa-arrow-right-from-bracket" />
                  <span>Cash Out</span>
                </>
              </NavDropdown.Item>
              <NavDropdown.Divider />
              <NavDropdown.Item href="#" icon="fa-solid fa-clock-rotate-left">
                <>
                  <FontAwesomeIcon icon="fa-solid fa-clock-rotate-left" />
                  <span>History</span>
                </>
              </NavDropdown.Item>
            </NavDropdown>
          </Nav>
        </Nav>
      </Navbar>
      <CashInDialog
        show={showCashInDialog}
        currency={currency}
        locale={locale}
        handleClose={handleCashInDialogClose}
      />
    </>
  );
}
