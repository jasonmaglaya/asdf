import { createSlice } from "@reduxjs/toolkit";

const errorMessagesSlice = createSlice({
  name: "errorMessages",
  initialState: {
    messages: [],
  },
  reducers: {
    setErrorMessages: (state, action) => {
      state.messages = action.payload;
    },
  },
});

export const { setErrorMessages } = errorMessagesSlice.actions;

export default errorMessagesSlice.reducer;
