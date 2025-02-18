import { Container } from "react-bootstrap";
import { useNavigate, useLocation, Outlet } from "react-router-dom";
import NavBar from "./NavBar";
import { useEffect } from "react";
import { pathFeatureMappings } from "../constants";
import { useDispatch } from "react-redux";
import { fetchFeatures, fetchUser } from "../store/userSlice";
import { useSelector } from "react-redux";
import ErrorMessage from "../components/_shared/ErrorMessage";

const ProtectedLayout = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { features, loadingFeatures, user } = useSelector(
    (state) => state.user
  );
  const { pathname } = useLocation();

  const redirectUrl = `/login?returnUrl=${encodeURI(pathname)}`;
  if (!localStorage.getItem("user")) {
    window.location.replace(redirectUrl);
  }

  if (
    pathFeatureMappings[pathname] &&
    !features.includes(pathFeatureMappings[pathname]) &&
    !loadingFeatures
  ) {
    navigate("/", { replace: true });
  }

  useEffect(() => {
    if (user && !user.isActive) {
      navigate("/login", { replace: true });
    }
  }, [user, navigate]);

  useEffect(() => {
    dispatch(fetchUser());
    dispatch(fetchFeatures());
  }, [dispatch]);
  return (
    <>
      <NavBar />
      <Container className="p-0" fluid style={{ minHeight: "800px" }}>
        <ErrorMessage />
        <Outlet />
      </Container>
    </>
  );
};

export default ProtectedLayout;
