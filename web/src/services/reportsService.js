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
