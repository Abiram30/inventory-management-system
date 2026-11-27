import { createSlice, type PayloadAction } from '@reduxjs/toolkit';

interface AuthState {
  isLoggedIn: boolean;
  username: string;
  userRole: string;
}

const initialState: AuthState = {
  isLoggedIn: false,
  username: 'Admin_01',
  userRole: 'Admin',
};

export const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    loginSuccess: (state, action: PayloadAction<{ username: string; userRole: string }>) => {
      state.isLoggedIn = true;
      state.username = action.payload.username;
      state.userRole = action.payload.userRole;
    },
    logout: (state) => {
      state.isLoggedIn = false;
    },
  },
});

export const { loginSuccess, logout } = authSlice.actions;
export default authSlice.reducer;