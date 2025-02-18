import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { getAppSettings } from "../services/settingsService";

// Async action to fetch settings from an API
export const fetchAppSettings = createAsyncThunk(
  "appSettings/fetchAppSettings",
  async () => {
    const { data } = await getAppSettings();
    return data.result;
  }
);

const appSettingsSlice = createSlice({
  name: "appSettings",
  initialState: {
    appSettings: { currency: "USD", locale: "en-US" },
    isLoading: true,
    error: null,
  },
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchAppSettings.pending, (state) => {
        state.appSettings = {};
        state.isLoading = true;
        state.error = null;
      })
      .addCase(fetchAppSettings.fulfilled, (state, action) => {
        state.appSettings = action.payload;
        state.isLoading = false;
        state.error = null;
      })
      .addCase(fetchAppSettings.rejected, (state, action) => {
        state.appSettings = {};
        state.isLoading = false;
        state.error = action.error.message;
      });
  },
});

export default appSettingsSlice.reducer;
