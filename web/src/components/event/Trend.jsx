import { Card } from "react-bootstrap";
import { useEffect, useMemo, useState } from "react";
import TeamAvatar from "./TeamAvatar";

export default function Trend({ winners }) {
  const [trend, setTrend] = useState({});
  const [history, setHistory] = useState({});
  const [maxColumns, setMaxColumns] = useState(9);

  const initialLegend = useMemo(
    () => [
      { code: "M", color: "#e50914", text: "MERON", count: 0 },
      { code: "W", color: "#3d9ae8", text: "WALA", count: 0 },
      { code: "D", color: "green", text: "DRAW", count: 0 },
      { code: "C", color: "grey", text: "CANCELLED", count: 0 },
    ],
    []
  );

  const [legend, setLegend] = useState(initialLegend);

  const maxRows = 6;

  useEffect(() => {
    const board = {};
    Array.from({ length: maxRows }).forEach((_, row) => {
      board[row + 1] = {};
    });

    const hist = {};
    Array.from({ length: maxRows }).forEach((_, row) => {
      hist[row + 1] = {};
    });

    let row = 0;
    let col = 0;
    let lastCol = 0;
    let prevWinner;
    let streak = 0;
    let x = 1;
    let y = 1;
    let maxCol = 0;

    (winners || []).forEach((winner) => {
      const { number, teamCode: code } = winner;

      // History
      if (x > maxRows) {
        x = 1;
        y++;
      }

      hist[x.toString()][y.toString()] = { number, code };
      x++;

      // Trend
      if (code === "C") {
        return;
      }

      if (prevWinner === code) {
        row++;
        if (row > maxRows) {
          row--;
          col++;
        } else {
          let cell = board[row.toString()][col.toString()];
          if (cell) {
            row--;
            col++;
          } else {
            const adjacentCell = board[row.toString()][(col - 1).toString()];

            if (
              code === adjacentCell?.code &&
              adjacentCell?.streak !== streak
            ) {
              row--;
              col++;
            }
          }
        }
      } else {
        row = 1;
        col = lastCol + 1;
        lastCol++;
        streak++;
      }

      prevWinner = code;

      board[row.toString()][col.toString()] = { number, code, streak };

      if (col > maxCol) {
        maxCol = col;
      }
    });

    maxCol = maxCol < 9 ? 9 : maxCol;

    setMaxColumns(maxCol + 1);

    const newLegend = initialLegend.map((x) => {
      x.count = (winners || []).filter((y) => y.teamCode === x.code).length;
      return x;
    });

    setLegend(newLegend);
    setHistory(hist);
    setTrend(board);
  }, [winners, initialLegend]);

  return (
    <Card bg="dark" text="white">
      <Card.Header>
        <div className="d-flex align-items-center justify-content-md-center overflow-auto">
          {legend.map(({ code, text, count }) => {
            const color = legend.find((x) => x.code === code)?.color;
            return (
              <div
                key={text}
                className="d-flex flex-row align-items-center"
                style={{ marginRight: "1em" }}
              >
                <TeamAvatar color={color}>{count}</TeamAvatar>
                <span>{text}</span>
              </div>
            );
          })}
        </div>
      </Card.Header>
      <Card.Body>
        {
          <div className="overflow-auto">
            {Array.from({ length: maxRows }).map((_, row) => {
              return (
                <div className="d-flex flex-row" key={row}>
                  {Array.from({ length: maxColumns }).map((_, col) => {
                    const winner = history[row + 1]?.[col + 1];
                    const color = legend.find(
                      (x) => x.code === winner?.code
                    )?.color;
                    return (
                      <TeamAvatar key={`${row}-${col}`} color={color}>
                        {winner?.number}
                      </TeamAvatar>
                    );
                  })}
                </div>
              );
            })}
            <hr />
            {Array.from({ length: maxRows }).map((_, row) => {
              return (
                <div className="d-flex flex-row" key={row}>
                  {Array.from({ length: maxColumns }).map((_, col) => {
                    const winner = trend[row + 1]?.[col + 1];
                    const color = legend.find(
                      (x) => x.code === winner?.code
                    )?.color;
                    return (
                      <TeamAvatar key={`${row}-${col}`} color={color}>
                        {winner?.code ? (
                          <div
                            className="rounded-circle d-flex align-items-center justify-content-center text-light m-1"
                            style={{
                              width: "10px",
                              height: "10px",
                              backgroundColor: "white",
                              flexShrink: 0,
                            }}
                          ></div>
                        ) : (
                          <></>
                        )}
                      </TeamAvatar>
                    );
                  })}
                </div>
              );
            })}
          </div>
        }
      </Card.Body>
    </Card>
  );
}
