import { create } from 'zustand'; //global state
import type { User } from '../api/types';
import { me } from '../api/endpoints/auth';

type AuthStatus = 'idle' | 'checking' | 'authed' | 'guest';

type AuthState = {
  token: string | null;
  user: User | null;
  status: AuthStatus;
  setAuth: (t: string | null, u: User | null) => void;
  logout: () => void;
  hydrate: () => Promise<void>;
};

export const useAuth = create<AuthState>((set, get) => ({
  token: localStorage.getItem('token'),
  user: null,
  status: 'idle',

  setAuth: (token, user) => {
    if (token) localStorage.setItem('token', token);
    else localStorage.removeItem('token');
    set({ token, user, status: token && user ? 'authed' : token ? 'checking' : 'guest' });
  },

  logout: () => {
    localStorage.removeItem('token');
    set({ token: null, user: null, status: 'guest' });
  },

  hydrate: async () => {
    const { token } = get();
    if (!token) {
      set({ status: 'guest', user: null });
      return;
    }
    // token exists, try to fetch profile
    set({ status: 'checking' });
    try {
      const profile = await me();
      set({ user: profile, status: 'authed' });
    } catch {
      // token invalid or me() failed — clear auth
      localStorage.removeItem('token');
      set({ token: null, user: null, status: 'guest' });
    }
  },
}));
