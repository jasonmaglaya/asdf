import jwtInterceptor from "./jwtInterceptor";

export const getAllUsers = async (pageNumber, pageSize, isAgent) => {
  return jwtInterceptor.get(`/users`, {
    params: { pageNumber, pageSize, isAgent },
  });
};

export const getUser = async (userId) => {
  if (!userId) {
    userId = "me";
  }

  return jwtInterceptor.get(`/users/${userId}`);
};

export const getDownLines = async () => {
  return jwtInterceptor.get(`/users/me/down-lines`);
};

export const updateStatus = async (userId, isActive) => {
  return jwtInterceptor.patch(`/users/${userId}/status`, { isActive });
};

export const updateBettingStatus = async (userId, isBettingLocked) => {
  return jwtInterceptor.patch(`/users/${userId}/betting/status`, {
    isBettingLocked,
  });
};

export const updateRole = async (userId, roleCode) => {
  return jwtInterceptor.patch(`/users/${userId}/role`, { roleCode });
};

export const updateAgency = async (userId, commission) => {
  return jwtInterceptor.patch(`/users/${userId}/agency`, { commission });
};

export const transferCredits = async (userId, amount, from, notes) => {
  return jwtInterceptor.post(`/users/${userId}/credits`, {
    amount,
    from,
    notes,
  });
};

export const searchUser = async (keyword, pageNumber, pageSize, isAgent) => {
  return jwtInterceptor.get(`/users/search`, {
    params: {
      keyword,
      pageNumber,
      pageSize,
      isAgent,
    },
  });
};
