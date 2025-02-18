import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { getUserFeatures } from "../services/settingsService";
import { getUser } from "../services/usersService";

export const fetchUser = createAsyncThunk("user/fetchUser", async () => {
  const { data } = await getUser();
  return data;
});

export const fetchFeatures = createAsyncThunk(
  "user/fetchFeatures",
  async () => {
    const { data } = await getUserFeatures();
    return data.result;
  }
);

const userSlice = createSlice({
  name: "user",
  initialState: {
    user: null,
    role: {},
    credits: 0,
    features: [],
    isLoading: true,
    error: [],
    loadingFeatures: true,
  },
  reducers: {
    setCredits: (state, action) => {
      state.credits = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchUser.pending, (state) => {
        state.user = null;
        state.role = {};
        state.credits = 0;
        state.isLoading = true;
        state.error = [];
      })
      .addCase(fetchUser.fulfilled, (state, action) => {
        state.user = action.payload.result;
        state.role = action.payload.result.role;
        state.credits = action.payload.result.credits;
        state.isLoading = false;
        state.error = [];
      })
      .addCase(fetchUser.rejected, (state, action) => {
        state.user = null;
        state.role = {};
        state.credits = 0;
        state.isLoading = false;
        state.error = action.payload?.errors;
      })
      .addCase(fetchFeatures.pending, (state) => {
        state.features = [];
        state.loadingFeatures = true;
      })
      .addCase(fetchFeatures.fulfilled, (state, action) => {
        state.features = action.payload;
        state.loadingFeatures = false;
      })
      .addCase(fetchFeatures.rejected, (state) => {
        state.features = [];
        state.loadingFeatures = false;
      });
  },
});

export const { setCredits } = userSlice.actions;

export default userSlice.reducer;
