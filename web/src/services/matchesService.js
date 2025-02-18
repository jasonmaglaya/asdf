import jwtInterceptor from "./jwtInterceptor";

export const addBet = async (matchId, teamCode, amount) => {
  return jwtInterceptor.post(`/matches/${matchId}/bets`, { teamCode, amount });
};

export const getBets = async (matchId) => {
  return jwtInterceptor.get(`/matches/${matchId}/bets/me`);
};

export const getTotalBets = async (matchId) => {
  return jwtInterceptor.get(`/matches/${matchId}/bets/total`);
};

export const updateStatus = async (eventId, matchId, status) => {
  return jwtInterceptor.post(`/matches/${matchId}/status`, { eventId, status });
};

export const declareWinner = async (eventId, matchId, teamCodes) => {
  return jwtInterceptor.post(`/matches/${matchId}/winner`, {
    eventId,
    teamCodes,
  });
};
export const reDeclareWinner = async (eventId, matchId, teamCodes) => {
  return jwtInterceptor.post(`/matches/${matchId}/winner/re-declare`, {
    eventId,
    teamCodes,
  });
};

export const cancelMatch = async (eventId, matchId) => {
  return jwtInterceptor.post(`/matches/${matchId}/cancel`, {
    eventId,
  });
};
