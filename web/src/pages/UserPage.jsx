import { useEffect, useState } from "react";
import User from "../components/user/User";
import { getUser } from "../services/usersService";
import { useParams, useNavigate } from "react-router-dom";

export default function UserPage() {
  const { userId } = useParams();
  const [user, setUser] = useState();
  const navigate = useNavigate();

  useEffect(() => {
    getUser(userId)
      .then(({ data }) => {
        setUser(data.result);
      })
      .catch(() => {
        navigate("/", { replace: true });
      });
  }, [userId, navigate]);
  return <User user={user} setUser={setUser} />;
}
