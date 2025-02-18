import { useNavigate } from "react-router-dom";
import ReferralLink from "../components/user/ReferralLink.jsx";
import { AgentRoles, PlayerRoles } from "../constants/index.js";
import { useSelector } from "react-redux";

export default function ReferralLinkPage() {
  const navigate = useNavigate();
  var { user } = useSelector((state) => state.user);

  if (PlayerRoles.includes(user?.role)) {
    navigate("/");
    return;
  }

  let referralCode = user?.referralCode;

  if (!AgentRoles.includes(user?.role)) {
    referralCode = "";
  }

  return <ReferralLink referralCode={referralCode} />;
}
