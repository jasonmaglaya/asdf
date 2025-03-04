import jwtInterceptor from "./jwtInterceptor";

export const getEventsReport = async () => {
  return jwtInterceptor.get(`/reports/events`);
};

export const getEventSummary = async (eventId) => {
  return jwtInterceptor.get(`/reports/events/${eventId}`);
};

export const getPlayerEventSummary = async (eventId) => {
  return jwtInterceptor.get(`/reports/players/events/${eventId}`);
};

export const getMatchSummary = async (matchId) => {
  return jwtInterceptor.get(`/reports/matches/${matchId}`);
};

export const getPlayerBetSummary = async (matchId, userId) => {
  return jwtInterceptor.get(`/reports/matches/${matchId}/players/${userId}`);
};
