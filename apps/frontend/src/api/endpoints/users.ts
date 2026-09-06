import {api} from '../client';
import type {User} from '../types';

export const listUsers = () => api < User[] > ('/api/Users');
export const getUser = (id: number) => api < User > (`/api/Users/${id}`);
export const updateUser = (id: number, payload: Partial < User > ) => api < void > (`/api/Users/${id}`, {
   method: 'PUT',
   body: JSON.stringify(payload)
});
export const deleteUser = (id: number) => api < void > (`/api/Users/${id}`, {
   method: 'DELETE'
});
export const approveUser = (id: number) => api < void > (`/api/Users/${id}/approve`, {
   method: 'POST'
});
export const changePassword = (id: number, newPassword: string, oldPassword: string) => api < void > (`/api/Users/${id}/change-password`, {
   method: 'POST',
   body: JSON.stringify({
      userId: id,
      newPassword,
      oldPassword
   })
});