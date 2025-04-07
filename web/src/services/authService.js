import axios from "axios";
import jwtInterceptor from "./jwtInterceptor";

export const login = async (username, password) => {
  return axios.post("/auth/login", {
    username,
    password,
  });
};

export const logout = async () => {
  return jwtInterceptor.post("/auth/logout");
};

export const checkReferralCode = async (referralCode) => {
  return jwtInterceptor.get(
    `/auth/referral-code/check?referralCode=${referralCode}`
  );
};

export const signUp = async (
  username,
  password,
  confirmPassword,
  contactNumber,
  referralCode
) => {
  return axios.post("/auth/signup", {
    username,
    password,
    confirmPassword,
    contactNumber,
    referralCode,
  });
};

export const userChangePassword = async (
  oldPassword,
  newPassword,
  confirmPassword,
  securityCode
) => {
  return jwtInterceptor.put("/auth/password", {
    oldPassword,
    newPassword,
    confirmPassword,
    securityCode,
  });
};
