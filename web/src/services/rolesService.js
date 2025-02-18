import jwtInterceptor from "./jwtInterceptor";

export const getRoles = async () => {
  return jwtInterceptor.get(`/values/roles`);
};
