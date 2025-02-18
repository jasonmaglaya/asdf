import {
  Badge,
  ButtonGroup,
  Dropdown,
  DropdownButton,
  Table,
} from "react-bootstrap";
import { MatchStatus } from "../../constants";
import { useEffect, useMemo, useState } from "react";
import TeamAvatar from "../event/TeamAvatar";
import DeclareWinnerDialog from "../event/DeclareWinnerDialog";
import { reDeclareWinner } from "../../services/matchesService";
import { getMatches } from "../../services/eventsService";

export default function MatchList({ eventId, maxWinners, teams, allowDraw }) {
  const legend = useMemo(
    () => [
      { code: "M", color: "#e50914", text: "MERON", count: 0 },
      { code: "W", color: "#3d9ae8", text: "WALA", count: 0 },
      { code: "D", color: "green", text: "DRAW", count: 0 },
      { code: "C", color: "grey", text: "CANCELLED", count: 0 },
    ],
    []
  );
  const [showReDeclareWinnerDialog, setShowReDeclareWinnerDialog] =
    useState(false);
  const [matchId, setMatchId] = useState();
  const [exclude, setExclude] = useState([]);
  const [matches, setMatches] = useState([]);

  const showReDeclareDialog = (matchId, winnerCode) => {
    setMatchId(matchId);
    setExclude(winnerCode?.split(","));
    setShowReDeclareWinnerDialog(true);
  };

  const handleReDeclareWinner = (winners, callback) => {
    reDeclareWinner(eventId, matchId, winners)
      .then(async () => {
        try {
          const { data } = await getMatches(eventId);
          setMatches(data?.result?.list || []);
        } catch {}
      })
      .finally(() => {
        if (callback) {
          callback();
        }

        setShowReDeclareWinnerDialog(false);
      });
  };

  useEffect(() => {
    const loadMatches = async () => {
      try {
        const { data } = await getMatches(eventId);
        setMatches(data?.result?.list || []);
      } catch {}
    };

    loadMatches();
  }, [eventId]);

  return (
    <>
      <Table variant="dark" striped hover>
        <thead>
          <tr>
            <th>FIGHT #</th>
            <th>STATUS</th>
            <th>WINNER</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {matches.map((match) => (
            <tr key={match.id} className="align-middle">
              <td>{match.number}</td>
              <td>
                <Badge
                  className="text-uppercase"
                  bg={
                    match.status === MatchStatus.Open
                      ? "success"
                      : match.status === MatchStatus.Cancelled
                      ? "secondary"
                      : match.status === MatchStatus.New
                      ? "secondary"
                      : match.status === MatchStatus.Finalized
                      ? "warning"
                      : match.status === MatchStatus.Declared
                      ? "warning"
                      : match.status === MatchStatus.Completed
                      ? "info"
                      : "danger"
                  }
                >
                  {match.status}
                </Badge>
              </td>
              <td>
                {match.winnerCode?.split(",").map((code) => {
                  const { color, text } = legend.find((x) => x.code === code);
                  return (
                    <div key={code} className="d-flex align-items-center">
                      <TeamAvatar key={code} color={color}>
                        {code}
                      </TeamAvatar>
                      {text}
                    </div>
                  );
                })}
              </td>
              <td className="text-end">
                <DropdownButton as={ButtonGroup} variant="secondary">
                  <Dropdown.Item eventKey="view" onClick={() => {}}>
                    View
                  </Dropdown.Item>
                  {[MatchStatus.Cancelled, MatchStatus.Completed].includes(
                    match.status
                  ) && (
                    <Dropdown.Item
                      eventKey="redeclare"
                      onClick={() =>
                        showReDeclareDialog(match.id, match.winnerCode)
                      }
                    >
                      Re-Declare
                    </Dropdown.Item>
                  )}
                </DropdownButton>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
      <DeclareWinnerDialog
        title="Re-Declare Winner"
        show={showReDeclareWinnerDialog}
        handleClose={() => {
          setShowReDeclareWinnerDialog(false);
        }}
        teams={teams}
        maxWinners={maxWinners}
        handleConfirm={handleReDeclareWinner}
        allowDraw={allowDraw}
        exclude={exclude}
      ></DeclareWinnerDialog>
    </>
  );
}
