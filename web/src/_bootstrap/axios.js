import axios from "axios";

export default function bootstrapAxios() {
  axios.defaults.baseURL = process.env.REACT_APP_API_BASEURL;
}
