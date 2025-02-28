import jwtInterceptor from "./jwtInterceptor";

export const getEventsReport = async () => {
  return jwtInterceptor.get(`/reports/events`);
};
