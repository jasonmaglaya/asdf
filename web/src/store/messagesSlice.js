import { createSlice } from "@reduxjs/toolkit";

const messagesSlice = createSlice({
  name: "messages",
  initialState: {
    errorMessages: [],
    successMessages: [],
  },
  reducers: {
    setErrorMessages: (state, action) => {
      state.errorMessages = action.payload;
    },
    setSuccessMessages: (state, action) => {
      state.successMessages = action.payload;
    },
  },
});

export const { setErrorMessages, setSuccessMessages } = messagesSlice.actions;

export default messagesSlice.reducer;
