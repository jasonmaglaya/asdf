import { useEffect } from "react";
import { Outlet } from "react-router-dom";
import { useDispatch } from "react-redux";
import { fetchAppSettings } from "../store/appSettingsSlice";
import LoadingScreen from "../components/_shared/LoadingScreen";
import { useSelector } from "react-redux";

export const AuthLayout = () => {
  const dispatch = useDispatch();
  const { isLoading } = useSelector((state) => state.appSettings);

  useEffect(() => {
    dispatch(fetchAppSettings());
  }, [dispatch]);

  return isLoading ? <LoadingScreen /> : <Outlet />;
};
