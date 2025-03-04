import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Masonry from "react-masonry-css";
import { useMediaQuery } from "react-responsive";
import Video from "../components/event/Video";
import Betting from "../components/event/Betting";
import Trend from "../components/event/Trend";
import Controller from "../components/event/Controller";
import { getEvent, getWinners } from "../services/eventsService";
import { getCurrentMatch } from "../services/eventsService";
import { MatchContext } from "../components/_shared/MatchContext";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { getTotalBets } from "../services/matchesService";
import {
  PlayerRoles,
  EventHubEvents,
  Features,
  EventStatus,
  AdminRoles,
  AgentRoles,
} from "../constants";
import { getUser } from "../services/usersService";
import { useDispatch, useSelector } from "react-redux";
import { setCredits } from "../store/userSlice";

export default function EventPage() {
  const breakpointColumnsObj = {
    default: 2,
    1100: 1,
    700: 1,
    500: 1,
  };

  const isDesktopOrLaptop = useMediaQuery({ minWidth: 1225 });
  const isMediumDesktopOrLaptop = useMediaQuery({ minWidth: 1718 });
  const isLargeDesktopOrLaptop = useMediaQuery({ minWidth: 2200 });
  const isTabletOrMobile = useMediaQuery({ minWidth: 1100, maxWidth: 1224 });

  const [connection, setConnection] = useState(null);
  const [event, setEvent] = useState({});
  const [match, setMatch] = useState();
  const [status, setStatus] = useState();
  const [totalBets, setTotalBets] = useState([]);
  const [commission, setCommission] = useState(0);
  const { eventId } = useParams();
  const { features, role, user } = useSelector((state) => state.user);
  const [winners, setWinners] = useState([]);
  const [winnersList, setWinnersList] = useState([]);
  const [canControlMatch, setCanControlMatch] = useState([]);
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { appSettings } = useSelector((state) => state.appSettings);
  const { currency, locale } = appSettings;

  const getAllBets = (matchId) => {
    getTotalBets(matchId).then(({ data }) => {
      setTotalBets(data.result.totalBets);
      setCommission(data.result.commission);
    });
  };

  const getWinnersList = async (id) => {
    try {
      const { data } = await getWinners(id);
      setWinnersList(data?.result);
    } catch {}
  };

  useEffect(() => {
    if (match?.id) {
      getAllBets(match.id);
    }
  }, [match]);

  useEffect(() => {
    getWinnersList(eventId);
  }, [eventId]);

  useEffect(() => {
    setCanControlMatch(features.includes(Features.ControlMatch));
  }, [features]);

  const matchContext = {
    match,
    setMatch,
    status,
    setStatus,
    totalBets,
    setTotalBets,
    commission,
    setCommission,
    winners,
    setWinners,
  };

  useEffect(() => {
    const loadEvent = async () => {
      try {
        const getEventResult = await getEvent(eventId);
        if (!getEventResult.data.isSuccessful) {
          return;
        }

        const event = getEventResult.data.result;

        if (!event || event.status !== EventStatus.Active) {
          navigate("/", { replace: true });
          return;
        }

        const currentMatchResult = await getCurrentMatch(eventId);

        if (!currentMatchResult.data.isSuccessful) {
          return;
        }

        setEvent(event);

        const currentMatch = currentMatchResult.data.result;

        setMatch(currentMatch);
        setStatus(currentMatch?.status);
        setWinners(currentMatch?.winners);
      } catch (error) {
        navigate("/", { replace: true });
      }
    };

    const user = JSON.parse(localStorage.getItem("user")?.toString());
    if (!user) {
      navigate("/login", { replace: true });
      return;
    }

    const { accessToken } = user;

    const newConnection = new HubConnectionBuilder()
      .withUrl(`${process.env.REACT_APP_API_BASEURL}/hubs/events`, {
        accessTokenFactory: () => accessToken,
      })
      .withAutomaticReconnect([0, 3000])
      .build();

    const joinRoom = async () => {
      if (newConnection.state !== "Connected") {
        return null;
      }

      await newConnection.invoke(EventHubEvents.JoinEvent, eventId);

      setConnection(newConnection);
      loadEvent();
    };

    const startConnection = async () => {
      try {
        await newConnection.start();
        await joinRoom();
      } catch {
        setConnection(null);
        setTimeout(startConnection, 1000);
      }
    };

    newConnection.onreconnecting(() => {
      setConnection(null);
    });

    newConnection.onreconnected(async () => {
      await joinRoom();
    });

    newConnection.onclose(async () => {
      setConnection(newConnection);
      await startConnection();
    });

    newConnection.on(EventHubEvents.TotalBetsReceived, (currentTotalBets) => {
      setTotalBets(currentTotalBets);
    });

    newConnection.on(EventHubEvents.MatchStatusReceived, (stat) => {
      getUser().then(({ data }) => {
        dispatch(setCredits(data.result.credits));
      });

      setStatus(stat);
      getWinnersList(eventId);
    });

    newConnection.on(EventHubEvents.WinnerDeclared, (teamCodes) => {
      getUser().then(({ data }) => {
        dispatch(setCredits(data.result.credits));
      });

      setWinners(teamCodes);
      getWinnersList(eventId);
    });

    newConnection.on(EventHubEvents.WinnerReDeclared, () => {
      getWinnersList(eventId);
    });

    newConnection.on(EventHubEvents.NextMatchReceived, (match) => {
      getUser().then(({ data }) => {
        dispatch(setCredits(data.result.credits));
      });

      setMatch(match);
      setStatus(match?.status);
    });

    startConnection();
  }, [eventId, dispatch, navigate]);

  return (
    <MatchContext.Provider value={matchContext}>
      <Masonry
        breakpointCols={breakpointColumnsObj}
        className="my-masonry-grid"
        columnClassName="my-masonry-grid_column"
      >
        <Video embedTag={event?.video}></Video>
        {canControlMatch && isDesktopOrLaptop && <></>}
        {canControlMatch ? (
          <Controller
            eventId={eventId}
            maxWinners={event.maxWinners}
            canReDeclare={features?.includes(Features.ReDeclare)}
            allowDraw={event.allowDraw}
          />
        ) : null}
        {canControlMatch && isDesktopOrLaptop && <></>}
        {canControlMatch && isDesktopOrLaptop && <></>}
        {canControlMatch && isTabletOrMobile && <></>}
        <Betting
          eventId={eventId}
          title={event.title}
          minimumBet={event.minimumBet}
          maximumBet={event.maximumBet}
          allowDraw={event.allowDraw}
          maxDrawBet={event.maxDrawBet}
          drawMultiplier={event.drawMultiplier}
          allowAdminBetting={event.allowAdminBetting}
          allowAgentBetting={event.allowAgentBetting}
          currency={currency}
          locale={locale}
          canControlMatch={canControlMatch}
          allowBetting={
            !user?.isBettingLocked &&
            (PlayerRoles.includes(role) ||
              (AdminRoles.includes(role) && event.allowAdminBetting) ||
              (AgentRoles.includes(role) && event.allowAgentBetting))
          }
          match={match}
          totalBets={totalBets}
          commission={commission}
          status={status}
          winners={winners}
        ></Betting>
        {((canControlMatch && event.allowAdminBetting) ||
          (!event.allowAgentBetting && !PlayerRoles.includes(role))) &&
          isMediumDesktopOrLaptop && <></>}
        {isLargeDesktopOrLaptop && !canControlMatch && <></>}
        {canControlMatch &&
          !event.allowAdminBetting &&
          !isMediumDesktopOrLaptop && <></>}
        {user?.isBettingLocked && !canControlMatch && isDesktopOrLaptop && (
          <></>
        )}
        {user?.isBettingLocked &&
          !canControlMatch &&
          isLargeDesktopOrLaptop && <></>}
        <Trend winners={winnersList} />
        {connection?.state !== "Connected" && (
          <span className="bg-primary text-dark connection-status">
            Connecting...
          </span>
        )}
      </Masonry>
    </MatchContext.Provider>
  );
}
