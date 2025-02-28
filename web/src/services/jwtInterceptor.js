import axios from "axios";

const jwtInterceptor = axios.create({});

jwtInterceptor.interceptors.request.use((config) => {
  config.baseURL = process.env.REACT_APP_API_BASEURL;

  const userString = localStorage.getItem("user")?.toString();
  if (!userString) {
    return config;
  }

  const user = JSON.parse(userString);
  if (!user) {
    return config;
  }

  config.headers["Authorization"] = `Bearer ${user.accessToken}`;

  return config;
});

jwtInterceptor.interceptors.response.use(
  (response) => {
    return response;
  },

  async (error) => {
    if (error.response?.status !== 401) {
      return Promise.reject(error);
    }

    const userString = localStorage.getItem("user")?.toString();
    if (!userString) {
      window.location.href = "/login";
      return;
    }

    const user = JSON.parse(userString);

    const { refreshToken } = user;
    const payload = { refreshToken };

    try {
      let response = await jwtInterceptor.post(
        "/auth/access-token/refresh",
        payload
      );

      const { data } = response;
      if (!data?.isSuccessful) {
        window.location.href = "/login";
        return;
      }

      localStorage.setItem(
        "user",
        JSON.stringify({
          accessToken: data.accessToken,
          refreshToken: data.refreshToken,
        })
      );

      error.config.headers["Authorization"] = `bearer ${data.accessToken}`;

      return axios(error.config);
    } catch {
      window.location.href = "/login";
    }
  }
);

export default jwtInterceptor;
