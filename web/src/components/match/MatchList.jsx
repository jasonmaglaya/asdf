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
import { cancelMatch, reDeclareWinner } from "../../services/matchesService";
import { getMatches } from "../../services/eventsService";
import ConfirmDialog from "../_shared/ConfirmDialog";

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
  const [showCancelDialog, setShowCancelDialog] = useState(false);
  const [isBusy, setIsBusy] = useState(false);
  const [matchId, setMatchId] = useState();
  const [exclude, setExclude] = useState([]);
  const [matches, setMatches] = useState([]);

  const showReDeclareDialog = (matchId, winnerCode) => {
    setMatchId(matchId);
    setExclude(winnerCode?.split(","));
    setShowReDeclareWinnerDialog(true);
  };

  const showCancelConfirmation = (matchId) => {
    setMatchId(matchId);

    setShowCancelDialog(true);
  };

  const handleCancel = async () => {
    setIsBusy(true);

    cancelMatch(eventId, matchId)
      .then(async ({ data }) => {
        if (!data.isSuccessful) {
          return;
        }

        await loadMatches(eventId);
      })
      .catch(() => {})
      .finally(() => {
        setIsBusy(false);
        setShowCancelDialog(false);
      });
  };

  const handleReDeclareWinner = (winners, callback) => {
    reDeclareWinner(eventId, matchId, winners)
      .then(async () => {
        await loadMatches(eventId);
      })
      .finally(() => {
        if (callback) {
          callback();
        }

        setShowReDeclareWinnerDialog(false);
      });
  };

  const loadMatches = async (id) => {
    try {
      const { data } = await getMatches(id);
      setMatches(data?.result?.list || []);
    } catch {}
  };

  useEffect(() => {
    loadMatches(eventId);
  }, [eventId]);

  return (
    <>
      <Table variant="dark" striped hover style={{ marginBottom: "10rem" }}>
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
                  {[MatchStatus.Completed].includes(match.status) && (
                    <Dropdown.Item
                      eventKey="cancel"
                      onClick={() => showCancelConfirmation(match.id)}
                    >
                      Cancel
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
      <ConfirmDialog
        confirmButtonText="CONFIRM"
        handleConfirm={handleCancel}
        handleClose={() => {
          setShowCancelDialog(false);
        }}
        show={showCancelDialog}
        isBusy={isBusy}
      >
        <span className="h5">
          Do you really want to <span className="text-danger">CANCEL</span> the
          fight?
        </span>
      </ConfirmDialog>
    </>
  );
}
