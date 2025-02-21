// src/store/store.js
import { configureStore } from "@reduxjs/toolkit";
import appSettingsReducer from "./appSettingsSlice";
import userSlice from "./userSlice";
import messagesReducer from "./messagesSlice";

export const store = configureStore({
  reducer: {
    appSettings: appSettingsReducer,
    user: userSlice,
    messages: messagesReducer,
  },
});
