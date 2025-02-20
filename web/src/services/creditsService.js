import jwtInterceptor from "./jwtInterceptor";

export const getBalance = async (partnerToken) => {
  return jwtInterceptor.get("/credits", {
    params: {
      partnerToken,
    },
  });
};

export const cashIn = async (request) => {
  return jwtInterceptor.post("/credits/cash-in", request);
};

export const cashOut = async (request) => {
  return jwtInterceptor.post("/credits/cash-out", request);
};
