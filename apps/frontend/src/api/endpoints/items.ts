import {api, ApiError} from '../client';
import type {Item} from '../types';

export type OrderDir = 'asc' | 'desc';

export interface ListItemsParams {
  search?: string;
  orderBy?: string;      //  "price", "createdAt"
  orderDir?: OrderDir;   // "asc" | "desc"
  categoryId?: number;
  page?: number;         // default 1 (server-side)
  pageSize?: number;     // default 20 (server-side)
  groupByCategory?: boolean;
}

function toQuery(params?: object) {
  if (!params) return '';
  const entries = Object.entries(params as Record<string, unknown>)
    .filter(([, v]) => v !== undefined && v !== null && v !== '');
  if (!entries.length) return '';
  const usp = new URLSearchParams();
  for (const [k, v] of entries) usp.append(k, String(v));
  return `?${usp.toString()}`;
}


export const listItems = <T = Item[]>(params?: ListItemsParams) => api<T>(`/api/Items${toQuery(params)}`);

export const getItem = (id: number) => api < Item > (`/api/Items/${id}`);
export const createItem = (payload: Partial < Item > ) => api < Item > ('/api/Items', {
   method: 'POST',
   body: JSON.stringify(payload)
});
export const updateItem = (id: number, payload: Partial < Item > ) => api < void > (`/api/Items/${id}`, {
   method: 'PUT',
   body: JSON.stringify(payload)
});
export const deleteItem = (id: number) => api < void > (`/api/Items/${id}`, {
   method: 'DELETE'
});
// export const listMyItems = () => api < Item[] > (`/api/Items/seller/mine`);

export async function listMyItems() {
  try {
    return await api<Item[]>('/api/Items/seller/mine');
  } catch (e) {
    if (e instanceof ApiError && e.status === 404) return [];
    throw e;
  }
}
