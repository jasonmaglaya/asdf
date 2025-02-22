import { Container } from "react-bootstrap";
import { Outlet } from "react-router-dom";
import ErrorMessage from "../components/_shared/Messages";

export const HomeLayout = () => {
  return (
    <Container className="p-2">
      <ErrorMessage />
      <Outlet />
    </Container>
  );
};
