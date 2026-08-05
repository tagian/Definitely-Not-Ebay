/* eslint-disable @typescript-eslint/no-explicit-any */
import {api} from './client';
import type {AuthResponse,User} from './types';

export const login = (email: string, password: string) =>
api < AuthResponse > ('/api/Auth/login', {
      method: 'POST',
      body: JSON.stringify({
         email,
         password
      })
   });


export const register = (payload: any) =>
   api < void > ('/api/Auth/register', {
      method: 'POST',
      body: JSON.stringify(payload)
   });


export const me = () => api < User > ('/api/Users/me');

export const updateMe = (payload: Partial<User>) =>
   api<User | void>("/api/Users/me", {
    method: "POST",
   body: JSON.stringify(payload),
  });

