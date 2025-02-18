import { useEffect, useState } from "react";
import UsersList from "../components/user/UsersList.jsx";
import { getDownLines } from "../services/usersService.js";
import { Card } from "react-bootstrap";
import { useSelector } from "react-redux";

export default function DownLinesPage() {
  const [downLines, setDownLines] = useState([]);
  const { appSettings } = useSelector((state) => state.appSettings);
  const { currency, locale } = appSettings;

  useEffect(() => {
    getDownLines().then(({ data }) => {
      setDownLines(data.result);
    });
  }, []);

  return (
    <Card bg="dark" text="white">
      <Card.Header className="p-2 pb-0">
        <h5 className="text-center">Down Lines</h5>
      </Card.Header>
      <Card.Body style={{ padding: "0px" }}>
        <UsersList users={downLines} currency={currency} locale={locale} />
      </Card.Body>
    </Card>
  );
}
