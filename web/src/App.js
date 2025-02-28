import { createBrowserRouter, Navigate } from "react-router-dom";
import { HomeLayout } from "./layouts/HomeLayout";
import ProtectedLayout from "./layouts/ProtectedLayout";
import { AuthLayout } from "./layouts/AuthLayout";
import LoginPage from "./pages/LoginPage";
import SignUpPage from "./pages/SignUpPage";
import HomePage from "./pages/HomePage";
import EventPage from "./pages/EventPage";
import NewEventPage from "./pages/NewEventPage";
import ReferralLinkPage from "./pages/ReferralLinkPage";
import DownLinesPage from "./pages/DownLinesPage";
import UserPage from "./pages/UserPage";
import UsersPage from "./pages/UsersPage";
import TransferCreditPage from "./pages/TransferCreditPage";
import EventsMaintenancePage from "./pages/EventsMaintenancePage";
import MatchesPage from "./pages/MatchesPage";
import PlayerEventSummaryPage from "./pages/PlayerEventSummaryPage";
import EventsReportPage from "./pages/EventsReportPage";

const getUserData = () => {
  return new Promise((resolve) =>
    setTimeout(() => {
      const user = localStorage.getItem("user");
      resolve(user);
    }, 0)
  );
};

export const router = createBrowserRouter([
  {
    path: "/",
    element: <AuthLayout />,
    loader: async () => {
      const user = await getUserData();
      return { user };
    },
    children: [
      {
        element: <HomeLayout />,
        children: [
          {
            path: "/login",
            element: <LoginPage />,
          },
          {
            path: "/signup/:referralCode",
            element: <SignUpPage />,
          },
          {
            path: "/signup",
            element: <SignUpPage />,
          },
        ],
      },
      {
        element: <ProtectedLayout />,
        children: [
          {
            path: "/",
            element: <HomePage />,
          },
          {
            path: "/events",
            element: <EventsMaintenancePage />,
          },
          {
            path: "/events/new",
            element: <NewEventPage />,
          },
          {
            path: "/events/summary",
            element: <PlayerEventSummaryPage />,
          },
          {
            path: "/events/:eventId",
            element: <EventPage />,
          },
          {
            path: "/events/:eventId/edit",
            element: <NewEventPage />,
          },
          {
            path: "/events/:eventId/matches",
            element: <MatchesPage />,
          },
          {
            path: "/referral-link",
            element: <ReferralLinkPage />,
          },
          {
            path: "/down-lines",
            element: <DownLinesPage />,
          },
          {
            path: "/users",
            element: <UsersPage />,
          },
          {
            path: "/users/:userId",
            element: <UserPage />,
          },
          {
            path: "/users/:userId/transfer-credits",
            element: <TransferCreditPage />,
          },
          {
            path: "/reports/events",
            element: <EventsReportPage />,
          },
        ],
      },
      {
        path: "*",
        element: <Navigate to="/login" />,
      },
    ],
  },
]);
