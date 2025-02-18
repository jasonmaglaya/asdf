// src/store/store.js
import { configureStore } from "@reduxjs/toolkit";
import appSettingsReducer from "./appSettingsSlice";
import userSlice from "./userSlice";
import errorMessagesReducer from "./errorMessagesSlice";

export const store = configureStore({
  reducer: {
    appSettings: appSettingsReducer,
    user: userSlice,
    errorMessages: errorMessagesReducer,
  },
});
