import {api} from './client';
import type {Category} from './types';
export const listCategories = () => api < Category[] > ('/api/Categories');
export const createCategory = (payload: Partial < Category > ) => api < Category > ('/api/Categories', {
   method: 'POST',
   body: JSON.stringify(payload)
});
export const getCategory = (id: number) => api < Category > (`/api/Categories/${id}`);
export const updateCategory = (id: number, payload: Partial < Category > ) => api < void > (`/api/Categories/${id}`, {
   method: 'PUT',
   body: JSON.stringify(payload)
});
export const deleteCategory = (id: number) => api < void > (`/api/Categories/${id}`, {
   method: 'DELETE'
});