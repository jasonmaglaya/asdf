import jwtInterceptor from "./jwtInterceptor";

export const getBalance = async (partnerToken) => {
  return jwtInterceptor.get("/credits", {
    params: {
      partnerToken,
    },
  });
};

export const cashIn = async (partnerToken, amount, currency) => {
  return jwtInterceptor.post("/credits/cash-in", {
    partnerToken,
    amount,
    currency,
  });
};

export const cashOut = async (partnerToken, amount, currency) => {
  return jwtInterceptor.post("/credits/cash-out", {
    partnerToken,
    amount,
    currency,
  });
};

export const getCreditHistory = async (pageNumber, pageSize) => {
  return jwtInterceptor.get("/credits/history", {
    params: {
      pageNumber,
      pageSize,
    },
  });
};
