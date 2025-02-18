import axios from "axios";
import jwtInterceptor from "./jwtInterceptor";

export const getUserFeatures = async () => {
  return jwtInterceptor.get(`/settings/features/me`);
};

export const getAppSettings = async () => {
  return axios.get(`/settings/app-settings`);
};
