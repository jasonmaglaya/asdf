import { useEffect, useState } from "react";
import UsersList from "../components/user/UsersList.jsx";
import { getAllUsers } from "../services/usersService.js";
import { Card } from "react-bootstrap";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";

export default function UsersPage() {
  const [isBusy, setIsBusy] = useState(true);
  const [users, setUsers] = useState([]);
  const { appSettings } = useSelector((state) => state.appSettings);
  const { currency, locale } = appSettings;
  const navigate = useNavigate();

  useEffect(() => {
    getAllUsers(1, 30)
      .then(({ data }) => {
        setUsers(data?.result?.list || []);
      })
      .catch(() => {
        navigate("/", { replace: true });
      })
      .finally(() => {
        setIsBusy(false);
      });
  }, [navigate]);

  return (
    <Card bg="dark" text="white">
      <Card.Header className="p-1 pb-0">
        <h5 className="text-center">Users</h5>
      </Card.Header>
      <Card.Body style={{ padding: "0px" }}>
        <UsersList
          users={users}
          currency={currency}
          locale={locale}
          isSearching={isBusy}
        />
      </Card.Body>
    </Card>
  );
}
