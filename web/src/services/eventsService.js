import jwtInterceptor from "./jwtInterceptor";

export const addEvent = async (request) => {
  return jwtInterceptor.post("/events", request);
};

export const updateEventStatus = async (eventId, status) => {
  return jwtInterceptor.put(`/events/${eventId}/status`, { status });
};

export const updateEvent = async (request) => {
  return jwtInterceptor.put("events", request);
};

export const getEvents = async (pageNumber, pageSize) => {
  return jwtInterceptor.get("/events", {
    params: {
      pageNumber,
      pageSize,
    },
  });
};

export const getAllEvents = async (status, pageNumber, pageSize) => {
  return jwtInterceptor.get("/events/all", {
    params: {
      status,
      pageNumber,
      pageSize,
    },
  });
};

export const getEvent = async (id) => {
  return jwtInterceptor.get(`/events/${id}`);
};

export const getCurrentMatch = async (id) => {
  return jwtInterceptor.get(`/events/${id}/matches/current`);
};

export const nextMatch = async (id) => {
  return jwtInterceptor.post(`/events/${id}/matches/next`);
};

export const getWinners = async (id) => {
  return jwtInterceptor.get(`/events/${id}/winners`);
};

export const getMatches = async (eventId, pageNumber, pageSize) => {
  return jwtInterceptor.get(`events/${eventId}/matches`, {
    params: {
      pageNumber,
      pageSize,
    },
  });
};

export const getLastEvents = async () => {
  return jwtInterceptor.get("/events/last-n");
};
