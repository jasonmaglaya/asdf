import { useEffect, useState } from "react";
import { Breadcrumb, Container, Table } from "react-bootstrap";
import { useSelector } from "react-redux";
import SpinnerComponent from "../../components/_shared/SpinnerComponent";
import { getEventsReport } from "../../services/reportsService";
import { NavLink } from "react-router-dom";

export default function EventsReportPage() {
  const [isLoading, setIsLoading] = useState(true);
  const [summary, setSummary] = useState([]);
  const { appSettings } = useSelector((state) => state.appSettings);
  const { currency, locale } = appSettings;

  useEffect(() => {
    const loadSummary = async () => {
      try {
        setIsLoading(true);

        const { data } = await getEventsReport();
        setSummary(data.result);
      } catch {
      } finally {
        setIsLoading(false);
      }
    };

    loadSummary();
  }, []);

  return (
    <Container className="mt-2 text-light px-3" fluid>
      <h3>Events</h3>
      <Breadcrumb>
        <Breadcrumb.Item className="text-light" active>
          Events
        </Breadcrumb.Item>
      </Breadcrumb>
      {isLoading ? (
        <SpinnerComponent />
      ) : (
        <Table striped bordered hover responsive variant="dark">
          <thead>
            <tr>
              <th className="align-middle">Event</th>
              <th className="align-middle" style={{ width: "7%" }}>
                Date
              </th>
              <th className="align-middle" style={{ width: "3%" }}>
                Fights
              </th>
              <th className="align-middle" style={{ width: "7%" }}>
                Commission(%)
              </th>
              <th className="align-middle text-end" style={{ width: "8%" }}>
                Total Bets
              </th>
              <th style={{ width: "8%" }} className="align-middle text-end">
                Commission
              </th>
              <th style={{ width: "5%" }} className="text-center align-middle">
                Draw Multiplier
              </th>
              <th style={{ width: "8%" }} className="text-end align-middle">
                Total Draw
              </th>
              <th style={{ width: "8%" }} className="text-end align-middle">
                Draw Payouts
              </th>
              <th style={{ width: "8%" }} className="text-end align-middle">
                Draw Net
              </th>
            </tr>
          </thead>
          <tbody>
            {summary?.map((item) => {
              return (
                <tr key={`${item.fightNumber}-${item.betTimeStamp}-1`}>
                  <td className="align-middle">
                    <NavLink to={`/reports/events/${item.eventId}`}>
                      {item.title}
                    </NavLink>
                  </td>
                  <td className="align-middle">
                    {new Date(item.eventDate).toLocaleDateString(locale, {
                      year: "numeric",
                      month: "2-digit",
                      day: "2-digit",
                    })}
                  </td>
                  <td className="align-middle">{item.matches}</td>
                  <td className="align-middle">
                    {(item.commissionPercentage * 100).toFixed(2)}%
                  </td>
                  <td className="text-end align-middle">
                    {item.totalBets.toLocaleString(locale || "en-US", {
                      style: "currency",
                      currency: currency || "USD",
                    })}
                  </td>
                  <td className="text-end align-middle text-success">
                    {item.commission.toLocaleString(locale || "en-US", {
                      style: "currency",
                      currency: currency || "USD",
                    })}
                  </td>
                  <td className="align-middle text-center">
                    x{item.drawMultiplier}
                  </td>
                  <td className="text-end align-middle">
                    {item.totalDraw?.toLocaleString(locale || "en-US", {
                      style: "currency",
                      currency: currency || "USD",
                    })}
                  </td>
                  <td className="text-end align-middle">
                    {item.totalDrawPayout?.toLocaleString(locale || "en-US", {
                      style: "currency",
                      currency: currency || "USD",
                    })}
                  </td>
                  <td
                    className={
                      item.drawNet === 0
                        ? "text-end align-middle text-warning"
                        : item.drawNet > 0
                        ? "text-end align-middle text-success"
                        : "text-end align-middle text-danger"
                    }
                  >
                    {item.drawNet?.toLocaleString(locale || "en-US", {
                      style: "currency",
                      currency: currency || "USD",
                    })}
                  </td>
                </tr>
              );
            })}
          </tbody>
        </Table>
      )}
    </Container>
  );
}
